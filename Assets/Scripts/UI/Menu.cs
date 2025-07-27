using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private string startScene;
    [SerializeField] private GameObject optionsUI;

    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;

    void Start()
    {
        Debug.Log($"Is has save data: {DataPersistenceManager.Instance.HasGameData()}");
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueGameButton.interactable = false;
        }
        HideOptions();
    }


    public void OnContinueGameClicked()
    {
        DisableMenuButtons();
        SceneManager.LoadSceneAsync(startScene);// TODO load last opened scene
    }

    public void OnLoadGameClicked()
    {

    }
    public void OnNewGameClicked()
    {
        DisableMenuButtons();
        DataPersistenceManager.Instance.NewGame();

        SceneManager.LoadSceneAsync(startScene);
    }
    //OnSaveGameClicked();

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
}
