using System.Collections;
using Akkerman.UI;
using UnityEngine;


namespace Akkerman.FPS
{
    public class Weapon : HoldableItem
    {
        private static readonly int FireHash = Animator.StringToHash("Fire");
        private static readonly int ReloadHash = Animator.StringToHash("Reload");
        [SerializeField] private WeaponData data;
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private ParticleSystem muzzleVFX;
        [SerializeField] private Animator animator;

        public WeaponData Data => data;

        public int CurrentAmmo { get; private set; }
        public int AmmoAmount { get; private set; }
        public bool IsReloading { get; private set; }

        private float nextFireTime;
        private Coroutine fireRoutine;

        public event System.Action OnShoot;


        private void Awake()
        {
            CurrentAmmo = data.magazineSize;
            AmmoAmount = 999;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                OnFireInputDown();
            if (Input.GetMouseButtonUp(0))
                OnFireInputUp();
            if (Input.GetKeyDown(KeyCode.R))
                Reload();
        }

        public void OnFireInputDown()
        {
            if (data.fireMode == FireMode.Auto)
                fireRoutine = StartCoroutine(AutoFireLoop());
            else
                TryFireOnce();
              
        }

        public void OnFireInputUp()
        {
            if (fireRoutine != null)
            {
                StopCoroutine(fireRoutine);
                fireRoutine = null;
            }
        }

        private IEnumerator AutoFireLoop()
        {
            while (true)
            {
                TryFireOnce();
                yield return null;
            }
        }

        private void TryFireOnce()
        {
            if (IsReloading || Time.time < nextFireTime)
                return;

            if (CurrentAmmo <= 0 && !data.infiniteAmmo)
            {
                if (data.emptySound != null)
                    AudioSource.PlayClipAtPoint(data.emptySound, muzzlePoint.position);
                return;
            }

            nextFireTime = Time.time + 1f / data.fireRate;

            if (data.fireMode == FireMode.Burst)
                StartCoroutine(BurstRoutine());
            else
                Shoot();
        }

        private IEnumerator BurstRoutine()
        {
            for (int i = 0; i < data.burstCount; i++)
            {
                Shoot();
                if (i < data.burstCount - 1)
                    yield return new WaitForSeconds(data.burstInterval);
            }
        }

        private void Shoot()
        {
            for (int i = 0; i < data.pelletsPerShot; i++)
            {
                Vector3 dir = ApplySpread(muzzlePoint.forward, data.spreadAngle);
                GameObject bullet = Instantiate(data.bulletPrefab, muzzlePoint.position, Quaternion.LookRotation(dir));
                // bullet.Initialize(data.damage);
            }

            if (!data.infiniteAmmo)
            {
                CurrentAmmo--;
                UpdateUI();
            }
            OnShoot?.Invoke();
            PlayFireFX();
        }

        private Vector3 ApplySpread(Vector3 forward, float angleDegrees)
        {
            if (angleDegrees <= 0f)
                return forward;
            
            float x = Random.Range(-angleDegrees, angleDegrees);
            float y = Random.Range(-angleDegrees, angleDegrees);
            Quaternion spreadRot = Quaternion.Euler(x, y, 0f);
            return spreadRot * forward;
        }

        private void PlayFireFX()
        {
            if (data.muzzleFlashPrefab != null)
            {
                //Instantiate(data.muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation, muzzlePoint);
                muzzleVFX.Play();
            }


            if (data.fireSound != null)
                AudioSource.PlayClipAtPoint(data.fireSound, muzzlePoint.position);

            if (animator != null)
                animator.SetTrigger(FireHash);
        }

        public void Reload()
        {
            if (IsReloading || CurrentAmmo == data.magazineSize) return;
            if (AmmoAmount <= 0) return;
            StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            IsReloading = true;

            if (data.reloadSound != null)
                AudioSource.PlayClipAtPoint(data.reloadSound, muzzlePoint.position);
            if (animator != null)
                animator.SetTrigger(ReloadHash);

            yield return new WaitForSeconds(data.reloadTime);

            int ammoToFull = data.magazineSize - CurrentAmmo;
            if (AmmoAmount < ammoToFull)
            {
                CurrentAmmo += AmmoAmount;
                AmmoAmount = 0;
            }
            else
            {
                CurrentAmmo = data.magazineSize;
                AmmoAmount -= ammoToFull;
            }
            UpdateUI();
            IsReloading = false;
        }

        public override void UpdateUI()
        {
            string ammoText = data.infiniteAmmo ? "" :
            $"{CurrentAmmo} | {AmmoAmount}";
            GameUI.Instance.IngameUI.SetAmmoUI(ammoText, data.weaponIcon);
        }
    }
    
}
