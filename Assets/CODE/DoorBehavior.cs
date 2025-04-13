using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    public Animator doorAnimator; // Assign the Animator for this door
    public DoorScript linkedDoor; // Reference to the other door's DoorScript
    private bool isDoorActive = true; // To prevent re-triggering while the door is in use
    public AudioSource doorAudioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip rattlingSound; // New sound for rattling
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    [Header("Item Requirement")]
    public string requiredItem; // Leave empty if no item is required

    [Header("Scene Loading")]
    public bool loadNewScene = false; // Set this to true if this door loads a new scene
    public string sceneName; // Name of the scene to load

    [Header("Decision System")]
    public bool affectsDecisionTree = false; // Set true if you want the door to change decision tree node
    public string decisionNodeID; // The name of the node to switch to

    [Header("One-Way Door")]
    public bool isOneWay = false; // Set true if this door should not be re-entered

    private PlayerInventory playerInventory;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<PlayerInventory>();
        }
    }

    public void TriggerDoor()
    {
        if (!isDoorActive) return; 

        if (isOneWay)
        {
            Debug.Log("This door is one-way and cannot be re-entered.");
            PlayRattlingSound(); 
            return; 
        }

        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance not found! Is Inventory attached to the Player?");
            return;
        }

        Debug.Log($"Required Item: {requiredItem}");

        // Check if the required item is in inventory
        if (string.IsNullOrEmpty(requiredItem) || Inventory.Instance.HasItem(requiredItem))
        {
            Debug.Log("Door unlocked! Opening...");

            // load the scene 
            if (loadNewScene && !string.IsNullOrEmpty(sceneName))
            {
                Debug.Log($"Loading scene: {sceneName}");
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                StartCoroutine(DoorSequence()); // Run normal teleport
            }
        }
        else
        {
            Debug.Log("You need " + requiredItem + " to open this door!");
            PlayRattlingSound(); // Play rattling sound
        }
    }

    private IEnumerator DoorSequence()
    {
        isDoorActive = false;

        // Open this door
        if (doorAnimator != null)
        {
            doorAnimator.Play("DoorOpen");
            doorAudioSource.clip = openSound;
            doorAudioSource.Play();
        }

        if (linkedDoor != null && linkedDoor.doorAnimator != null)
        {
            linkedDoor.doorAnimator.Play("DoorOpen");
        }

        yield return new WaitForSeconds(0.1f); // Wait for the linked door to open

        doorAudioSource.clip = closeSound;
        doorAudioSource.Play();

        yield return new WaitForSeconds(0.2f);

        // Move player to new position
        GameObject player = GameObject.FindWithTag("Player");
        player.SetActive(false);
        player.transform.position = new Vector3(x, y, z);
        player.SetActive(true);

        if (linkedDoor != null && linkedDoor.isOneWay)
        {
            linkedDoor.isOneWay = true; 
        }

        UpdateDecisionNode();

        yield return new WaitForSeconds(0.1f);

        if (doorAnimator != null)
        {
            doorAnimator.Play("DoorClose");
        }
        if (linkedDoor != null && linkedDoor.doorAnimator != null)
        {
            linkedDoor.doorAnimator.Play("DoorClose");
        }

        yield return new WaitForSeconds(1f); 

        isDoorActive = true; 
    }

    private void UpdateDecisionNode()
    {
        if (!affectsDecisionTree) return;

        bool success = DecisionManager.Instance.SetCurrentNode(decisionNodeID);

        if (success)
        {
            DataPersistenceManager.Instance.SaveGame();
            Debug.Log($"Decision node updated successfully to: {decisionNodeID}");
        }
        else
        {
            Debug.LogWarning($"Failed to update decision node to: {decisionNodeID}");
        }
    }

    // Play rattling sound
    private void PlayRattlingSound()
    {
        if (rattlingSound != null)
        {
            doorAudioSource.clip = rattlingSound;
            doorAudioSource.Play();
        }
    }
}
