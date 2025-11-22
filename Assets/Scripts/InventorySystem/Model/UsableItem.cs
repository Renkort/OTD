using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akkerman.InventorySystem
{
    
    [CreateAssetMenu(fileName = "Usable Item Data", menuName = "Inventory/Item Data/Usable Item")]
    public class UsableItem : ItemData, IItemAction, IDestroyableItem
    {
        [SerializeField] private List<ModifierData> modifiersDatas = new List<ModifierData>();
        public AudioClip actionSFX => throw new NotImplementedException();

        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            foreach (ModifierData data in modifiersDatas)
            {
                data.statModifier.AffectCharacter(character, data.value);
            }
            return true;
        }
    }

    public interface IDestroyableItem
    {

    }

    public interface IItemAction
    {
        public AudioClip actionSFX { get; }
        bool PerformAction(GameObject character, List<ItemParameter> itemState);
    }

    [Serializable]
    public class ModifierData
    {
        public CharacterStatModifierSO statModifier;
        public float value;
    }
}
