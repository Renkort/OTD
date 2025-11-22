using UnityEngine;


namespace Akkerman.InventorySystem
{
    
    public class MouseFollower : MonoBehaviour
    {
        public ItemUI itemUI;
        [SerializeField] private Canvas canvas;

        public void Awake()
        {
            if (canvas == null)
                transform.root.GetComponent<Canvas>();
            if (itemUI == null)
                itemUI = GetComponentInChildren<ItemUI>();
        }

        private void Update()
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform,
                Input.mousePosition,
                canvas.worldCamera, out position);
            transform.position = canvas.transform.TransformPoint(position);
        }

        public void SetData(Sprite sprite, int quantity)
        {
            itemUI.SetData(sprite, quantity);
        }

        public void ResetData()
        {
            itemUI.ResetData();
        }

        public void Toggle(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}
