using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu; // Assign your Pause Menu Canvas in the Inspector
    private bool isPaused = false;
    //public Camera mainCamera; // Assign your camera in the Inspector
    //public GameObject raytracing;
    //public float distanceFromCamera = 1f;

    
    void Update()
    {
        // Check if Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //transform.position = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

    }

    public void PauseGame()
    {
        isPaused = true;
        //raytracing.SetActive(false);
        pauseMenu.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Freeze game time
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false); // Hide the pause menu
        
        Time.timeScale = 1f; // Resume game time
    }

    public void QuitGame() // Optional: For a quit button in the pause menu
    {
        Application.Quit();
        Debug.Log("Game Quit!"); // Only visible in the editor
    }
}
