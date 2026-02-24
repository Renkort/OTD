using Akkerman.UI;
using UnityEngine;


namespace Akkerman.FPS
{
    
    [CreateAssetMenu(fileName="New Weapon Data", menuName="Akkerman/FPS/Weapon Data")]
    public class WeaponSO : ScriptableObject
    {
        public Weapon.WeaponModel weaponModel;
        public CrossUIType crossUIType;
        public Sprite bulletIcon;
        public AudioClip ShootingSound;
        public AudioClip ReloadSound;
    }
}

