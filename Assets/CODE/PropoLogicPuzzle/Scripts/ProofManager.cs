using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProofController : MonoBehaviour
{
    [Header("Puzzle Data")]
    public PuzzleSO currentPuzzle; // Assign your PuzzleSO asset in the Inspector

    [Header("UI References")]
    public Transform givensPanel;       // Left panel: container for facts and given rules.
    public Transform proofPanel;        // Right panel: container for proof lines.
    public GameObject proofLinePrefab;  // Prefab for proof lines.
    public GameObject ruleOptionsPanel; // Pop-up panel for rule options.
    public TextMeshProUGUI thingToProveText;       // At top of proof page (displays the conclusion).
    public GameObject ruleOptionButtonPrefab; // Prefab for a rule option button.
    public float factYOffset = 10f; // Set in Inspector: vertical gap between Facts
    public float ruleYOffset = 15f; // Set in Inspector: vertical gap between Given Rules


    private List<ProofLine> proofLines = new List<ProofLine>();
    private Stack<ProofLine> proofHistory = new Stack<ProofLine>();

    void Start()
    {
        // Display the conclusion at the top.
        if (thingToProveText != null && currentPuzzle.conclusion != null)
            thingToProveText.text = currentPuzzle.conclusion.englishSentence;
        
        LoadGivens();
    }

    /// <summary>
    /// Loads all facts and given rules into the givens panel.
    /// </summary>
    private void LoadGivens()
    {
        // We'll use these as our base positions.
        Vector3 baseFactPos = Vector3.zero;
        Vector3 baseRulePos = Vector3.zero;
        bool baseFactSet = false;
        bool baseRuleSet = false;
        float currentFactY = 0f;
        float currentRuleY = 0f;
        
        // Instantiate fact UI items.
        foreach (var fact in currentPuzzle.facts)
        {
            GameObject factGO = Instantiate(Resources.Load<GameObject>("Prefabs/FactPrefab"), givensPanel);
            FactDisplay fd = factGO.GetComponent<FactDisplay>();
            if (fd != null)
                fd.Setup(fact);
            
            // If this is the first fact, store its default anchored position.
            RectTransform rt = factGO.GetComponent<RectTransform>();
            if (!baseFactSet)
            {
                baseFactPos = rt.anchoredPosition;
                currentFactY = baseFactPos.y;
                baseFactSet = true;
            }
            else
            {
                // For subsequent facts, decrement the y position.
                currentFactY -= factYOffset;
            }
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, currentFactY);
        }

        // Instantiate given rule UI items.
        foreach (var rule in currentPuzzle.givenRules)
        {
            GameObject ruleGO = Instantiate(Resources.Load<GameObject>("Prefabs/GivenRulePrefab"), givensPanel);
            GivenRuleDisplay grd = ruleGO.GetComponent<GivenRuleDisplay>();
            if (grd != null)
                grd.Setup(rule);
            
            // If this is the first rule, store its default anchored position.
            RectTransform rt = ruleGO.GetComponent<RectTransform>();
            if (!baseRuleSet)
            {
                baseRulePos = rt.anchoredPosition;
                currentRuleY = baseRulePos.y;
                baseRuleSet = true;
            }
            else
            {
                // For subsequent rules, decrement the y position.
                currentRuleY -= ruleYOffset;
            }
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, currentRuleY);
        }
    }

    /// <summary>
    /// Called by a drop handler when a fact or given rule is dropped into the proof area.
    /// </summary>
    public void AddProofLine(string englishText, string logicText)
    {
        if (proofLines.Count >= currentPuzzle.maxProofLines)
        {
            Debug.LogWarning("Maximum number of proof lines reached!");
            return;
        }
        GameObject newLineGO = Instantiate(proofLinePrefab, proofPanel);
        ProofLine newLine = newLineGO.GetComponent<ProofLine>();
        int lineNumber = proofLines.Count + 1;
        newLine.Initialize(lineNumber, englishText, logicText);
        proofLines.Add(newLine);
        proofHistory.Push(newLine);

        if (CheckProofValidity())
        {
            Debug.Log("Proof is valid!");
            // Here you can highlight the final line and enable the Submit button.
        }
    }

    /// <summary>
    /// Checks whether the current proof is valid.
    /// (For example: if the last line matches the conclusion’s factID or equals "false" for contradiction.)
    /// </summary>
    private bool CheckProofValidity()
    {
        if (proofLines.Count > 0)
        {
            string lastLogic = proofLines[proofLines.Count - 1].logicText.text.Trim();
            if (lastLogic == currentPuzzle.conclusion.factID)
                return true;
            if (lastLogic.ToLower() == "false")
                return true;
        }
        return false;
    }

    /// <summary>
    /// Called when the user clicks an Undo button.
    /// </summary>
    public void UndoLastProofLine()
    {
        if (proofHistory.Count > 0)
        {
            ProofLine lastLine = proofHistory.Pop();
            proofLines.Remove(lastLine);
            Destroy(lastLine.gameObject);
        }
    }

    /// <summary>
    /// Displays a pop-up with available rule options for the selected proof line.
    /// </summary>
    public void ShowRuleOptions(ProofLine selectedLine)
    {
        // Clear previous options.
        foreach (Transform child in ruleOptionsPanel.transform)
            Destroy(child.gameObject);
        
        List<RuleOption> validOptions = ComputeValidRuleOptions(selectedLine);
        foreach (var option in validOptions)
        {
            GameObject optionButtonGO = Instantiate(ruleOptionButtonPrefab, ruleOptionsPanel.transform);
            RuleOptionButton optionButton = optionButtonGO.GetComponent<RuleOptionButton>();
            if (optionButton != null)
                optionButton.Setup(option, selectedLine, this);
        }
        ruleOptionsPanel.SetActive(true);
    }

    private List<RuleOption> ComputeValidRuleOptions(ProofLine selectedLine)
    {
        List<RuleOption> options = new List<RuleOption>();
        // Parse the current logic string from the proof line.
        LogicParser parser = new LogicParser();
        ExpressionNode currentExpr;
        try
        {
            currentExpr = parser.Parse(selectedLine.logicText.text);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing expression: " + ex.Message);
            return options;
        }
        
        // Process all available rules (both equivalence and inference)
        foreach(var rule in currentPuzzle.availableEquivalenceRules)
        {
            if (!rule.enabledInPuzzle)
                continue;
            
            // Try forward transformation
            ExpressionNode transformedExpr = RuleEngine.ApplyRule(currentExpr, rule.forwardPattern, rule.forwardReplacement);
            if (!RuleEngine.AreEqual(currentExpr, transformedExpr))
            {
                RuleOption option = new RuleOption
                {
                    ruleName = rule.ruleName + " (Fwd)",
                    englishPreview = rule.englishDescription + " (forward)",
                    resultingEnglish = selectedLine.englishText.text + " (via " + rule.ruleName + " forward)",
                    resultingLogic = transformedExpr.ToLogicString()
                };
                options.Add(option);
            }
            
            // For equivalence rules marked symmetric, try reverse transformation.
            if (rule.ruleType == RuleType.Equivalence && rule.symmetric)
            {
                // Reverse transformation: swap pattern and replacement.
                ExpressionNode reverseExpr = RuleEngine.ApplyRule(currentExpr, rule.forwardReplacement, rule.forwardPattern);
                if (!RuleEngine.AreEqual(currentExpr, reverseExpr))
                {
                    RuleOption reverseOption = new RuleOption
                    {
                        ruleName = rule.ruleName + " (Rev)",
                        englishPreview = rule.englishDescription + " (reverse)",
                        resultingEnglish = selectedLine.englishText.text + " (via " + rule.ruleName + " reverse)",
                        resultingLogic = reverseExpr.ToLogicString()
                    };
                    options.Add(reverseOption);
                }
            }
        }
        
        // Process inference rules similarly (if any are available in PuzzleSO.availableInferenceRules)
        foreach(var rule in currentPuzzle.availableInferenceRules)
        {
            if (!rule.enabledInPuzzle)
                continue;
            
            // Inference rules are typically one–direction.
            ExpressionNode transformedExpr = RuleEngine.ApplyRule(currentExpr, rule.forwardPattern, rule.forwardReplacement);
            if (!RuleEngine.AreEqual(currentExpr, transformedExpr))
            {
                RuleOption option = new RuleOption
                {
                    ruleName = rule.ruleName,
                    englishPreview = rule.englishDescription,
                    resultingEnglish = selectedLine.englishText.text + " (via " + rule.ruleName + ")",
                    resultingLogic = transformedExpr.ToLogicString()
                };
                options.Add(option);
            }
        }
        
        return options;
    }


    /// <summary>
    /// Called when a rule option is selected from the pop-up.
    /// </summary>
    public void ApplyRuleOption(ProofLine selectedLine, RuleOption option)
    {
        // For rules like commutative or associative, you might overwrite the same line.
        // For inference rules, you might add a new line.
        // Here we simply update the existing line.
        selectedLine.UpdateLine(option.resultingEnglish, option.resultingLogic);
        ruleOptionsPanel.SetActive(false);
    }
}
