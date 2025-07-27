using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class IngameUI : MonoBehaviour
{
    [SerializeField] private bool showDebugInfo = false;
    private float deltaTime = 0.0f;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = Player.Instance.gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
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
        Rect posRect = new Rect(0, uiSize*2, w, uiSize);
        Rect memoryRect = new Rect(0, uiSize*3, w, uiSize);

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
}
