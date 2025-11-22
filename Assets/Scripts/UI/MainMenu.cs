using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Akkerman.Audio;
using Akkerman.SaveSystem;
using Akkerman.Localization;


namespace Akkerman.UI
{
    
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string startScene;
        [SerializeField] private GameObject optionsUI;
        [SerializeField] private SoundMixerHandler soundMixerHandler;
        [SerializeField] private TextMeshProUGUI gameVerLabel;

        [Header("LOCALIZATION")]
        [SerializeField] private List<TextMeshProUGUI> textToLocalize;

        [Header("Button SFX Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip buttonClickSfx;

        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueGameButton;
        [SerializeField] private GameObject noSaveGameDialogue = null;
        [SerializeField] private GameObject confirmationPrompt = null;

        [Header("GRAPHICS OPTIONS")]
        [SerializeField] private Slider brightnessSlider = null;
        [SerializeField] private TextMeshProUGUI brightnessTextValue = null;
        [SerializeField] private float defaultBrightness = 1f;

        [Header("Resolution Dropdowns")]
        public TMP_Dropdown ResolutionDropdown;
        private Resolution[] resolutions;

        [Space(10)]
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        private int qualityLevel;
        private bool isFullscreen;
        private float brightnessLevel;

        [Header("CONTROL OPTIONS")]
        [SerializeField] private Slider mouseSenSlider = null;
        [SerializeField] private TextMeshProUGUI mouseSensValueText = null;
        [SerializeField] private int defaultSensitivity = 4;
        public int mainControllerSens = 4;
        [SerializeField] private Toggle invertYToggle = null;

        [Header("AUDIO OPTIONS")]
        [SerializeField] private float defaultVolume = 0.5f;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI masterVolumeText;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;
        private bool isOptionsActive = false;

        [Header("CHANGELOG")]
        [SerializeField] private TextMeshProUGUI changelogLabel;
        [SerializeField] private GameObject changelogScroll;
        private bool isChangelogActive = false;

        void Start()
        {
            LocalizeText();
            resolutions = Screen.resolutions;
            ResolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }


            ResolutionDropdown.AddOptions(options);
            ResolutionDropdown.value = currentResolutionIndex;
            ResolutionDropdown.RefreshShownValue();

            Debug.Log($"Is has save data: {DataPersistenceManager.Instance.HasGameData()}");
            if (!DataPersistenceManager.Instance.HasGameData())
            {
                continueGameButton.interactable = false;
            }
            // HideOptions();
            SetCursorVisible(true);

            gameVerLabel.text = $"[ {Application.productName} ver. {Application.version} ]";

            string changelogPath = Path.Combine(Application.streamingAssetsPath, "CHANGELOG.md");
            if (File.Exists(changelogPath))
            {
                string changelogText = File.ReadAllText(changelogPath);

                changelogLabel.text = changelogText;
            }
        }


        public void OnContinueGameClicked()
        {
            DataPersistenceManager.Instance.LoadGame();
            DisableMenuButtons();
            Debug.Log($"[DEBUG] Last opened scene: {GameUI.Instance.LastOpenedScene}");
            SceneManager.LoadSceneAsync(GameUI.Instance.LastOpenedScene);
        }

        public void OnLoadGameClicked()
        {
            if (!string.IsNullOrEmpty(GameUI.Instance.LastOpenedScene))
            {
                // lastOpenedScene = PlayerPrefs.GetString("SavedLevel");
                SceneManager.LoadSceneAsync(GameUI.Instance.LastOpenedScene);
            }
            else
            {
                noSaveGameDialogue.SetActive(true);
            }
        }
        public void OnNewGameClicked()
        {
            DisableMenuButtons();
            DataPersistenceManager.Instance.NewGame();

            SceneManager.LoadSceneAsync(startScene);
        }

        public void OnOptionsClicked()
        {
            if (optionsUI == null)
                return;
            if (isOptionsActive)
            {
                isOptionsActive = false;
                optionsUI.SetActive(false);
            }
            else
            {
                isOptionsActive = true;
                optionsUI.SetActive(true);
            }
        }


        public void ShowOptions()
        {
            if (optionsUI != null)
                optionsUI.SetActive(true);
        }

        public void HideOptions()
        {
            if (optionsUI != null)
                optionsUI.SetActive(false);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void DisableMenuButtons()
        {
            newGameButton.interactable = false;
            continueGameButton.interactable = false;
        }
        public void PlayClickSFX()
        {
            audioSource.PlayOneShot(buttonClickSfx);
        }

        public void OnResetButtonClicked(string menuType)
        {
            switch (menuType)
            {
                case "graphics":
                    brightnessSlider.value = defaultBrightness;
                    brightnessTextValue.text = defaultBrightness.ToString("0.0");

                    qualityDropdown.value = 4;
                    QualitySettings.SetQualityLevel(1);

                    fullscreenToggle.isOn = true;
                    Screen.fullScreen = true;

                    Resolution currentResolution = Screen.currentResolution;
                    Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
                    ResolutionDropdown.value = resolutions.Length;
                    GraphicsApply();
                    break;
                case "audio":
                    masterVolumeSlider.value = defaultVolume;
                    masterVolumeText.text = defaultVolume.ToString("0.00");
                    soundMixerHandler.SetMasterVolume(defaultVolume);

                    musicVolumeSlider.value = defaultVolume;
                    musicVolumeText.text = defaultVolume.ToString("0.00");
                    soundMixerHandler.SetMusicVolume(defaultVolume);

                    sfxVolumeSlider.value = defaultVolume;
                    sfxVolumeText.text = defaultVolume.ToString("0.00");
                    soundMixerHandler.SetSoundFXVolume(defaultVolume);
                    VolumeApply();
                    break;
                case "gameplay":
                    mouseSensValueText.text = defaultSensitivity.ToString("0");
                    mouseSenSlider.value = defaultSensitivity;
                    mainControllerSens = defaultSensitivity;
                    invertYToggle.isOn = false;
                    GameplayApply();
                    break;
            }
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetBrightness(float brightness)
        {
            brightnessLevel = brightness;
            brightnessTextValue.text = brightness.ToString("0.0");
        }

        public void SetFullscreen(bool isFullscreen)
        {
            this.isFullscreen = isFullscreen;
        }

        public void SetQuality(int qualityIndex)
        {
            qualityLevel = qualityIndex;
        }

        public void GraphicsApply()
        {
            PlayerPrefs.SetFloat("masterBrightness", brightnessLevel);
            // TODO: set brightness with Brightness on MainCamera;

            PlayerPrefs.SetInt("masterQuality", qualityLevel);
            QualitySettings.SetQualityLevel(qualityLevel);

            PlayerPrefs.SetInt("masterFullscreen", isFullscreen ? 1 : 0);
            Screen.fullScreen = isFullscreen;

            StartCoroutine(ConfirmationBox());
        }

        public void UpdateVolumeText(string volumeOption)
        {
            switch (volumeOption)
            {
                case "master":
                    masterVolumeText.text = masterVolumeSlider.value.ToString("0.00");
                    break;
                case "music":
                    musicVolumeText.text = musicVolumeSlider.value.ToString("0.00");
                    break;
                case "sfx":
                    sfxVolumeText.text = sfxVolumeSlider.value.ToString("0.00");
                    break;
            }
        }

        public void VolumeApply()
        {
            PlayerPrefs.SetFloat("masterVolume", masterVolumeSlider.value);
            PlayerPrefs.SetFloat("musicVolume", musicVolumeSlider.value);
            PlayerPrefs.SetFloat("sfxVolume", sfxVolumeSlider.value);
            StartCoroutine(ConfirmationBox());
        }

        public IEnumerator ConfirmationBox()
        {
            confirmationPrompt.SetActive(true);
            yield return new WaitForSeconds(2);
            confirmationPrompt.SetActive(false);
        }

        public void SetControllerSens(float sensitivity)
        {
            mainControllerSens = Mathf.RoundToInt(sensitivity);
            mouseSensValueText.text = sensitivity.ToString("0");
        }

        public void GameplayApply()
        {
            if (invertYToggle.isOn)
            {
                PlayerPrefs.SetInt("masterInvertY", 1);
                //TODO: Set invert Y on PlayerFPSContoller
            }
            else
            {
                PlayerPrefs.SetInt("masterInvertY", 0);
            }
            PlayerPrefs.SetFloat("masterSensitivity", mainControllerSens);
            //TODO: set mouse sensitivity on PlayerFPSController;
            StartCoroutine(ConfirmationBox());
        }

        public void SetCursorVisible(bool isVisible)
        {
            if (isVisible)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            Cursor.visible = isVisible;
        }

        public void ShowHideChangelog()
        {
            if (isChangelogActive)
            {
                changelogScroll.SetActive(false);
                isChangelogActive = false;
            }
            else
            {
                changelogScroll.SetActive(true);
                isChangelogActive = true;
            }

        }

        private void LocalizeText()
        {
            foreach (TextMeshProUGUI UIText in textToLocalize)
            {
                UIText.text = LocalizationLoader.Instance.GetLocalizedLine(UIText.text);
            }
        }
    }
}
