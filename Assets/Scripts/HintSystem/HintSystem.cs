using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Akkerman.Hints
{
    
    public class HintSystem : MonoBehaviour
    {
        [System.Serializable]
        public class Hint
        {
            public string HintText;
            public Sprite Icon;
            public float Duration; //0 - infinity time
            public bool ShowOnlyOnce;
            [HideInInspector] public bool wasShown;
        }

        public List<Hint> hints = new();
        public GameObject hintPanel;
        public TextMeshProUGUI hintText;
        public Image hintIcon;

        private Coroutine currentHintCoroutine;

        void Start()
        {
            if (hintPanel != null)
                hintPanel.SetActive(false);
        }

        public void ShowHint(int hintIndex)
        {
            if (hintIndex < 0 || hintIndex >= hints.Count)
            {
                Debug.LogWarning("Hint index out of range!");
                return;
            }

            Hint hint = hints[hintIndex];

            if (hint.ShowOnlyOnce && hint.wasShown)
                return;

            if (currentHintCoroutine != null)
            {
                StopCoroutine(currentHintCoroutine);
                hintPanel.SetActive(false);
            }

            hintText.text = hint.HintText;
            if (hint.Icon != null)
            {
                hintIcon.gameObject.SetActive(true);
                hintIcon.sprite = hint.Icon;
            }
            else
            {
                hintIcon.gameObject.SetActive(false);
            }
            hintPanel.SetActive(true);
            hint.wasShown = true;
            if (hint.Duration > 0)
            {
                currentHintCoroutine = StartCoroutine(HideAfterDelay(hint.Duration));
            }
        }

        public void HideHint()
        {
            if (currentHintCoroutine != null)
                StopCoroutine(currentHintCoroutine);
            hintPanel.SetActive(false);
        }

        private IEnumerator HideAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            hintPanel.SetActive(false);
            currentHintCoroutine = null;
        }
    }

}
