using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace Akkerman.InventorySystem
{
    
    public class RequireItemInteractor : ItemInteractor2D
    {
        [SerializeField] private int itemQuantity;
        [SerializeField] private List<TextMeshProUGUI> itemRequireText;
        [SerializeField] private UnityEvent OnUsedInteractorEnd;
        private int current;
        public override void InteractWithItems()
        {
            for (int i = 0; i < items.Count; i++)
            {

                if (inventory.RemoveItems(items[i], itemQuantity))
                {
                    itemRequireText[i].text = "";
                    interactIcon.SetActive(false);
                    //Destroy(this);
                    OnUsedInteractorEnd?.Invoke();
                }
                else
                {
                    current = i;
                    StopAllCoroutines();
                    StartCoroutine(RedText());
                }
            }

        }

        private IEnumerator RedText()
        {
            itemRequireText[current].color = Color.red;
            yield return new WaitForSeconds(1);
            itemRequireText[current].color = Color.white;
        }
    }
}
