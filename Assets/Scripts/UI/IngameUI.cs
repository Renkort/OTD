using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;
using Akkerman.SaveSystem;
using Akkerman.FPS;
using Akkerman.FPS.Usables;
using System.Collections.Generic;

namespace Akkerman.UI
{
    
    public class IngameUI : MonoBehaviour, IDataPersistance
    {
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private GameObject deathScreen;
        [SerializeField] private Animator whiteFadeScreen;
        [SerializeField] private List<CrossData> crosses;

        [Header("FPS UI")]
        [SerializeField] private TextMeshProUGUI ammoDisplay;
        [SerializeField] private Image weaponBulletIcon;
        public Slider forceModifierSlider;
        [SerializeField] private Throwable throwable;
        [SerializeField] private TextMeshProUGUI itemHolderHint;
        private float deltaTime = 0.0f;
        private Transform playerTransform;

        public Animator WhiteFadeScreen => whiteFadeScreen;

        void Start()
        {
            playerTransform = Player.Instance.gameObject.GetComponent<Transform>();

            deathScreen.SetActive(false);
            forceModifierSlider.maxValue = throwable.ForceModifierLimit;
        }

        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            HandleSaveGameInput();

            if (Player.Instance.IsDead)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log($"Loading last game save...");
                    DataPersistenceManager.Instance.LoadGame();
                }
            }
        }

        public void SetAmmoUI(string ammoDisplay, Sprite weaponBulletIcon)
        {
            this.ammoDisplay.text = ammoDisplay;
            this.weaponBulletIcon.sprite = weaponBulletIcon;
        }

        public void DisplayForceModifierSlider(float sliderValue)
        {
            forceModifierSlider.value = sliderValue;
        }
        public void UpdateItemHolderHint(string hintText)
        {
            itemHolderHint.text = hintText;
        }
        public void SetActiveCrossUI(CrossUIType crossType, bool isActive)
        {
            foreach (var cross in crosses)
            {
                if (cross.Type == crossType)
                {
                    cross.gameObject.SetActive(isActive);
                }
                else
                {
                    cross.gameObject.SetActive(false);
                }
            }
        }

        private void HandleSaveGameInput()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                showDebugInfo = !showDebugInfo;
            }
            if (Player.Instance.DialogueUI.IsOpen)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.F5) && !Player.Instance.IsDead)
            {
                Debug.Log("QUICK SAVING...");
                DataPersistenceManager.Instance.SaveGame();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                Debug.Log("LOADING...");
                DataPersistenceManager.Instance.LoadGame();
            }

        }

        public void ShowDeathScreen()
        {
            deathScreen.SetActive(true);
        }

        //Debug UI
        private void OnGUI()
        {
            if (!showDebugInfo)
            {
                return;
            }

            int w = Screen.width, h = Screen.height;
            int uiSize = h * 2 / 100;
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = uiSize;
            style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            Rect verRect = new Rect(0, 0, w, uiSize);
            Rect fpsRect = new Rect(0, uiSize, w, uiSize);
            Rect posRect = new Rect(0, uiSize * 2, w, uiSize);
            Rect memoryRect = new Rect(0, uiSize * 3, w, uiSize);

            string verText = $"{Application.productName} ver. {Application.version}";

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string fpsText = string.Format("{0:0.0} ms ({1:0} fps), ", msec, fps);

            float x = playerTransform.position.x, y = playerTransform.position.y, z = playerTransform.position.z;
            string posText = string.Format("X: {0:0.00} Y: {1:0.00} Z: {2:0.00}", x, y, z);

            long allocMemory = Profiler.GetTotalAllocatedMemoryLong();
            string allocMemoryText = $"Allocated memory: {allocMemory / 1024 / 1024} MB";

            GUI.Label(verRect, verText, style); // game version
            GUI.Label(fpsRect, fpsText, style); // fps
            GUI.Label(posRect, posText, style); // player position
            GUI.Label(memoryRect, allocMemoryText, style); // allocated memory

        }

        public void LoadData(GameData data)
        {
            showDebugInfo = data.ShowDebugInfo;
            deathScreen.SetActive(data.IsDead);
        }

        public void SaveData(ref GameData data)
        {
            data.ShowDebugInfo = showDebugInfo;
        }
    }

    [System.Serializable]
    public struct CrossData
    {
        public CrossUIType Type;
        public GameObject gameObject;

    }
    public enum CrossUIType
    { Dot, Cross, Shotgun}
}
