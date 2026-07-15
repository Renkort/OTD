using UnityEngine;


namespace Akkerman.FPS
{
    public abstract class HoldableItem : MonoBehaviour
    {
        public HoldableItemData HoldableData;
        public abstract void UpdateUI();
    }
}
