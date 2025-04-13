using UnityEngine;
using UnityEngine.UI;
using System;

public class SaveLoadUI : MonoBehaviour
{
    public static SaveLoadUI Instance;
    public GameObject menuUI; // Assign in Inspector

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowMenu()
    {
        menuUI.SetActive(true);
    }

    public void SaveGame()
    {
        DataPersistenceManager.Instance.SaveGame();
        Debug.Log("Game Saved!");
        menuUI.SetActive(false);
    }

    public void LoadGame()
    {
        DataPersistenceManager.Instance.LoadGame();
        Debug.Log("Game Loaded!");
        menuUI.SetActive(false);
    }
    
    // This will be temp and if the game is worked on more this will be changed to a visual tree that can be clicked through
    public void LoadMajorCheckpoint()
    {
        DataPersistenceManager.Instance.LoadMajorCheckpoint();
        Debug.Log("Major checkpoint loaded!");
        menuUI.SetActive(false);
    }

    public void CloseMenu()
    {
        menuUI.SetActive(false);
    }
}
