using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectInteract : MonoBehaviour, IDataPersistence
{
    [Header("Interaction Settings")]
    public int interactionLimit = -1;
    private int interactionCount = 0;
    public bool isRepeatable = false;
    public UnityEvent Object_Action;

    [Header("Dialogue Settings")]
    public string dialogueFileName;
    public GameObject DialogueCanvas;

    [Header("Icon Settings")]
    public GameObject interactionIcon;
    public bool enableBlinkEffect = true;

    [Header("Interaction Direction")]
    public bool useDirectionalInteraction = false;
    public Vector3 interactionDirection = Vector3.forward;

    [Header("Save System")]
    public string objectID; // Unique ID for saving interaction state

    private void Awake()
    {
        // If the object already has an ID (set in Inspector), do NOT change it.
        if (string.IsNullOrEmpty(objectID))
        {
            objectID = GenerateStableID();
        }
    }

    private void Start()
    {
        if (interactionIcon != null)
        {
            interactionIcon.SetActive(false);
        }
    }


    private bool isInteracting = false;

    public IEnumerator Interact(Transform playerTransform)
    {
        if (isInteracting) yield break; // Ignore input if already interacting

        isInteracting = true; // Mark as interacting
        Debug.Log($"[ObjectInteract] Interact() called on {gameObject.name}");

        if (interactionLimit > 0 && interactionCount >= interactionLimit)
        {
            yield break;
        }

        if (useDirectionalInteraction && !IsPlayerFacingCorrectDirection(playerTransform))
        {
            yield break; // Prevent interaction if facing the wrong way
        }

        interactionCount++;

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

        Object_Action.Invoke();

        yield return new WaitForSeconds(0.2f); 

        isInteracting = false; 

        if (interactionLimit > 0 && interactionCount >= interactionLimit)
        {
            interactionIcon?.SetActive(false);
        }
        else if (isRepeatable)
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
        interactionCount = 0;
    }

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

    public bool IsPlayerFacingCorrectDirection(Transform playerTransform)
    {
        if (!useDirectionalInteraction) return true; // Skip direction check

        if (interactionDirection == Vector3.zero) return true; // Prevents invalid vector math


        Vector3 toObject = (transform.position - playerTransform.position).normalized;
        Vector3 requiredDirection = transform.TransformDirection(interactionDirection).normalized;
        return Vector3.Dot(requiredDirection, toObject) > 0.5f;
    }

    public void UpdateIconVisibility(float distance, float interactionRange, Transform playerTransform)
    {
        if (interactionIcon == null || (interactionLimit > 0 && interactionCount >= interactionLimit))
            return;

        bool facingCorrectDirection = useDirectionalInteraction ? IsPlayerFacingCorrectDirection(playerTransform) : true;
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

    public void LoadData(GameData data)
    {
        foreach (var objState in data.objectInteractions)
        {
            if (objState.objectID == objectID)
            {
                interactionCount = objState.interactionCount;

                if (interactionLimit > 0 && interactionCount >= interactionLimit)
                {
                    interactionIcon?.SetActive(false);
                }

                return;
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        bool found = false;

        for (int i = 0; i < data.objectInteractions.Count; i++)
        {
            if (data.objectInteractions[i].objectID == objectID)
            {
                data.objectInteractions[i].interactionCount = interactionCount;
                found = true;
                break;
            }
        }

        if (!found)
        {
            data.objectInteractions.Add(new ObjectInteractionState(objectID, interactionCount));
        }
    }

    /// <summary>
    /// Generates a stable unique ID based on the object's scene and name.
    /// </summary>
    private string GenerateStableID()
    {
        return gameObject.scene.name + "_" + gameObject.name + "_" + transform.position.ToString();
    }
}
