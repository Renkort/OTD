using System.Collections.Generic;
using UnityEngine;
using Akkerman.FPS.Usables;
using Akkerman.UI;

namespace Akkerman.FPS
{

    public class ItemHolderFPS : MonoBehaviour
    {

        [SerializeField] private ItemHolderFPSData data;
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
        public static ItemHolderFPS Instance;

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
            if (data.avaliableItems.Contains(itemKeyIndex)) return;

            data.avaliableItems.Add(itemKeyIndex);

            if (!data.earlierAvaliableItems.Contains(itemKeyIndex))
            {
                data.earlierAvaliableItems.Add(itemKeyIndex);
                data.currentItemIndex = itemKeyIndex;
                SelectItem();
            }
        }

        public void RemoveAvailableItem(int itemKeyIndex)
        {
            data.avaliableItems.Remove(itemKeyIndex);
        }

        private void HandlePlayerInput()
        {
            int previousActiveItem = data.currentItemIndex;

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (data.currentItemIndex >= holdableItems.Count - 1)
                    data.currentItemIndex = 0;
                else
                    data.currentItemIndex++;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (data.currentItemIndex <= 0)
                    data.currentItemIndex = holdableItems.Count - 1;
                else
                    data.currentItemIndex--;
            }

            if (Input.GetKeyDown(KeyCode.Alpha0) && holdableItems.Count >= 1)
            { data.currentItemIndex = 0; }
            if (Input.GetKeyDown(KeyCode.Alpha1) && holdableItems.Count >= 2)
            { data.currentItemIndex = 1; }
            if (Input.GetKeyDown(KeyCode.Alpha2) && holdableItems.Count >= 3)
            { data.currentItemIndex = 2; }
            if (Input.GetKeyDown(KeyCode.Alpha3) && holdableItems.Count >= 4)
            { data.currentItemIndex = 3; }
            if (Input.GetKeyDown(KeyCode.Alpha4) && holdableItems.Count >= 5)
            { data.currentItemIndex = 4; }
            if (Input.GetKeyDown(KeyCode.Alpha5) && holdableItems.Count >= 6)
            { data.currentItemIndex = 5; }

            if (previousActiveItem != data.currentItemIndex)
                SelectItem();
        }

        private void SelectItem()
        {
            if (data.currentItemIndex == -1 || !data.avaliableItems.Contains(data.currentItemIndex))
                return;

            for (int i = 0; i < holdableItems.Count; i++)
            {
                if (holdableItems[i].ButtonNum == data.currentItemIndex)
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
            if (data.currentItemIndex >= holdableItems.Count)
                return;

            targetPosition = playerCamera.TransformPoint(positionOffset);
            holdableItems[data.currentItemIndex].holder.transform.position = Vector3.SmoothDamp(
                holdableItems[data.currentItemIndex].holder.transform.position,
                targetPosition,
                ref positionVelocity,
                positionSpeed * Time.deltaTime
            );
            // Vector3 targetRotation = playerCamera.rotation.eulerAngles - rotationOffset;
            //Quaternion targetRotation = playerCamera.rotation * Quaternion.Euler(rotationOffset);
            Quaternion targetRotation = playerCamera.rotation;
            Vector3 targetEuler = targetRotation.eulerAngles + rotationOffset;
            targetRotation = Quaternion.Euler(targetEuler);

            holdableItems[data.currentItemIndex].holder.transform.rotation = QuaternionUtil.SmoothDamp(
                holdableItems[data.currentItemIndex].holder.transform.rotation,
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
