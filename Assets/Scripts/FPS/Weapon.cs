using System.Collections;
using UnityEngine;
using Akkerman.UI;


namespace Akkerman.FPS
{
    
    public class Weapon : HoldableItem
    {
        [SerializeField] private WeaponSO weaponData;
        [SerializeField] private bool useADS = false;
        [SerializeField] private AudioSource shootingChanel;
        [SerializeField] private AudioSource reloadingChanel;

        [SerializeField] private int itemIndex;
        [SerializeField] private bool isShooting, readyToShoot;
        private bool allowReset = true;
        [SerializeField] private float shootingDelay = 2f;

        [SerializeField] private int bulletsPerBurst = 3;
        [SerializeField] private int burstBulletsLeft;

        [Header("Spread")]
        [SerializeField] private float spreadIntensity;
        [SerializeField] private float hipSpreadIntensity;
        [SerializeField] private float adsSpreadIntensity;

        [SerializeField] private ParticleSystem muzzleEffect;

        [Header("Bullet Settings")]
        [SerializeField] private GameObject hitscanBulletPrefab;
        [SerializeField] private GameObject physicsBulletPrefab;
        [SerializeField] private bool usePhysicsBullets = true;
        [SerializeField] private Transform bulletSpawn;
        [SerializeField] private float bulletVelocity = 100f;
        [SerializeField] private float bulletPrefabLifeTime = 3f;

        [Header("Loading")]
        [SerializeField] private float reloadTime;
        [SerializeField] private int magazineSize, bulletsLeft, bulletsAmount;
        public int BulletsLeft => bulletsLeft;
        [SerializeField] private bool isReloading;

        public Vector3 spawnPosition;
        public Vector3 spawnRotation;
        public bool IsUsing = false;
        private bool isADS;

        public enum WeaponModel
        {
            A3500X,
            Agram2000
        }

        public WeaponModel thisWeaponModel;


        private Animator animator;

        public enum ShootingMode
        {
            Single,
            Burst, 
            Auto
        }
        [SerializeField] private ShootingMode currentShootingMode;

        private void Awake()
        {
            readyToShoot = true;
            burstBulletsLeft = bulletsPerBurst;
            animator = GetComponent<Animator>();

            spreadIntensity = hipSpreadIntensity;
        }

        private void Start()
        {
            UpdateUI();
            if (!IsUsing)
                animator.enabled = false;
        }

        private void Update()
        {
            if (!IsUsing)
                return;

            if (Input.GetMouseButtonDown(1) && useADS)
            {
                EnterADS();
            }

            if (Input.GetMouseButtonUp(1) && useADS)
            {
                ExitADS();
            }
                
            if (bulletsLeft == 0 && isShooting)
            {
                //SoundManager.Instance.dryfireSoundA3500X.Play();
                Reload();
            }

            if (currentShootingMode == ShootingMode.Auto)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (currentShootingMode == ShootingMode.Single || 
            currentShootingMode == ShootingMode.Burst)
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize)
            {
                Reload();
            }
            if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0)
            {
                //Reload();
            }

            if (readyToShoot && isShooting && bulletsLeft > 0 && !isReloading)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            
        }
        public override void UpdateUI()
        {
            if (!IsUsing)
                return;

            string ammoText = $"{bulletsLeft/bulletsPerBurst} | {bulletsAmount/bulletsPerBurst}";
            GameUI.Instance.IngameUI.SetAmmoUI(ammoText, weaponData.bulletIcon);
        }

        private void EnterADS()
        {
            animator.SetTrigger("enterADS");
            isADS = true;
            GameUI.Instance.IngameUI.SetActiveCrossUI(CrossUIType.Dot, false);
            spreadIntensity = adsSpreadIntensity;
        }
        private void ExitADS()
        {
            animator.SetTrigger("exitADS");
            isADS = false;
            GameUI.Instance.IngameUI.SetActiveCrossUI(weaponData.crossUIType, true);
            spreadIntensity = hipSpreadIntensity;
        }

        private void FireWeapon()
        {
            bulletsLeft--;
            muzzleEffect.Play();
            Player.Instance.FpsController.ShakeCameraRotation(0.2f, 3f);

            if (isADS)
            {
                animator.SetTrigger("RECOIL_ADS");
            }
            else
            {
                animator.SetTrigger("RECOIL");
            }
            shootingChanel.PlayOneShot(weaponData.ShootingSound);
            UpdateUI();

            readyToShoot = false;
            Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;


            GameObject bulletPrefab = usePhysicsBullets ? physicsBulletPrefab : hitscanBulletPrefab;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            bullet.transform.forward = shootingDirection;
            // bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
            // StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

            if (allowReset)
            {
                Invoke("ResetShot", shootingDelay);
                allowReset = false;
            }

            if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
            {
                burstBulletsLeft--;
                Invoke("FireWeapon", shootingDelay);
            }

        }

        private void Reload()
        {
            if (bulletsAmount <= 0)
                return;
            reloadingChanel.PlayOneShot(weaponData.ReloadSound);
            animator.SetTrigger("RELOAD");

            isReloading = true;
            Invoke("ReloadCompleted", reloadTime);
        }
        private void ReloadCompleted()
        {
            //bulletsLeft = magazineSize;
            if (bulletsAmount < magazineSize)
            {
                bulletsLeft = bulletsAmount;
                bulletsAmount = 0;
            }
            else
            {
                bulletsAmount -= (magazineSize - bulletsLeft);
                bulletsLeft = magazineSize;
            }
            isReloading = false;
            UpdateUI();
        }
        private void ResetShot()
        {
            readyToShoot = true;
            allowReset = true;
        }

        Vector3 CalculateDirectionAndSpread()
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(100);
            }

            Vector3 direction = targetPoint - bulletSpawn.position;

            float x = Random.Range(-spreadIntensity, spreadIntensity);
            float y = Random.Range(-spreadIntensity, spreadIntensity);

            return direction + new Vector3(x, y, 0f);
        }
        private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(bullet);
        }

        public void EnableWeapon()
        {
            IsUsing = true;
            animator.enabled = true;
            UpdateUI();
        }
        public void AddAmmo(int ammo)
        {
            bulletsAmount += ammo;
            UpdateUI();
        }
    }
}
