using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FactDisplay : MonoBehaviour
{
    public TextMeshProUGUI factIDText;
    public TextMeshProUGUI englishText;
    public Button addButton;
    private ProofController proofController; // Reference to the proof system

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

    public void Setup(Fact fact)
    {
        if (factIDText != null) {
            factIDText.text = fact.factID;
        }
        if (englishText != null) {
            LogicParser parser = new LogicParser();
            ExpressionNode expr = parser.Parse(fact.factID);
            englishText.text = EnglishTranslator.TranslateProperFactTextWithCache(expr);
        }
    }

    private void OnClicked()
    {
        string justification = "From given fact: " + factIDText.text + ".";
        if (proofController != null)
        {
            proofController.AddProofLine(englishText.text, factIDText.text, justification);
            Debug.Log($"[CLICK] Proof line added: {factIDText.text}");
        }
        else
        {
            Debug.LogWarning("[CLICK] ProofController is not active or assigned!");
        }
    }
}