using Akkerman.UI;
using UnityEngine;


namespace Akkerman.FPS
{
    
    [CreateAssetMenu(fileName="New Weapon Data", menuName="Akkerman/FPS/Complex Weapon Data")]
    public class WeaponSO : ScriptableObject
    {
        public WeaponComplex.WeaponModel weaponModel;
        public Sprite bulletIcon;
        public AudioClip ShootingSound;
        public AudioClip ReloadSound;
        public int bulletsPerShot;
    }
}

