using UnityEngine;
using System;
using System.Collections.Generic;

public class WardrobeDoors : MonoBehaviour, IDataPersistence
{
    [Header("Wardrobe Settings")]
    public string wardrobeID; // Unique identifier for each wardrobe
    public Animator leftDoorAnimator;
    public Animator rightDoorAnimator;
    public GameObject storedItem;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip openSound;

    [Header("Decision System Settings")]
    public bool updateOnItemCollection = false; // If true, update decision tree when item is collected

    private bool isOpen = false;
    private bool itemCollected = false; // Tracks if the item was taken

    private void Awake()
    {
        // Generate a unique ID if none is assigned (only for new wardrobes)
        if (string.IsNullOrEmpty(wardrobeID))
        {
            wardrobeID = Guid.NewGuid().ToString();
        }
    }

    public void OpenWardrobe()
    {
        if (!isOpen)
        {
            // Re-enable Animator before playing animations
            leftDoorAnimator.enabled = true;
            rightDoorAnimator.enabled = true;

            // Play animations for opening the doors
            leftDoorAnimator.Play("DoorOpen_Left");
            rightDoorAnimator.Play("DoorOpen_Right");

            // Play sound effect if available
            if (audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }

            isOpen = true;
            Debug.Log($"Wardrobe {wardrobeID} opened.");

            // Update decision tree if NOT waiting for item collection
            if (!updateOnItemCollection)
            {
                GetComponent<UpdateNode>()?.TryUpdateDecisionNode();
            }
        }
        else if (!itemCollected) // If the wardrobe is already open, allow collecting the item
        {
            RetrieveItem();
        }
    }

    private void RetrieveItem()
    {
        if (storedItem != null)
        {
            Inventory.Instance.AddItem(storedItem.name); // Add item to inventory
            itemCollected = true;
            storedItem.SetActive(false); // Hide item instead of destroying it
            Debug.Log($"Item collected from wardrobe {wardrobeID}!");

            // Update decision tree if waiting for item collection
            if (updateOnItemCollection)
            {
                GetComponent<UpdateNode>()?.TryUpdateDecisionNode();
            }
        }
    }

    public void LoadData(GameData data)
    {
        WardrobeState savedState = data.wardrobeStates.Find(w => w.wardrobeID == wardrobeID);
        if (savedState != null)
        {
            isOpen = savedState.isOpen;
        }
        else
        {
            isOpen = false; 
        }

        if (!isOpen)
        {
            leftDoorAnimator.enabled = false;
            rightDoorAnimator.enabled = false;

            leftDoorAnimator.transform.localRotation = Quaternion.Euler(0, 0, 0);
            rightDoorAnimator.transform.localRotation = Quaternion.Euler(0, 0, 0);

            Debug.Log($"Wardrobe {wardrobeID} reset to closed state.");
        }
        else
        {
            leftDoorAnimator.enabled = true;
            rightDoorAnimator.enabled = true;

            leftDoorAnimator.Play("DoorOpen_Left", 0, 1f);
            rightDoorAnimator.Play("DoorOpen_Right", 0, 1f);
        }

        // Load item collection status
        CollectedItem savedItem = data.collectedItems.Find(c => c.wardrobeID == wardrobeID);
        if (savedItem != null)
        {
            itemCollected = savedItem.itemCollected;
        }
        else
        {
            itemCollected = false;
        }

        if (!itemCollected && storedItem != null)
        {
            storedItem.SetActive(true); 
        }
        else if (itemCollected && storedItem != null)
        {
            storedItem.SetActive(false); 
        }
    }

    public void SaveData(ref GameData data)
    {
        // Ensure list exists
        if (data.wardrobeStates == null) data.wardrobeStates = new List<WardrobeState>();
        if (data.collectedItems == null) data.collectedItems = new List<CollectedItem>();

        // Save wardrobe state
        WardrobeState existingState = data.wardrobeStates.Find(w => w.wardrobeID == wardrobeID);
        if (existingState != null)
        {
            existingState.isOpen = isOpen;
        }
        else
        {
            data.wardrobeStates.Add(new WardrobeState(wardrobeID, isOpen));
        }

        // Save collected item status
        CollectedItem existingItem = data.collectedItems.Find(c => c.wardrobeID == wardrobeID);
        if (existingItem != null)
        {
            existingItem.itemCollected = itemCollected;
        }
        else
        {
            data.collectedItems.Add(new CollectedItem(wardrobeID, itemCollected));
        }
    }
}
