using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akkerman.InventorySystem
{
    public abstract class CharacterStatModifierSO : ScriptableObject
    {
        public abstract void AffectCharacter(GameObject character, float value);
    }
}
