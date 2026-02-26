using System.Collections.Generic;
using UnityEngine;
using Akkerman.FPS.Usables;
using Akkerman.UI;

namespace Akkerman.FPS
{

    public class LightItemHolder : MonoBehaviour
    {
        [SerializeField] private int currentItemIndex = 0;
        [SerializeField] private List<int> avaliableItems;
        [SerializeField] private List<HoldableItemData> holdableItems;

        [Header("CAMERA FOLLOWING")]
        [SerializeField] private Transform playerCamera;
        [SerializeField] private float rotationSpeed = 4f;
        [SerializeField] private float positionSpeed = 10f;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Vector3 rotationOffset;
        private Vector3 rotationVelocity;
        private Vector3 positionVelocity;
        private Vector3 targetPosition;
        public static LightItemHolder Instance;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            SelectItem();
        }

        void Update()
        {
            HandlePlayerInput();
            FollowCamera();
        }

        public void AddAvailableItem(int itemKeyIndex)
        {
            if (avaliableItems.Contains(itemKeyIndex)) return;

            avaliableItems.Add(itemKeyIndex);
        }

        private void HandlePlayerInput()
        {
            int previousActiveItem = currentItemIndex;

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (currentItemIndex >= holdableItems.Count - 1)
                    currentItemIndex = 0;
                else
                    currentItemIndex++;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (currentItemIndex <= 0)
                    currentItemIndex = holdableItems.Count - 1;
                else
                    currentItemIndex--;
            }

            if (Input.GetKeyDown(KeyCode.Alpha0) && holdableItems.Count >= 1)
            { currentItemIndex = 0; }
            if (Input.GetKeyDown(KeyCode.Alpha1) && holdableItems.Count >= 2)
            { currentItemIndex = 1; }
            if (Input.GetKeyDown(KeyCode.Alpha2) && holdableItems.Count >= 3)
            { currentItemIndex = 2; }
            if (Input.GetKeyDown(KeyCode.Alpha3) && holdableItems.Count >= 4)
            { currentItemIndex = 3; }
            if (Input.GetKeyDown(KeyCode.Alpha4) && holdableItems.Count >= 5)
            { currentItemIndex = 4; }
            if (Input.GetKeyDown(KeyCode.Alpha5) && holdableItems.Count >= 6)
            { currentItemIndex = 5; }

            if (previousActiveItem != currentItemIndex)
                SelectItem();
        }

        private void SelectItem()
        {
            if (currentItemIndex == -1 || !avaliableItems.Contains(currentItemIndex))
                return;

            for (int i = 0; i < holdableItems.Count; i++)
            {
                if (holdableItems[i].ButtonNum == currentItemIndex)
                {
                    holdableItems[i].holder.gameObject.SetActive(true);
                    GameUI.Instance.IngameUI.SetActiveCrossUI(holdableItems[i].CrossUIType, true);
                    holdableItems[i].instance.UpdateUI();
                }
                else
                {
                    holdableItems[i].holder.gameObject.SetActive(false);
                }
            }
        }

        private void FollowCamera()
        {
            if (currentItemIndex >= holdableItems.Count)
                return;

            targetPosition = playerCamera.TransformPoint(positionOffset);
            holdableItems[currentItemIndex].holder.transform.position = Vector3.SmoothDamp(
                holdableItems[currentItemIndex].holder.transform.position,
                targetPosition,
                ref positionVelocity,
                positionSpeed * Time.deltaTime
            );
            // Vector3 targetRotation = playerCamera.rotation.eulerAngles - rotationOffset;
            //Quaternion targetRotation = playerCamera.rotation * Quaternion.Euler(rotationOffset);
            Quaternion targetRotation = playerCamera.rotation;
            Vector3 targetEuler = targetRotation.eulerAngles + rotationOffset;
            targetRotation = Quaternion.Euler(targetEuler);

            holdableItems[currentItemIndex].holder.transform.rotation = QuaternionUtil.SmoothDamp(
                holdableItems[currentItemIndex].holder.transform.rotation,
                targetRotation,
                ref rotationVelocity,
                rotationSpeed * Time.deltaTime
            );
        }
        public Weapon GetWeaponByModel(Weapon.WeaponModel weaponModel)
        {
            for (int i = 0; i < holdableItems.Count; i++)
            {
                Weapon weapon = holdableItems[i].instance.GetComponentInChildren<Weapon>();
                if (weapon == null)
                    continue;
                if (weapon.thisWeaponModel == weaponModel)
                    return weapon;
            }
            return null;
        }

    }

    public interface IHoldable
    {
        public void UpdateUI();
    }


    [System.Serializable]
    public struct HoldableItemData
    {
        public int ButtonNum;
        public Transform holder;
        public HoldableItem instance;
        public string HintKey;
        public CrossUIType CrossUIType;
    }
}
