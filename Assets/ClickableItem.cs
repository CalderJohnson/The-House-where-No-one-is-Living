using UnityEngine;
using UnityEngine.UI;

public class ClickableItem : MonoBehaviour
{
    public ProofController proofController; // Reference to the proof system
    public string factOrRuleText; // The text representing this fact or rule

    private void Awake()
    {
        // Ensure the object has a Button component
        Button button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        if (proofController != null)
        {
            proofController.AddProofLine(factOrRuleText, "Added via click");
            Debug.Log($"[CLICK] Added proof line: {factOrRuleText}");
        }
        else
        {
            Debug.LogWarning("[CLICK] ProofController is not assigned!");
        }
    }
}

