using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Akkerman.Subtitles
{
    
    public class SubtitleSystem : MonoBehaviour
    {
        [System.Serializable]
        public class Subtitle
        {
            [TextArea(3, 5)] public string Text;
            public float Duration;
            public float FadeTime = 0.5f;
            public Color TextColor = Color.white;
            public bool UseBackground = true;
        }

        public static SubtitleSystem Instance;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private CanvasGroup textCanvasGroup;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private RectTransform panelTransform;

        [Header("Settings")]
        [SerializeField] private float panelOffset = 50f;
        [SerializeField] private float moveSpeed = 1f;

        private Queue<Subtitle> subtitleQueue = new Queue<Subtitle>();
        private Coroutine currentSubtitleCoroutine;
        private Vector2 onScreenPosition;
        private Vector2 offScreenPosition;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            CalculateScreenPosition();
            ResetSubtitlePosition();
        }

        private void CalculateScreenPosition()
        {
            float screenHeight = Screen.height;
            offScreenPosition = new Vector2(0, -panelOffset);
            onScreenPosition = new Vector2(0, panelOffset);
        }
        private void ResetSubtitlePosition()
        {
            panelTransform.anchoredPosition = offScreenPosition;
            textCanvasGroup.alpha = 0;
        }

        public void AddSubtitle(Subtitle subtitle)
        {
            Debug.Log($"DEBUG: Start subtitle. Off Pos: {offScreenPosition}, On Pos: {onScreenPosition}");
            subtitleQueue.Enqueue(subtitle);
            if (currentSubtitleCoroutine == null)
            {
                currentSubtitleCoroutine = StartCoroutine(ShowSubtitles());
            }
        }

        private IEnumerator ShowSubtitles()
        {
            while (subtitleQueue.Count > 0)
            {
                Subtitle currentSub = subtitleQueue.Dequeue();

                subtitleText.text = currentSub.Text;
                subtitleText.color = currentSub.TextColor;
                backgroundImage.enabled = currentSub.UseBackground;

                //onScreenPosition = new Vector2(0, subtitleText.rectTransform.sizeDelta.y / 2 + panelOffset);
                Debug.Log($"DEBUG: Start subtitle. Off Pos: {offScreenPosition}, On Pos: {onScreenPosition}");
                yield return StartCoroutine(MovePanel(onScreenPosition, true, currentSub.FadeTime));

                yield return new WaitForSeconds(currentSub.Duration);

                yield return StartCoroutine(MovePanel(offScreenPosition, false, currentSub.FadeTime));
            }

            currentSubtitleCoroutine = null;
        }

        private IEnumerator MovePanel(Vector2 targetPos, bool appearing, float fadeTime)
        {
            Vector2 startPos = panelTransform.anchoredPosition;
            float startAlpha = textCanvasGroup.alpha;
            float targetAlpha = appearing ? 1f : 0f;
            float time = 0f;

            while (time < fadeTime)
            {
                time += Time.deltaTime * moveSpeed;
                float t = time / fadeTime;
                panelTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                textCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            panelTransform.anchoredPosition = targetPos;
            textCanvasGroup.alpha = targetAlpha;
        }

        //[ContextMenu("Test Subtitle")]
        public void TestSubtitle()
        {
            AddSubtitle(new Subtitle()
            {
                Text = "0 This is test text. ",
                Duration = 6f,
                FadeTime = 0.5f,
                UseBackground = true
            });
        }
    }
}
