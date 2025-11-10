using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace Akkerman.InventorySystem
{
    
    public class CellUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler,
        IEndDragHandler, IDropHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image itemImage;
        public TextMeshProUGUI QuantityText;
        [SerializeField] private Image borderImage;
        public CellData Data;

        public event Action<CellUI> OnItemClicked,
            OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;

        public void Awake()
        {
            Deselect();
        }

        private void Start()
        {
            UpdateUI();
        }

        private void Update()
        {
            // //if (activateItem.WasPerformedThisFrame())
            // if (Input.GetMouseButtonDown(1) && isClicked)
            // {
            //     StartCoroutine(ClickOnCellAction(1));
            //     isClicked = false;
            //     //OnRightMouseBtnClick?.Invoke(this);
            // }
            // //if (selectItem.WasPerformedThisFrame())
            // if (Input.GetMouseButtonDown(0) && isClicked)
            //     StartCoroutine(ClickOnCellAction(0));
            //     //OnItemClicked?.Invoke(this);
        }


        public void Select()
        {
            if (borderImage != null)
                borderImage.enabled = true;
        }
        public void Deselect()
        {
            if (borderImage != null)
                borderImage.enabled = false;
        }


        public void AddItem(ItemData item)
        {
            Data.IsEmpty = false;
            Data.inventoryItem.itemData = item;
            itemImage.sprite = item.Icon;
            if (item.IsStackable)
                Data.Quantity++;
            else Data.Quantity = 1;
            QuantityText.text = Data.Quantity.ToString();
            UpdateUI();
        }

        public void AddItems(ItemData item, int count)
        {
            Data.IsEmpty = false;
            Data.inventoryItem.itemData = item;
            itemImage.sprite = item.Icon;
            if (item.IsStackable)
                Data.Quantity += count;
            else
                Data.Quantity = count;
            QuantityText.text = Data.Quantity.ToString();
            UpdateUI();
        }

        public bool TryRemoveItem(int number = 1)
        {
            if (number > Data.Quantity)
                return false;
            Data.Quantity -= number;
            QuantityText.text = Data.Quantity.ToString();
            if (Data.Quantity == 0)
                ClearCell();
            return true;
        }

        public void ClearCell()
        {
            Data.IsEmpty = true;
            Data.inventoryItem.itemData = null;
            Data.Quantity = 0;
            itemImage.sprite = null;
            QuantityText.text = "";
        }

        public void UpdateUI()
        {
            if (Data.inventoryItem.itemData != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = Data.inventoryItem.itemData.Icon;
                QuantityText.text = Data.Quantity.ToString();
            }
            else
            {
                itemImage.sprite = null;
                itemImage.gameObject.SetActive(false);
                QuantityText.text = "";
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnItemEndDrag?.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Data.IsEmpty)
                return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                OnRightMouseBtnClick?.Invoke(this);
            }
            else
            {
                OnItemClicked?.Invoke(this);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnItemClicked?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {

        }
    }
}
