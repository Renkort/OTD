using UnityEngine;
using UnityEngine.UI;

namespace Akkerman.InventorySystem
{
    
    public class ItemUI : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private int quantity;

        public void SetData(Sprite sprite, int quantity)
        {
            itemImage.sprite = sprite;
            this.quantity = quantity;
        }

        public void ResetData()
        {
            itemImage.sprite = null;
            quantity = 0;
        }
    }
}
