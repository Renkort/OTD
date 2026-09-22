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

        [Header("SHOT KICK")]
        public float kickBack;         // толчок назад по Z (оружие "прилетает" к лицу)
        public float kickUp;           // подъём ствола вверх, градусы
        public float kickSideAngle;    // случайный уход влево/вправо, градусы
        public float kickRandomness;   // 0-1, разброс направления/силы толчка
        public float kickSnappiness;   // скорость "снаппинга" к цели толчка (резкость самого удара)
        public float kickReturnSpeed;  // скорость затухания цели толчка к нулю (как быстро "прощается" накопленный recoil)
    }
    
}
