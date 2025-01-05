using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Object_Interact : MonoBehaviour
{
    bool isInteractable = false;
    Collider Player;
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    public GameObject interactionIcon;
    public Animator leftDoorAnimator; 
    public Animator rightDoorAnimator; 
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
            //Debug.Log("enter");
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
            //Debug.Log("exit");
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
            OpenWardrobe();

            isInteractable = false;
            Player = null;

        }
    }

    private void OpenWardrobe()
    {
        isOpen = true; // Mark the wardrobe as opened.

       
        leftDoorAnimator.Play("DoorOpen_Left");
        rightDoorAnimator.Play("DoorOpen_Right");

        // Hide the interaction icon.
        if (interactionIcon != null)
        {
            interactionIcon.SetActive(false);
        }
    }
}
