using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] private List<HandleItem> handleItems;

    public static ItemHolder Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void SetActiveItem(EquippableItemData itemData, bool isActive)
    {
        if (itemData == null)
            return;
        // DisableAllItems();
        foreach(HandleItem handleItem in handleItems)
        {
            HandleItem? currentActiveItem = GetActiveHandleItemByEquipmentType(itemData.equipmentType);
            if (currentActiveItem != null)
            {
                currentActiveItem?.GameObject.SetActive(false);
            }
        }

        foreach (HandleItem handleItem in handleItems)
        {
            if (handleItem.EquipableItem == itemData)
            {
                handleItem.GameObject.SetActive(isActive);
                break;
            }
        }
    }
    
    private HandleItem? GetActiveHandleItemByEquipmentType(EquipmentCell.EquipmentType type)
    {
        foreach (HandleItem handleItem in handleItems)
        {
            if (handleItem.GameObject.activeInHierarchy && handleItem.EquipableItem.equipmentType == type)
                return handleItem;
        }
        return null;
    }
    
    public void DisableAllItems()
    {
        foreach(HandleItem handleItem in handleItems)
        {
            handleItem.GameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public struct HandleItem
{
    public EquippableItemData EquipableItem;
    public GameObject GameObject;
}
