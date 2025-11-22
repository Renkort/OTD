using System;


namespace Akkerman.InventorySystem
{
    
    [Serializable]
    public class CellData
    {
        public InventoryItem inventoryItem;
        public bool IsEmpty;
        public int Quantity;
        public string ID;

        public CellData()
        {
            IsEmpty = true;
            Quantity = 0;
            ID = Guid.NewGuid().ToString();
        }

        public void SwapCellsData(CellData cellData)
        {
            InventoryItem tmpItemData = inventoryItem;
            bool tmpIsEmtpy = IsEmpty;
            int tmpQuantity = Quantity;
            string tmpID = ID;

            inventoryItem = cellData.inventoryItem;
            cellData.inventoryItem = tmpItemData;

            Quantity = cellData.Quantity;
            cellData.Quantity = tmpQuantity;

            IsEmpty = cellData.IsEmpty;
            cellData.IsEmpty = tmpIsEmtpy;

            ID = cellData.ID;
            cellData.ID = tmpID;
        }
    }
}
