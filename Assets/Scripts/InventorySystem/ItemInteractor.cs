using System.Collections.Generic;
using UnityEngine;
using Akkerman.InteractionSystem;
using Akkerman.Audio;

namespace Akkerman.InventorySystem
{

    public class ItemInteractor : InteractionPoint
    {
        [SerializeField] private AudioClip pickUpSFX;
        [SerializeField] private string InteractText;
        [SerializeField] private List<ItemData> items;
        void Start()
        {
            OnInteract += PickUpItems;
            SetInteractText(InteractText);
        }

        private void PickUpItems()
        {
            foreach (ItemData item in items)
            {
                InventoryUI.Instance.AddItemAtEmpty(item);
            }
            //play pick up sound FX
            SoundFXHandler.Instance.PlaySoundFXClip(pickUpSFX, transform, 1f);
            Destroy(gameObject);
        }
    }
}
