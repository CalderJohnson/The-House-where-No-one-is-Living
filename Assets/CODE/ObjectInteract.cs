using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ObjectInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public bool isRepeatable = false;
    public UnityEvent Object_Action; // Action(s) to invoke on interaction.

    [Header("Dialogue Settings")]
    // Instead of having a local Dialogue reference, we'll use a global DialogueManager.
    public string dialogueFileName;   // Dialogue file name for this object.
    public GameObject DialogueCanvas;

    [Header("Icon Settings")]
    public GameObject interactionIcon; // The icon to indicate interaction availability.
    public bool enableBlinkEffect = true; // Toggle for enabling/disabling the blink effect.

    private bool isOpen = false;

    private void Start()
    {
        // Ensure the icon is initially hidden.
        if (interactionIcon != null)
        {
            interactionIcon.SetActive(false);
        }
    }

    /// <summary>
    /// Called by the PlayerInteract script to perform the interaction.
    /// </summary>
    public IEnumerator Interact()
    {
        if (isOpen)
            yield break; // Already interacted with; do nothing.

        // Blink the interaction icon before hiding it, if enabled.
        if (enableBlinkEffect)
        {
            yield return StartCoroutine(BlinkIcon());
        }
        else if (interactionIcon != null)
        {
            interactionIcon.SetActive(false);
        }

        if (Dialogue.Instance != null && !string.IsNullOrEmpty(dialogueFileName))
        {
            Dialogue.Instance.SetDialogueFileName(dialogueFileName);

            // Activate the dialogue UI elements if they exist
            DialogueCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Dialogue instance not found in the scene!");
        }

        // Invoke any object-specific actions.
        Object_Action.Invoke();

        // Mark the object as having been interacted with.
        isOpen = true;

        // Hide the interaction icon permanently if it's not repeatable.
        if (!isRepeatable && interactionIcon != null)
        {
            interactionIcon.SetActive(false);
        }

        // If the interaction is repeatable, reset state after a delay.
        if (isRepeatable)
        {
            yield return new WaitForSeconds(3f);
            ResetState();
        }
    }

    /// <summary>
    /// Resets the object's interaction state.
    /// </summary>
    public void ResetState()
    {
        isOpen = false;
    }

    /// <summary>
    /// Blinks the interaction icon before it is hidden.
    /// </summary>
    IEnumerator BlinkIcon()
    {
        if (interactionIcon == null)
            yield break;

        float blinkDuration = 0.5f;   // Total duration of blinking.
        float blinkInterval = 0.2f;   // Interval between toggles.
        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            // Toggle off.
            interactionIcon.SetActive(false);
            yield return new WaitForSeconds(blinkInterval * 0.5f);
            // Toggle on.
            interactionIcon.SetActive(true);
            yield return new WaitForSeconds(blinkInterval * 0.5f);
            elapsed += blinkInterval;
        }

        interactionIcon.SetActive(false);
    }

    /// <summary>
    /// Updates the icon’s visibility, transparency, and color based on the player's distance and direction.
    /// Call this method from your PlayerInteract script every frame.
    /// </summary>
    /// <param name="distance">Current distance from the player.</param>
    /// <param name="interactionRange">The maximum distance at which interaction is possible.</param>
    /// <param name="playerTransform">The transform of the player.</param>
    public void UpdateIconVisibility(float distance, float interactionRange, Transform playerTransform)
    {
        if (interactionIcon == null)
            return;

        // If the object is not repeatable and has already been interacted with, hide the icon.
        if (!isRepeatable && isOpen)
        {
            interactionIcon.SetActive(false);
            return;
        }

        // Get direction from player to object
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);
        bool isFacingCorrectly = dotProduct > 0.8f; // Ensures the player is in front

        // Calculate transparency based on distance
        float alpha = Mathf.Clamp01(1 - (distance / interactionRange));

        // If the player is not facing correctly, hide the icon
        if (!isFacingCorrectly)
        {
            alpha = 0;
        }

        // Change color based on proximity
        Color newColor = (distance <= 1.0f ? Color.yellow : Color.white);
        newColor.a = alpha;

        // Update SpriteRenderer or CanvasGroup
        SpriteRenderer sr = interactionIcon.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = newColor;
        }
        else
        {
            CanvasGroup cg = interactionIcon.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = alpha;
            }
        }

        // Set active based on alpha
        interactionIcon.SetActive(alpha > 0);
    }
}