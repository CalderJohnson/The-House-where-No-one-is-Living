using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ObjectInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public int interactionLimit = -1; // Number of interactions allowed (-1 = unlimited)
    private int interactionCount = 0; // Tracks the number of times the object has been interacted with
    public bool isRepeatable = false; // Determines if the object can be interacted with multiple times
    public UnityEvent Object_Action; // Actions to invoke on interaction.

    [Header("Dialogue Settings")]
    public string dialogueFileName; // File name for dialogue, if applicable
    public GameObject DialogueCanvas; // Canvas to display dialogue UI

    [Header("Icon Settings")]
    public GameObject interactionIcon; // Icon that appears when object is interactable
    public bool enableBlinkEffect = true; // Determines if the interaction icon should blink when in range

    [Header("Interaction Direction")]
    public bool useDirectionalInteraction = true; // Toggle for directional interaction
    public Vector3 interactionDirection = Vector3.forward; // Default to front interaction.

    private bool isOpen = false; // Tracks if the object has already been interacted with

    private void Start()
    {
        // Hide interaction icon at the start
        if (interactionIcon != null)
        {
            interactionIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Called by the PlayerInteract script to perform the interaction.
    /// </summary>
    public IEnumerator Interact(Transform playerTransform)
    {
        Debug.Log($"Interact called on {gameObject.name}");

        // Check if the interaction limit has been reached (if set)
        if (interactionLimit > 0 && interactionCount >= interactionLimit)
        {
            Debug.Log($"{gameObject.name} has reached max interactions.");
            yield break;
        }

        // Skip directional check if useDirectionalInteraction is false
        if (useDirectionalInteraction && !IsPlayerFacingCorrectDirection(playerTransform))
        {
            Debug.Log("Player is not in the correct position to interact.");
            yield break;
        }

        Debug.Log("Starting interaction sequence...");
        interactionCount++; // Increment interaction counter

        // Handle interaction icon blink effect
        if (enableBlinkEffect)
        {
            yield return StartCoroutine(BlinkIcon());
        }
        else
        {
            interactionIcon?.SetActive(false);
        }

        // Handle dialogue if applicable
        if (Dialogue.Instance != null && !string.IsNullOrEmpty(dialogueFileName))
        {
            Dialogue.Instance.SetDialogueFileName(dialogueFileName);
            DialogueCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Dialogue instance not found in the scene!");
        }

        // Invoke interaction action(s)
        Object_Action.Invoke();
        Debug.Log($"Object Action Invoked! Interaction count: {interactionCount}");

        // If the interaction limit has been reached, disable further interactions
        if (interactionLimit > 0 && interactionCount >= interactionLimit)
        {
            interactionIcon?.SetActive(false); // Hide icon permanently after limit reached
        }
        else if (isRepeatable)
        {
            StartCoroutine(ResetAfterDelay(3f)); // This method is now defined
        }
    }

    /// <summary>
    /// Resets the object's interaction state after a delay.
    /// </summary>
    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetState();
    }

    /// <summary>
    /// Resets the interaction state, allowing the object to be interacted with again.
    /// </summary>
    public void ResetState()
    {
        Debug.Log($"{gameObject.name} has been reset. It is now interactable again.");
        isOpen = false;
        interactionCount = 0; // Reset interaction count
    }

    /// <summary>
    /// Handles the blinking effect for the interaction icon.
    /// </summary>
    private IEnumerator BlinkIcon()
    {
        if (interactionIcon == null) yield break;

        float blinkDuration = 0.5f, blinkInterval = 0.2f, elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            interactionIcon.SetActive(false);
            yield return new WaitForSeconds(blinkInterval * 0.5f);
            interactionIcon.SetActive(true);
            yield return new WaitForSeconds(blinkInterval * 0.5f);
            elapsed += blinkInterval;
        }

        interactionIcon.SetActive(false);
    }

    /// <summary>
    /// Checks if the player is facing the correct interaction direction.
    /// </summary>
    private bool IsPlayerFacingCorrectDirection(Transform playerTransform)
    {
        if (!useDirectionalInteraction) return true; // Skip direction check

        Vector3 toObject = (transform.position - playerTransform.position).normalized;
        Vector3 requiredDirection = transform.TransformDirection(interactionDirection).normalized;
        float dot = Vector3.Dot(requiredDirection, toObject);
        return dot > 0.5f;
    }

    /// <summary>
    /// Updates the icon’s visibility, transparency, and color based on the player's distance and position.
    /// </summary>
    public void UpdateIconVisibility(float distance, float interactionRange, Transform playerTransform)
    {
        // If interaction is limited and max interactions have been reached, disable icon
        if (interactionIcon == null || (interactionLimit > 0 && interactionCount >= interactionLimit))
            return;

        bool facingCorrectDirection = useDirectionalInteraction ? IsPlayerFacingCorrectDirection(playerTransform) : true;
        float alpha = Mathf.Clamp01(1 - (distance / interactionRange));
        bool isVisible = alpha > 0 && facingCorrectDirection;

        // Adjust icon color based on distance
        Color newColor = (distance <= 1.0f) ? Color.yellow : Color.white;
        newColor.a = alpha;

        if (interactionIcon.TryGetComponent(out SpriteRenderer sr))
        {
            sr.color = newColor;
        }
        else if (interactionIcon.TryGetComponent(out CanvasGroup cg))
        {
            cg.alpha = alpha;
        }

        interactionIcon.SetActive(isVisible);
    }
}
