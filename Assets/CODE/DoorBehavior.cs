using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator doorAnimator; // Assign the Animator for this door
    public DoorScript linkedDoor; // Reference to the other door's DoorScript
    private bool isDoorActive = true; // To prevent re-triggering while the door is in use
    public AudioSource doorAudioSource;
    public AudioClip openSound;   // Door sounds
    public AudioClip closeSound;
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    // Function to trigger door behavior
    public void TriggerDoor()
    {
        if (!isDoorActive) return; // Prevent interaction if the door is already active

        StartCoroutine(DoorSequence());
    }

    private IEnumerator DoorSequence()
    {
        isDoorActive = false;

        // Open this door
        if (doorAnimator != null)
        {
            doorAnimator.Play("DoorOpen");         
            doorAudioSource.clip = openSound;       // Play open sound
            doorAudioSource.Play();
        }

        // Open the linked door
        if (linkedDoor != null && linkedDoor.doorAnimator != null)
        {
            linkedDoor.doorAnimator.Play("DoorOpen");
        }
        yield return new WaitForSeconds(0.1f); // Wait for the linked door to open

        doorAudioSource.clip = closeSound;         // Play close sound
        doorAudioSource.Play();

        yield return new WaitForSeconds(0.2f);

        GameObject player = GameObject.FindWithTag("Player");
        player.SetActive(false);
        player.transform.position = new Vector3(x, y, z);
        player.SetActive(true);

        yield return new WaitForSeconds(0.1f); 

        // Close both doors
        if (doorAnimator != null)
        {
            doorAnimator.Play("DoorClose");
        }
        if (linkedDoor != null && linkedDoor.doorAnimator != null)
        {
            
            linkedDoor.doorAnimator.Play("DoorClose");
            
        }
        yield return new WaitForSeconds(1f); // Wait for the doors to close

        isDoorActive = true; // Reactivate the door for future use
    }
}
