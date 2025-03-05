using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: By the way, when accepting clicks from the user on the book, use Render Texture instead of Main or Raytracing Camera
public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu; // Assign your Pause Menu Canvas in the Inspector
    private bool isPaused = false;
    Animator Book;
    //public Camera mainCamera; // Assign your camera in the Inspector
    //public GameObject raytracing;
    //public float distanceFromCamera = 1f;

    private void Awake(){
        Book = pauseMenu.GetComponent<Animator>();
    }
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow) && isPaused){
            Debug.Log("Right key is being pressed!");
            Book.SetBool("Right", true);
        } else {
            Book.SetBool("Right", false);
        }

        // Check if Escape is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
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
