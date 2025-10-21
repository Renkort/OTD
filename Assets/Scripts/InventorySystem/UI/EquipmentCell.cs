using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCell : CellUI
{
    public enum EquipmentType
    {
        LeftHand, RightHand, Body, Head
    }

    public EquipmentType equipmentType;
}
