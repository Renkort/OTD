using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Akkerman.InventorySystem
{
    
    [CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory/InventoryData")]
    public class InventoryData : ScriptableObject
    {
        public int CellCount;
        public List<CellData> CellDatas;
        public List<CellData> EquipmentCellDatas;
    }
}
