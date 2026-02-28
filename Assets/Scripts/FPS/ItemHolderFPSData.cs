using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Akkerman.FPS
{
    [CreateAssetMenu(fileName="New IH Data", menuName="Akkerman/FPS/Item Holder Data")]
    public class ItemHolderFPSData : ScriptableObject
    {
        public int currentItemIndex = 0;
        public List<int> avaliableItems;
        public List<int> earlierAvaliableItems = new List<int>();
    }
}
