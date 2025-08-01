using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class IngameUI : MonoBehaviour, IDataPersistance
{
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Animator whiteFadeScreen;
    private float deltaTime = 0.0f;
    private Transform playerTransform;

    public Animator WhiteFadeScreen => whiteFadeScreen;

    void Start()
    {
        playerTransform = Player.Instance.gameObject.GetComponent<Transform>();

        deathScreen.SetActive(false);
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

    private void HandleSaveGameInput()
    {
        if (Player.Instance.DialogueUI.IsOpen)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.F5) && !Player.Instance.IsDead)
        {
            Debug.Log("SAVING...");
            DataPersistenceManager.Instance.SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            Debug.Log("LOADING...");
            DataPersistenceManager.Instance.LoadGame();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            showDebugInfo = !showDebugInfo;
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
