using UnityEngine;

namespace Akkerman.FPS
{
    public enum FireMode { Single, Burst, Auto }

    [CreateAssetMenu(menuName = "Akkerman/FPS/Weapon Data", fileName = "New Weapon")]
    public class WeaponData : ScriptableObject
    {
        [Header("GENERAL")]
        public GameObject bulletPrefab;
        public FireMode fireMode = FireMode.Single;

        [Header("DAMAGE / SHOT")]
        public float damage = 10f;
        public int pelletsPerShot = 1;
        public float spreadAngle = 0f;

        [Header("RATE OF FIRE")]
        public float fireRate = 5f; // shots per second
        public int burstCount = 3;
        public float burstInterval = 0.05f;

        [Header("AMMO")]
        public int magazineSize = 12;
        public float reloadTime = 1.5f;
        public bool infiniteAmmo = false;
        
        [Header("UI")]
        public string weaponName;
        public Sprite weaponIcon;

        [Header("FX")]
        public GameObject muzzleFlashPrefab;
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;

        [Header("RECOIL")]
        public float recoilKick = 1f;
        public float recoilRecoverySpeed = 8f;
    }
}