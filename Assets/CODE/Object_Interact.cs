using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Object_Interact : MonoBehaviour
{
    bool isInteractable = false;
    Collider Player;
    
    public GameObject interactionIcon;
    public UnityEvent Object_Action; 
    private bool isOpen = false;

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

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {


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

        if (isInteractable && Input.GetKeyDown(KeyCode.Q) && !isOpen )
        {
            Object_Action.Invoke();
            isOpen = true; 
            if (interactionIcon != null)
            {
                interactionIcon.SetActive(false);
            }

            isInteractable = false;
            Player = null;

        }
    }

    
}