using Akkerman.UI;
using UnityEngine;

namespace Akkerman.FPS
{
    [CreateAssetMenu(fileName = "NewHoldableItem", menuName = "Akkerman/FPS/Holdable Item Data")]
    public class HoldableItemData : ScriptableObject
    {
        [Header("HOLDABLE ITEM DATA")]
        public string labelName;
        public Sprite labelIcon;
        public CrossUIType crossUI;

        [Header("SWAY & BOB")]
        public WeaponSwaySettings swaySettings;
    }

    [System.Serializable]
    public struct WeaponSwaySettings
    {
        [Header("LOOK SWAY")]
        public float swayAmount;
        public float swaySmooth;
        public float maxSwayAngle;

        [Header("IDLE")]
        public float idleSwayAmount;
        public float idleSwaySpeed;

        [Header("WALK BOB")]
        public float walkBobSpeed;
        public float walkBobAmount;

        [Header("RUN BOB")]
        public float runBobSpeed;
        public float runBobAmount;

        [Header("SMOOTHING")]
        public float bobSmooth;
    }
    
}
