using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ObjectInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public bool isRepeatable = false;
    public UnityEvent Object_Action; // Actions to invoke on interaction.

    [Header("Dialogue Settings")]
    public string dialogueFileName;
    public GameObject DialogueCanvas;

    [Header("Icon Settings")]
    public GameObject interactionIcon;
    public bool enableBlinkEffect = true;

    [Header("Interaction Direction")]
    public Vector3 interactionDirection = Vector3.forward; // Default to front interaction.

    private bool isOpen = false;

    private void Start()
    {
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

        if (isOpen)
        {
            Debug.Log("Object already interacted with.");
            yield break;
        }

        if (!IsPlayerFacingCorrectDirection(playerTransform))
        {
            Debug.Log("Player is not in the correct position to interact.");
            yield break;
        }

        Debug.Log("Starting interaction sequence...");

        if (enableBlinkEffect)
        {
            yield return StartCoroutine(BlinkIcon());
        }
        else
        {
            interactionIcon?.SetActive(false);
        }

        if (Dialogue.Instance != null && !string.IsNullOrEmpty(dialogueFileName))
        {
            Dialogue.Instance.SetDialogueFileName(dialogueFileName);
            DialogueCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Dialogue instance not found in the scene!");
        }

        Object_Action.Invoke();
        Debug.Log("Object Action Invoked!");

        isOpen = true;

        if (!isRepeatable)
        {
            interactionIcon?.SetActive(false);
        }
        else
        {
            StartCoroutine(ResetAfterDelay(3f));
        }
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetState();
    }

    public void ResetState()
    {
        Debug.Log($"{gameObject.name} has been reset. It is now interactable again.");
        isOpen = false;
    }

    private IEnumerator BlinkIcon()
    {
        if (interactionIcon == null) yield break;

        float blinkDuration = 0.5f;
        float blinkInterval = 0.2f;
        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            interactionIcon.SetActive(!interactionIcon.activeSelf);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        interactionIcon.SetActive(false);
    }

    /// <summary>
    /// Checks if the player is facing the correct interaction direction.
    /// </summary>
    private bool IsPlayerFacingCorrectDirection(Transform playerTransform)
    {
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
        if (interactionIcon == null || (!isRepeatable && isOpen))
            return;

        bool facingCorrectDirection = IsPlayerFacingCorrectDirection(playerTransform);
        float alpha = Mathf.Clamp01(1 - (distance / interactionRange));
        bool isVisible = alpha > 0 && facingCorrectDirection;

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