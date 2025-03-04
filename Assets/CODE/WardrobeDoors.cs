using UnityEngine;
using System;

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
            leftDoorAnimator.Play("DoorOpen_Left");
            rightDoorAnimator.Play("DoorOpen_Right");

            if (audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }

            isOpen = true;
            Debug.Log("Wardrobe opened.");
        }
        else if (!itemCollected) // Only retrieve if item hasn't been taken
        {
            RetrieveItem();
        }
    }

    private void RetrieveItem()
    {
        if (storedItem != null)
        {
            Inventory.Instance.AddItem(storedItem.name);
            itemCollected = true;  // Mark as collected
            Destroy(storedItem);    // Remove from scene
            storedItem = null;
            Debug.Log($"Item collected from wardrobe {wardrobeID}!");
        }
    }

    // Load & Save System Integration
    public void LoadData(GameData data)
    {
        if (data.collectedItems.Contains(wardrobeID))
        {
            itemCollected = true;
            if (storedItem != null)
            {
                Destroy(storedItem); // Ensure item doesn't appear if already collected
                storedItem = null;
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        if (itemCollected)
        {
            data.collectedItems.Add(wardrobeID);
        }
    }
}
