using UnityEngine;

public class WardrobeDoors : MonoBehaviour
{
    public Animator leftDoorAnimator;
    public Animator rightDoorAnimator;
    public GameObject storedItem;
    public AudioSource audioSource;  // Add AudioSource component
    public AudioClip openSound;      // Assign an opening sound

    private bool isOpen = false;

    public void OpenWardrobe()
    {
        if (!isOpen)
        {
            leftDoorAnimator.Play("DoorOpen_Left");
            rightDoorAnimator.Play("DoorOpen_Right");

            if (audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);  // Play opening sound
            }

            isOpen = true;
            Debug.Log("Wardrobe opened.");
        }
        else
        {
            RetrieveItem();
        }
    }

    private void RetrieveItem()
    {
        if (storedItem != null)
        {
            Inventory.Instance.AddItem(storedItem.name);
            Destroy(storedItem); // Remove from scene
            storedItem = null;
            Debug.Log("Item collected from wardrobe!");
        }
    }
}
