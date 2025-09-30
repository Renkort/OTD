using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour, IDataPersistance
{

    [SerializeField] private bool IsMainMenu = false;
    [Header("GAME UI PARTS")]
    public static GameUI Instance;
    public MainMenu MainMenu;
    public IngameUI IngameUI;
    public IngameMenu IngameMenu;
    [SerializeField] private string lastOpenedScene;

    private bool activeGameMenu, activeIngameUI, activeMainMenu;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        if (IsMainMenu)
        {
            IngameUI.gameObject.SetActive(false);
            IngameMenu.gameObject.SetActive(false);
        }
        else
        {
            MainMenu.gameObject.SetActive(false);
            IngameMenu.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        HandleInput();
    }

    public void SetActiveIngameMenu(bool isActive)
    {
        IngameMenu.gameObject.SetActive(isActive);
        activeGameMenu = isActive;
        Player.Instance.SetCursorVisible(isActive);
        Time.timeScale = isActive ? 0f : 1f;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsMainMenu)
        {
            if (activeGameMenu)
            {
                SetActiveIngameMenu(false);

            }
            else
            {
                SetActiveIngameMenu(true);
            }
        }
    }

    public void LoadData(GameData data)
    {
        lastOpenedScene = data.LastOpenedScene;
        MainMenu.SaveLastOpenScene(lastOpenedScene);
    }
    public void SaveData(ref GameData data)
    {
        lastOpenedScene = SceneManager.GetActiveScene().name;
        data.LastOpenedScene = lastOpenedScene;
        MainMenu.SaveLastOpenScene(lastOpenedScene);
    }
}
