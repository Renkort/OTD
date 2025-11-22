using System.Collections.Generic;
using UnityEngine;


namespace Akkerman.InventorySystem
{
    
    [CreateAssetMenu(fileName = "Equippable Item Data", menuName = "Inventory/Item Data/Equippable Item")]
    public class EquippableItemData : ItemData, IItemAction, IEquippaleItem
    {
        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }
        public EquipmentCell.EquipmentType equipmentType;
        [field: SerializeField]
        public List<CharacterStatModifierSO> StatsToModify { get; private set; }
        [field: SerializeField] private float modifyValue;
        private float defaultValue;

        public bool PerformAction(GameObject character, List<ItemParameter> itemState)
        {
            for (int i = 0; i < StatsToModify.Count; i++)
            {
                StatsToModify[i].AffectCharacter(character, modifyValue);
            }
            return true;
        }

        public void SetModifyValue(float value)
        {
            defaultValue = modifyValue;
            modifyValue = value;
        }

        public void ResetModifyValue()
        {
            modifyValue = defaultValue;
        }
    }

    public interface IEquippaleItem
    {

    }
}
