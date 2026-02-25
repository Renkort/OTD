using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Akkerman.Utils
{
    
    public class SceneLoader : MonoBehaviour
    {
        [Header("FOR TRIGGER ENTER")]
        [SerializeField] private string triggerSceneName;
        [Header("UI SETTINGS")]
        public GameObject loadingScreen;
        public Slider progressBar;
        public TextMeshProUGUI progressText;

        private void Start()
        {
            if (loadingScreen != null)
                loadingScreen.SetActive(false);
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                LoadScene(triggerSceneName);
            }
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            if (loadingScreen != null)
                loadingScreen.SetActive(true);

            Akkerman.FPS.Player.Instance.FreezePlayerActions(true, true);
            // Time.timeScale = 0f;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                float progress = asyncLoad.progress / 0.9f;

                if (progressBar != null)
                    progressBar.value = progress;

                if (progressText != null)
                    progressText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";

                if (asyncLoad.progress >= 0.9f)
                {
                    // yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                    // or automatic
                    yield return new WaitForSeconds(1f);

                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }

            if (loadingScreen != null)
                loadingScreen.SetActive(false);

            // Time.timeScale = 1f;
        }
    }
}