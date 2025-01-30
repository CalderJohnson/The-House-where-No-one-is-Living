using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Object_Interact : MonoBehaviour
{   
    public bool isRepeatable = false;
    bool isInteractable = false;
    Collider Player;

    public GameObject interactionIcon;
    public UnityEvent Object_Action; 
    private bool isOpen = false;

    public Dialogue dialogueInstance; // Reference to the Dialogue script
    public string dialogueFileName;  // Name of the dialogue file for this object

    private void Start()
    {
        // Ensure the icon is initially hidden.
        if (interactionIcon != null)
        {
            interactionIcon.SetActive(false);
        }
    }
  
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("enter");
            isInteractable = true;
            Player = other;
        }

        if (other.CompareTag("Player") && !isOpen)
        {
            if (interactionIcon != null)
            {
                interactionIcon.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("exit");
            isInteractable = false;
            Player = null;
        }

        if (other.CompareTag("Player"))
        {
            if (interactionIcon != null)
            {
                interactionIcon.SetActive(false); // Hide the icon.
            }
        }
    }

    void Update()
    {
        if (isInteractable && Input.GetKeyDown(KeyCode.E) && !isOpen)
        {
            if (dialogueInstance != null)
            {
                // Set the dialogue file dynamically before invoking Object_Action
                dialogueInstance.SetDialogueFileName(dialogueFileName);
            }

            Object_Action.Invoke();
            isOpen = true; 
            if (interactionIcon != null)
            {
                interactionIcon.SetActive(false);
            }

            isInteractable = false;
            Player = null;
            
            if(isRepeatable == true){
                delay_time(5f);
                resetState();
            }
        }

    }

    public void resetState()
    {
        isOpen = false;
        //isInteractable = true;
    }

    IEnumerator delay_time(float waitTime){
        yield return new WaitForSeconds(waitTime);
    }
}