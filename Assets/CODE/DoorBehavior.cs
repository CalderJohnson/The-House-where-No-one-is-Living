using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public Animator doorAnimator; // Assign the door's Animator in the Inspector
    public Transform teleportTarget; // Assign the target position for teleportation
    public DoorScript linkedDoor; // Reference to the other door

    private bool isDoorActive = true; // To prevent interaction after the door is triggered

    // This method is triggered by Object_Interact when the player interacts with the door
    public void TriggerDoor()
    {
        if (!isDoorActive) return; // Do nothing if the door is inactive
        StartCoroutine(OpenAndTeleport());
    }

    private IEnumerator OpenAndTeleport()
    {
        // Play the door opening animation
        doorAnimator.SetTrigger("Open");
        yield return new WaitForSeconds(1f); // Wait for the door animation to finish

        // Teleport the player to the linked door's position
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && linkedDoor != null)
        {
            player.transform.position = linkedDoor.teleportTarget.position;
        }

        // Close the door after teleportation
        linkedDoor.StartCoroutine(linkedDoor.CloseDoor());

        // Deactivate the current door to prevent immediate re-use
        isDoorActive = false;

        // Reactivate the door after a short delay
        yield return new WaitForSeconds(1f);
        isDoorActive = true;
    }

    private IEnumerator CloseDoor()
    {
        // Play the door closing animation
        doorAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(1f);
    }
}