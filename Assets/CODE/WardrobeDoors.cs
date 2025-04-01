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
    [Tooltip("Check this if the wardrobe affects the decision tree.")]
    public bool affectsDecisionTree = false;

    [Tooltip("The decision node that will be updated when interacting.")]
    public string decisionNodeID = ""; // Node to set when interacted with

    [Tooltip("Check this to update the decision node when the item is collected instead of when the wardrobe is opened.")]
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

            // Update decision tree **if we are NOT waiting for item collection**
            if (affectsDecisionTree && !updateOnItemCollection)
            {
                UpdateDecisionNode();
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

            // Update decision tree **if waiting for item collection**
            if (affectsDecisionTree && updateOnItemCollection)
            {
                UpdateDecisionNode();
            }
        }
    }

    private void UpdateDecisionNode()
    {
        DataPersistenceManager.Instance.gameData.currentDecisionNode = decisionNodeID;
        DataPersistenceManager.Instance.SaveGame();
        Debug.Log($"Decision node updated to: {decisionNodeID}");
    }

    public void LoadData(GameData data)
    {
        // Load wardrobe open state
        WardrobeState savedState = data.wardrobeStates.Find(w => w.wardrobeID == wardrobeID);
        if (savedState != null)
        {
            isOpen = savedState.isOpen;
        }
        else
        {
            isOpen = false; // Default to closed if no save data is found
        }

        // If wardrobe should be closed, reset doors
        if (!isOpen)
        {
            // Disable Animator to reset doors manually
            leftDoorAnimator.enabled = false;
            rightDoorAnimator.enabled = false;

            // Reset door rotation (adjust values based on closed rotation)
            leftDoorAnimator.transform.localRotation = Quaternion.Euler(0, 0, 0);
            rightDoorAnimator.transform.localRotation = Quaternion.Euler(0, 0, 0);

            Debug.Log($"Wardrobe {wardrobeID} reset to closed state.");
        }
        else
        {
            // Ensure Animator is enabled so doors stay open if saved that way
            leftDoorAnimator.enabled = true;
            rightDoorAnimator.enabled = true;

            // Play open animation instantly if it was open in save
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
            itemCollected = false; // Default to not collected
        }

        // Restore the item if it was not collected in save
        if (!itemCollected && storedItem != null)
        {
            storedItem.SetActive(true); // Ensure item is visible again
        }
        else if (itemCollected && storedItem != null)
        {
            storedItem.SetActive(false); // Hide item if collected
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
