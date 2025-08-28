using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryData data;
    private List<CellUI> cells = new List<CellUI>();
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject cellUIPrefab;
    [SerializeField] private GameObject cellContainer;
    [SerializeField] private UIInventoryDescription inventoryDescription;
    [SerializeField] private MouseFollower mouseFollower;
    private bool isOpen;
    private int curBeginDrag = -1;
    private GameObject player;
    private CellUI currentSelected;
    [HideInInspector] public bool IsInCutscene = false;

    public event Action<string> OnRecieveItem, OnRemoveItem;
    public event Action<int> OnDescriptionRequested; //, OnItemActionRequested;
    public static InventoryUI Instance;



    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        mouseFollower.Toggle(false);
        inventoryDescription.ResetDescription();
        player = FindObjectOfType<FPSPlayerController>().gameObject;
    }
    private void Start()
    {
        InitializeInventoryUI();
    }

    private void Update()
    {
        CheckInput();
        // if (isOpen)
        //     SelectCellUI();
    }

    void CheckInput()
    {
        if (IsInCutscene)
            return;
        if (Player.Instance.DialogueUI.IsOpen)
        {
            return;
        }

        if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Joystick1Button11)) && !isOpen)
        {
            inventoryUI.SetActive(true);
            isOpen = true;
            inventoryDescription.ResetDescription();
            DeselectCells();
            //EventSystem.current.SetSelectedGameObject(cellFirstGO);
            Player.Instance.FreezePlayerActions(true, true);
            Player.Instance.SetCursorVisible(true);
        }
        else if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Joystick1Button11)) && isOpen)
        {
            inventoryUI.SetActive(false);
            isOpen = false;
            EventSystem.current.SetSelectedGameObject(null);
            Player.Instance.FreezePlayerActions(false, false);
            Player.Instance.SetCursorVisible(false);
        }
    }

    private void SelectCellUI()
    {
        if (Input.anyKeyDown && EventSystem.current.currentSelectedGameObject.TryGetComponent<CellUI>(out currentSelected))
        {
            Debug.Log($"Current: {EventSystem.current.currentSelectedGameObject.name}");
            HandleItemSelection(currentSelected);
        }
    }

    public void InitializeInventoryUI()
    {
        for (int i = 0; i < data.CellCount; i++)
        {
            GameObject cellGO = Instantiate(cellUIPrefab, cellContainer.transform);
            cellGO.name = $"Cell_{i}";
            CellUI cellUI = cellGO.GetComponent<CellUI>();
            cells.Add(cellUI);
        }
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].OnItemClicked += HandleItemSelection;
            cells[i].OnItemBeginDrag += HandleBeginDrag;
            cells[i].OnItemDroppedOn += HandleSwapItems;
            cells[i].OnItemEndDrag += HandleEndDrag;
            cells[i].OnRightMouseBtnClick += HandlePerformItemActions;
        }
        LoadCellsData();
    }
    private string PrepareDescripriton(InventoryItem item)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(item.itemData.Desctiption);
        sb.AppendLine();
        for (int i = 0; i < item.ItemState.Count; i++)
        {
            sb.Append($"{item.ItemState[i].itemParameter.ParameterName} " +
                $": {item.ItemState[i].value} / {item.itemData.DefaultParametersList[i].value}");
        }
        return sb.ToString();
    }

    private void HandleBeginDrag(CellUI cell)
    {
        curBeginDrag = cells.IndexOf(cell);
        mouseFollower.Toggle(true);
        mouseFollower.SetData(cell.Data.inventoryItem.itemData.Icon, cell.Data.Quantity);
    }

    private void HandleEndDrag(CellUI cell)
    {
        mouseFollower.ResetData();
        mouseFollower.Toggle(false);
    }

    private void HandlePerformItemActions(CellUI cell)
    {
        //if (cell.name != currentSelected.name) { return; }

        IItemAction itemAction = cell.Data.inventoryItem.itemData as IItemAction;
        if (itemAction != null)
        {
            itemAction.PerformAction(player, null);
        }
        IDestroyableItem destroyableItem = cell.Data.inventoryItem.itemData as IDestroyableItem;
        if (destroyableItem != null)
        {
            cell.TryRemoveItem();
        }
        IEquippaleItem equippableItem = cell.Data.inventoryItem.itemData as IEquippaleItem;
        if (equippableItem != null)
        {
            EquipItem(cell);
        }
    }
    private void EquipItem(CellUI cell)
    {
        curBeginDrag = cells.IndexOf(cell);
        HandleSwapItems(cells[11]);
        if (cells.IndexOf(cell) == 11)
        {
            AddItemAtEmpty(cell.Data.inventoryItem.itemData);
            IItemAction itemAction = cell.Data.inventoryItem.itemData as IItemAction;
            EquippableItemData item = cell.Data.inventoryItem.itemData as EquippableItemData;
            //item.SetModifyValue(GlobalValues.DEFAULT_ATTACK);
            itemAction.PerformAction(player, null);
            item.ResetModifyValue();
            cell.TryRemoveItem(1);

        }
    }

    private void HandleItemSelection(CellUI cell)
    {
        int index = cells.IndexOf(cell);
        if (index == -1)
            return;
        OnDescriptionRequested?.Invoke(index);
        if (!cell.Data.IsEmpty)
        {
            inventoryDescription.SetDescription(cell,
                PrepareDescripriton(cell.Data.inventoryItem));
        }
        else
        {
            inventoryDescription.SetDescription("");
        }
        DeselectCells();
        cell.Select();
    }

    private void HandleSwapItems(CellUI endCell)
    {
        int index = cells.IndexOf(endCell);
        IEquippaleItem equippableItem = cells[curBeginDrag].Data.inventoryItem.itemData
            as IEquippaleItem;
        if (index == -1)
            return;
        else if (equippableItem == null && index == 11)
            return;
        cells[curBeginDrag].Data.SwapCellsData(endCell.Data);
        cells[curBeginDrag].UpdateUI();
        cells[cells.IndexOf(endCell)].UpdateUI();
    }

    private void DeselectCells()
    {
        foreach (CellUI cell in cells)
        {
            cell.Deselect();
        }
    }

    public void AddItemAtEmpty(ItemData item)
    {
        OnRecieveItem?.Invoke(item.ItemTitle);
        foreach (CellUI cell in cells)
        {
            if (cell.Data.IsEmpty)
                continue;
            if (cell.Data.inventoryItem.itemData.Id == item.Id && cell.Data.inventoryItem.itemData.IsStackable)
            {
                cell.AddItem(item);
                SaveCellsData();
                return;
            }
        }

        foreach (CellUI cell in cells)
        {
            if (cell.Data.IsEmpty)
            {
                cell.AddItem(item);
                SaveCellsData();
                break;
            }
        }
    }

    public void RemoveItems(ItemData item)
    {
        OnRemoveItem?.Invoke(item.ItemTitle);
        foreach (CellUI cell in cells)
        {
            if (!cell.Data.IsEmpty && cell.Data.inventoryItem.itemData.Id == item.Id)
            {
                cell.TryRemoveItem();
            }
        }
    }

    public bool RemoveItems(ItemData item, int quantity)
    {
        foreach (CellUI cell in cells)
        {
            if (!cell.Data.IsEmpty && cell.Data.inventoryItem.itemData.Id == item.Id)
            {
                if (cell.TryRemoveItem(quantity))
                    return true;
            }
        }
        return false;
    }

    public ItemData GetItem(string id)
    {
        foreach (var cell in data.CellDatas)
        {
            if (cell.inventoryItem.itemData.Id == id)
                return cell.inventoryItem.itemData;
        }
        return null;
    }

    private void LoadCellsData()
    {
        if (data.CellDatas.Count != cells.Count)
        {
            Debug.Log($"INFO: Initializing inventory data...");
            InitializeData();
        }
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Data = data.CellDatas[i];
        }
    }
    private void SaveCellsData()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            data.CellDatas[i] = cells[i].Data;
        }
    }
    private void InitializeData()
    {
        data.CellDatas = new List<CellData>();
        for (int i = 0; i < cells.Count; i++)
        {
            data.CellDatas.Add(cells[i].Data);
        }
    }
}
