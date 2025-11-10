using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akkerman.InventorySystem
{
    public class EquipmentCell : CellUI
    {
        public enum EquipmentType
        {
            LeftHand, RightHand, Body, Head
        }

        public EquipmentType equipmentType;
    }
}
