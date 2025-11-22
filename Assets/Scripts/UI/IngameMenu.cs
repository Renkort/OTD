using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Akkerman.SaveSystem;
using Akkerman.Audio;

namespace Akkerman.UI
{
    
    public class IngameMenu : MonoBehaviour
    {
        [SerializeField] private Button continueGameButton;
        [SerializeField] private Button gameOptionsButton;
        [SerializeField] private Button quitToMenuButton;
        [SerializeField] private Button quitToOSButton;

        [SerializeField] private GameObject gameOptionsPanel;
        [SerializeField] private string mainMenuSceneName;
        [Header("AUDIO")]
        [SerializeField] private AudioClip buttonSelectSFX;

        private bool isOpenOptions = false;


        void Awake()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            continueGameButton.onClick.AddListener(() => OnContinueGameClicked());
            gameOptionsButton.onClick.AddListener(() => OnGameOptionsClicked());
            quitToMenuButton.onClick.AddListener(() => OnQuitToMenuClicked());
            quitToOSButton.onClick.AddListener(() => OnQuitToOSClicked());
        }

        private void OnContinueGameClicked()
        {
            SoundFXHandler.Instance.PlaySoundFXClip(buttonSelectSFX);
            GameUI.Instance.SetActiveIngameMenu(false);
        }

        private void OnGameOptionsClicked()
        {
            SoundFXHandler.Instance.PlaySoundFXClip(buttonSelectSFX);
            if (isOpenOptions)
            {
                isOpenOptions = false;
                gameOptionsPanel.SetActive(false);
            }
            else
            {
                isOpenOptions = true;
                gameOptionsPanel.SetActive(true);
            }
        }
        private void OnQuitToMenuClicked()
        {
            DataPersistenceManager.Instance.SaveGame();
            SoundFXHandler.Instance.PlaySoundFXClip(buttonSelectSFX);
            SceneManager.LoadScene(mainMenuSceneName);
        }
        private void OnQuitToOSClicked()
        {
            SoundFXHandler.Instance.PlaySoundFXClip(buttonSelectSFX);
            DataPersistenceManager.Instance.SaveGame();
            Application.Quit();
        }

    }
}
