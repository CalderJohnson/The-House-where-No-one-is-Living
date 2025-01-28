using System.Collections;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator doorAnimator; // Assign the Animator for this door
    public DoorScript linkedDoor; // Reference to the other door's DoorScript
    public Transform teleportTarget; // The location where the player should be teleported
    private bool isDoorActive = true; // To prevent re-triggering while the door is in use

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
        }
        yield return new WaitForSeconds(1f); // Wait for the door to open

        // Open the linked door
        if (linkedDoor != null && linkedDoor.doorAnimator != null)
        {
            linkedDoor.doorAnimator.Play("DoorOpen");
        }
        yield return new WaitForSeconds(1f); // Wait for the linked door to open

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && teleportTarget != null)
        {
            player.transform.position = linkedDoor.teleportTarget.position;
        }


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
