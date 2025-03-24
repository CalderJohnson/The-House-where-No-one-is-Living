using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GivenRuleDisplay : MonoBehaviour
{
    public TextMeshProUGUI ruleLogicalText;
    public TextMeshProUGUI ruleEnglishText;
    public Button addButton;
    private ProofController proofController;
    private void Awake()
    {
        proofController = FindObjectOfType<ProofController>();
        if (proofController == null)
        {
            Debug.LogError("ProofController not found!");
        }

        // Ensure addButton is assigned
        if (addButton == null)
        {
            addButton = GetComponentInChildren<Button>(); // Find it in prefab
        }

        // Clear existing listeners to prevent duplicates
        addButton.onClick.RemoveAllListeners();
        addButton.onClick.AddListener(OnClicked);
    }
    
    public void Setup(GivenRule rule)
    {
        if(ruleLogicalText != null)
            ruleLogicalText.text = rule.logicalForm;
        if(ruleEnglishText != null)
            ruleEnglishText.text = rule.englishForm;
    }
    private void OnClicked()
    {
        if (proofController != null)
        {
            proofController.AddProofLine(ruleEnglishText.text, ruleLogicalText.text);
            Debug.Log($"[CLICK] Proof line added: {ruleLogicalText.text}");
        }
        else
        {
            Debug.LogWarning("[CLICK] ProofController is not active or assigned!");
        }
    }
}
