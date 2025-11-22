using System;
using System.Collections.Generic;
using UnityEngine;

namespace Akkerman.InventorySystem
{
    
    public abstract class ItemData : ScriptableObject
    {
        public string ItemTitle;
        [TextArea] public string Desctiption;
        public Sprite Icon;
        public bool IsStackable;
        public string Id;

        [field: SerializeField] public List<ItemParameter> DefaultParametersList { get; set; }
    }

    [Serializable]
    public struct InventoryItem
    {
        public ItemData itemData;
        [field: SerializeField] public List<ItemParameter> ItemState { get; set; }
    }

    [Serializable]
    public struct ItemParameter : IEquatable<ItemParameter>
    {
        public ItemParameterData itemParameter;
        public float value;

        public bool Equals(ItemParameter other)
        {
            return other.itemParameter == itemParameter;
        }
    }
}
