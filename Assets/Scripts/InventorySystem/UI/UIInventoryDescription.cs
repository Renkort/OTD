using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Akkerman.InventorySystem
{
    
    public class UIInventoryDescription : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;

        public void Awake()
        {
            ResetDescription();
        }

        public void ResetDescription()
        {
            itemImage.gameObject.SetActive(false);
            title.text = "";
            description.text = "";
        }

        public void SetDescription(CellUI cell, string description)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = cell.Data.inventoryItem.itemData.Icon;
            title.text = cell.Data.inventoryItem.itemData.ItemTitle;
            this.description.text = description;
        }

        public void SetDescription(string description)
        {
            ResetDescription();
            this.description.text = description;
        }
    }
}
