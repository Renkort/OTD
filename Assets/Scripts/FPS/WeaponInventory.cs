using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Akkerman.FPS
{
    public class WeaponInventory : MonoBehaviour
    {
        [Header("SLOTS")]
        [Tooltip("Weapons by key numbers 0,1,2...")]
        [SerializeField] private List<HoldableItem> weaponSlots;

        [Header("SWITCH SETTIGNS")]
        [SerializeField] private float switchDelay = 0.4f;
    	[SerializeField] private float scrollCooldown = 0.15f;

        public HoldableItem CurrentWeapon {get; private set; }
        public int CurrentIndex {get; private set; }
        public List<WeaponData> WeaponDatas {get; private set;}
        public List<Weapon> Weapons {get; private set;}

        public event System.Action<HoldableItemData, int> OnWeaponSwitched;

        private bool isSwitching;
        public bool IsSwitching  => isSwitching;
        private float lastScrollTime;

        private void Awake()
        {
            ReadWeaponSlots();
        }

        private void Start()
        {
            for (int i = 0; i < weaponSlots.Count; i++)
            {
                if (weaponSlots[i] != null)
                {
                    EquipSlot(i, true);
                    break;
                }
            }
        }

        private void Update()
        {
            HandleNumberKeys();
            HandleScrollWheel();
        }

        private void ReadWeaponSlots()
        {
            WeaponDatas = new();
            Weapons = new();
            for (int i = 0; i < weaponSlots.Count; i++)
            {
                Weapon weapon = weaponSlots[i] as Weapon;
                if (weapon != null)
                {
                    Weapons.Add(weapon);
                    WeaponDatas.Add(weapon.Data);
                }
                
            }   
        }

        private void HandleNumberKeys()
        {
            for (int i = 0; i < weaponSlots.Count && i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    TrySwitchTo(i);
                    break;
                }
            }
        }

        private void HandleScrollWheel()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) < 0.01f) return;
            if (Time.unscaledTime - lastScrollTime < scrollCooldown) return;

            lastScrollTime = Time.unscaledTime;

            int direction = scroll > 0f ? -1 : 1; // up - previous, down - next
            int nextIndex = GetNextAvailableSlot(CurrentIndex, direction);

            if (nextIndex != -1)
                TrySwitchTo(nextIndex);
        }

        private int GetNextAvailableSlot(int fromIndex, int direction)
        {
            if (weaponSlots.Count == 0) return -1;

            int index = fromIndex;
            for (int i = 0; i < weaponSlots.Count ; i++)
            {
                index = (index + direction + weaponSlots.Count) % weaponSlots.Count;
                if (weaponSlots[index] != null)
                    return index;
            }
            return -1;
        }

        public void TrySwitchTo(int index)
        {
            if (index < 0|| index >= weaponSlots.Count) return;
            if (weaponSlots[index] == null) return;
            if (index == CurrentIndex) return;
            if (isSwitching) return;

            StartCoroutine(SwitchRoutine(index));
        }

        private IEnumerator SwitchRoutine(int index)
        {
            isSwitching = true;

            if (CurrentWeapon != null)
                CurrentWeapon.gameObject.SetActive(false);

            yield return new WaitForSeconds(switchDelay);

            EquipSlot(index, false);
            isSwitching = false;
        }

        private void EquipSlot(int index, bool instant)
        {
            CurrentIndex = index;
            CurrentWeapon = weaponSlots[index];
            CurrentWeapon.gameObject.SetActive(true);

            weaponSlots[index].UpdateUI();
            OnWeaponSwitched?.Invoke(CurrentWeapon.HoldableData, index);
        }
    }
}
