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
        // Check if Tab is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //transform.position = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;
            if (isPaused){
                ResumeGame();
            } else {
                PauseGame();
            }
        }

        if(pauseMenu.activeSelf && isPaused){
            if(Input.GetKey(KeyCode.RightArrow)){
                Book.SetBool("Right", true);
            } else {
                Book.SetBool("Right", false);
            }

            if(Input.GetKey(KeyCode.LeftArrow)){
                Book.SetBool("Left", true);
            } else {
                Book.SetBool("Left", false);
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        // raytracing.SetActive(false);
        pauseMenu.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Freeze game time
        // Reset animation state
        Book.SetBool("Tab2Close", false);
    }

    public void ResumeGame()
    {
        // Enable the closing animation to be played
        Book.SetBool("Tab2Close", true);

        // Resume time before waiting for animation
        Time.timeScale = 1f; 

        StartCoroutine(CloseBookAndExit());
    }

    private IEnumerator CloseBookAndExit()
    {
        while (true)
        {
            AnimatorStateInfo currentState = Book.GetCurrentAnimatorStateInfo(0);

            if (currentState.IsName("Book Close") && currentState.normalizedTime >= 1f && !Book.IsInTransition(0))
            {
                break;
            }
            yield return null; // Wait for next frame
        }

        // Now disable the menu
        pauseMenu.SetActive(false);

        isPaused = false;
    }


    public void QuitGame() // Optional: For a quit button in the pause menu
    {
        Application.Quit();
        Debug.Log("Game Quit!"); // Only visible in the editor
    }
}
