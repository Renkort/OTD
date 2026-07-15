using System.Collections;
using Akkerman.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Akkerman.FPS
{
    public class WeaponSwitchUI : MonoBehaviour
    {
        [SerializeField] private WeaponInventory inventory;

        [Header("UI REFS")]
        [SerializeField] private CanvasGroup panelGroup;
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TextMeshProUGUI weaponNameText;

        [Header("BEHAVIOUR")]
        [SerializeField] private float showDuration = 0.8f;
        [SerializeField] private float fadeSpeed = 8.0f;
        private Coroutine hideRoutine;

        private void OnEnable()
        {
            inventory.OnWeaponSwitched += HandleWeaponSwitched;   
        }

        private void OnDisable()
        {
            inventory.OnWeaponSwitched -= HandleWeaponSwitched;
        }

        private void HandleWeaponSwitched(HoldableItemData data, int slotIndex)
        {
            if (weaponIcon != null)
                weaponIcon.sprite = data.labelIcon;
            if (weaponNameText != null)
                weaponNameText.text = data.labelName;/*string.IsNullOrEmpty(nameLabel) ? nameLabel : "ITEM";*/
            if (hideRoutine != null)
                StopCoroutine(hideRoutine);
            GameUI.Instance.IngameUI.SetActiveCrossUI(data.crossUI, true);
            
            hideRoutine = StartCoroutine(ShowThenHide());
        }

        private IEnumerator ShowThenHide()
        {
            yield return StartCoroutine(FadeTo(1.0f));
            yield return new WaitForSeconds(showDuration);
            yield return StartCoroutine(FadeTo(0.0f));
        }

        private IEnumerator FadeTo(float target)
        {
            while (Mathf.Abs(panelGroup.alpha - target) > 0.01f)
            {
                panelGroup.alpha = Mathf.MoveTowards(panelGroup.alpha, target, fadeSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
            panelGroup.alpha = target;
        }
    }
}
