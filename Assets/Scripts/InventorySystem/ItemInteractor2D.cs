using UnityEngine;
using System.Collections.Generic;

namespace Akkerman.InventorySystem
{
    
    public class ItemInteractor2D : MonoBehaviour
    {
        [Header("Collider required")]
        [SerializeField] protected new Collider2D collider;
        [SerializeField] protected GameObject interactIcon;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] protected List<ItemData> items;
        protected InventoryUI inventory;
        protected bool isPlayerNearby;

        private void Start()
        {
            inventory = FindObjectOfType<InventoryUI>();
            interactIcon.SetActive(false);
        }

        private void Update()
        {
            CheckPlayer();
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerNearby = true;
                interactIcon.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerNearby = false;
                interactIcon.SetActive(false);
            }
        }
        private void CheckPlayer()
        {

            if (isPlayerNearby && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button0)))
            {
                InteractWithItems();
            }
        }

        public virtual void InteractWithItems()
        {
            foreach (ItemData item in items)
            {
                inventory.AddItemAtEmpty(item);
            }
            Destroy(gameObject);
        }
    }
}
