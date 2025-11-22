using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Akkerman.Audio;

namespace Akkerman.UI
{
    
    public class LoadPrefs : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] private bool canUse = false;
        [SerializeField] private MainMenu mainMenu;
        [SerializeField] private SoundMixerHandler soundMixerHandler;

        [Header("Volume Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI masterVolumeText;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;

        [Header("Brightness Settings")]
        [SerializeField] private Slider brightnessSlider = null;
        [SerializeField] private TextMeshProUGUI brightnessTextValue = null;

        [Header("Quality Level Settings")]
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [Header("Fullscreen Settings")]
        [SerializeField] private Toggle fullscreenToggle;

        [Header("Sensitivity Settings")]
        [SerializeField] private Slider mouseSenSlider = null;
        [SerializeField] private TextMeshProUGUI mouseSensValueText = null;
        [Header("Invert Y Settings")]
        [SerializeField] private Toggle invertYToggle = null;

        void Awake()
        {
            if (canUse)
            {
                if (PlayerPrefs.HasKey("masterVolume"))
                {
                    float localVolume = PlayerPrefs.GetFloat("masterVolume");
                    masterVolumeText.text = localVolume.ToString("0.00");
                    masterVolumeSlider.value = localVolume;
                    soundMixerHandler.SetMasterVolume(localVolume);
                }
                else
                {
                    mainMenu.OnResetButtonClicked("audio");
                }
                if (PlayerPrefs.HasKey("musicVolume"))
                {
                    float localMusicVolume = PlayerPrefs.GetFloat("musicVolume");
                    musicVolumeText.text = localMusicVolume.ToString("0.00");
                    musicVolumeSlider.value = localMusicVolume;
                    soundMixerHandler.SetMusicVolume(localMusicVolume);
                }
                else
                {
                    mainMenu.OnResetButtonClicked("audio");
                }
                if (PlayerPrefs.HasKey("sfxVolume"))
                {
                    float localSfxVolume = PlayerPrefs.GetFloat("sfxVolume");
                    sfxVolumeText.text = localSfxVolume.ToString("0.00");
                    sfxVolumeSlider.value = localSfxVolume;
                    soundMixerHandler.SetSoundFXVolume(localSfxVolume);
                }
                else
                {
                    mainMenu.OnResetButtonClicked("audio");
                }

                if (PlayerPrefs.HasKey("masterQuality"))
                {
                    int localQuality = PlayerPrefs.GetInt("masterQuality");
                    qualityDropdown.value = localQuality;
                    QualitySettings.SetQualityLevel(localQuality);
                }

                if (PlayerPrefs.HasKey("masterFullscreen"))
                {
                    int localFullscreen = PlayerPrefs.GetInt("masterFullscreen");

                    if (localFullscreen == 1)
                    {
                        Screen.fullScreen = true;
                        fullscreenToggle.isOn = true;
                    }
                    else
                    {
                        Screen.fullScreen = false;
                        fullscreenToggle.isOn = false;
                    }
                }

                if (PlayerPrefs.HasKey("masterBrightness"))
                {
                    float localBrightness = PlayerPrefs.GetFloat("masterBrightness");

                    brightnessTextValue.text = localBrightness.ToString("0.0");
                    brightnessSlider.value = localBrightness;
                    //TODO: Apply brightness value to Brightness script on MainCamera;

                }

                if (PlayerPrefs.HasKey("masterSensitivity"))
                {
                    float localSensitivity = PlayerPrefs.GetFloat("masterSensitivity");

                    mouseSensValueText.text = localSensitivity.ToString("0");
                    mouseSenSlider.value = localSensitivity;
                    mainMenu.mainControllerSens = Mathf.RoundToInt(localSensitivity);
                }

                if (PlayerPrefs.HasKey("masterInvertY"))
                {
                    if (PlayerPrefs.GetInt("masterInvertY") == 1)
                    {
                        invertYToggle.isOn = true;
                    }
                    else
                    {
                        invertYToggle.isOn = false;
                    }
                }
            }
        }
    }
}
