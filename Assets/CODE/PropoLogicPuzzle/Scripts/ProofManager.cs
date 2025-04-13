using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProofController : MonoBehaviour
{   
    public GameObject propoBook;

    [Header("Puzzle Data")]
    public PuzzleSO currentPuzzle; // Assign your PuzzleSO asset in the Inspector

    [Header("UI References")]
    public GameObject bookCover;  // Reference to the PropoLogicBook
    public GameObject puzzleContent;  // Reference to the PropoLogicPuzzle (the content part)
    public Transform givensPanel;       // Left panel: container for facts and given rules.
    public Transform proofPanel;        // Right panel: container for proof lines.
    public GameObject proofLinePrefab;  // Prefab for proof lines.
    public GameObject ruleOptionsPanel; // Pop-up panel for rule options.
    public GameObject SubmitArea; // Where the submission button to end the puzzle appears.
    public TextMeshProUGUI thingToProveText;       // At top of proof page (displays the conclusion).
    public GameObject ruleOptionButtonPrefab; // Prefab for a rule option button.
    public float factYOffset = 10f; // Set in Inspector: vertical gap between Facts
    public float ruleYOffset = 15f; // Set in Inspector: vertical gap between Given Rules
    public float proofLineYOffset = 10f; // Set in Inspector: vertical gap between Proof Lines

    private List<ProofLine> proofLines = new List<ProofLine>();
    private Stack<ProofLine> proofHistory = new Stack<ProofLine>();
    private bool isPaused = false;
    private bool combineMode = false;
    private ProofLine combineLine1 = null;
    private ProofLine combineLine2 = null;
    // Keep track of clones to delete
    private List<GameObject> factObjects = new List<GameObject>();
    private List<GameObject> ruleObjects = new List<GameObject>();
    private List<GameObject> ruleOptionObjects = new List<GameObject>();


    Animator Book;

    private void Awake(){
        Book = propoBook.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isPaused)
            {
                OpenPuzzle(currentPuzzle);
            }
            else
            {
                EndPuzzle();
            }
        }
    }


    public void OpenPuzzle(PuzzleSO puzzleToLoad)
    {
        // Set the current puzzle to be played.
        currentPuzzle = puzzleToLoad;
        
        // (Re)initialize any necessary translators or caches.
        EnglishTranslator.GetRawFactText = (varName) =>
        {
            // Check facts from the puzzle
            foreach (var fact in currentPuzzle.facts)
            {
                if (fact.factID == varName)
                    return fact.rawFactText;
            }

            // Check the conclusion if needed.
            if (currentPuzzle.conclusion != null && currentPuzzle.conclusion.factID == varName)
            {
                return currentPuzzle.conclusion.rawFactText;
            }
            return varName;
        };

        // Set up conclusion text, etc.
        if (thingToProveText != null && currentPuzzle.conclusion != null)
            thingToProveText.text = currentPuzzle.conclusion.englishSentence;
        
        // Load facts/given rules onto your UI panels.
        LoadGivens();

        // Activate the proof book UI and pause game time.
        propoBook.SetActive(true);
        puzzleContent.SetActive(false); // Don't want the options to display before the book

        Time.timeScale = 0f;
        isPaused = true;

        // Reset animation parameters
        Book.SetBool("Tab2Close", false);

        // Wait for the opening animation to finish before showing the puzzle content
        StartCoroutine(WaitForOpenAnimation());

        Debug.Log("Proof puzzle opened and paused.");
    }

    /// <summary>
    /// Loads all facts and given rules into the givens panel.
    /// </summary>
    private void LoadGivens()
    {
        factObjects.Clear();
        ruleObjects.Clear();
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
            
            factObjects.Add(factGO);
            
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
            
            ruleObjects.Add(ruleGO);
            
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
    public void AddProofLine(string englishText, string logicText, string justification)
    {
        if (proofLines.Count >= currentPuzzle.maxProofLines)
        {
            Debug.LogWarning("Maximum number of proof lines reached!");
            return;
        }

        // Check if an identical line (excluding line number) already exists
        foreach (ProofLine line in proofLines)
        {
            if (line.logicText.text == logicText)
            {
                Debug.LogWarning($"Duplicate proof line detected: {englishText} ({logicText})");
                return;
            }
        }
        
        GameObject newLineGO = Instantiate(proofLinePrefab, proofPanel);
        ProofLine newLine = newLineGO.GetComponent<ProofLine>();
        int lineNumber = proofLines.Count + 1;
        newLine.Initialize(lineNumber, englishText, logicText, justification);
        newLine.gameObject.SetActive(true); // Since the Prefab is set to false, it shows up as false w/out this
        proofLines.Add(newLine);
        proofHistory.Push(newLine);

        RectTransform rt = newLineGO.GetComponent<RectTransform>();

        float newY;
        if (proofLines.Count == 1)
        {
            // Get the initial y-position of the proofLinePrefab
            newY = proofLinePrefab.GetComponent<RectTransform>().anchoredPosition.y;
        }
        else
        {
            // Get the y-position of the last proofLine and adjust by offset
            newY = proofLines[proofLines.Count - 2].GetComponent<RectTransform>().anchoredPosition.y - proofLineYOffset;
        }
        
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, newY);

        ToggleSubmitArea(CheckProofValidity());
        
    }

    /// <summary>
    /// Checks whether the current proof is valid.
    /// (For example: if the last line matches the conclusion’s factID or equals "false" for contradiction.)
    /// </summary>
    private string CheckProofValidity()
    {
        if (proofLines.Count > 0)
        {
            string lastLogic = proofLines[proofLines.Count - 1].logicText.text.Trim();
            if (lastLogic == currentPuzzle.conclusion.factID)
                return "Proven by direct proof! I found it out!";
            if (lastLogic.ToLower() == "false")
                return "That doesn't make sense...proven by contradiction!";
        }
        return "No";
    }

    private void ToggleSubmitArea(string proofResult)
    {
        if (proofResult != "No")
        {
            // Find the child named "successText" and get its TMP component
            TextMeshProUGUI successText = SubmitArea.transform.Find("SuccessText")?.GetComponent<TextMeshProUGUI>();

            if (successText != null)
            {
                successText.text = proofResult; // Set the text
            }
            else
            {
                Debug.LogWarning("successText not found in SubmitArea!");
            }

            SubmitArea.SetActive(true); 
        }
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
        Debug.Log("Finished computing valid options. Count: " + validOptions.Count);
        
        if (validOptions.Count == 0) {
            TooltipManager.Instance.ChangeTooltipText("No modifications currently available. Try adding or modifying a different line.");
        } else {
            foreach (var option in validOptions)
            {
                GameObject optionButtonGO = Instantiate(ruleOptionButtonPrefab, ruleOptionsPanel.transform);
                RuleOptionButton optionButton = optionButtonGO.GetComponent<RuleOptionButton>();
                if (optionButton != null)
                {
                    // Pass the option and the first combined line (or both if needed) and controller reference.
                    optionButton.Setup(option, selectedLine, this);
                }
                ruleOptionObjects.Add(optionButtonGO);
            }
            // Optionally force a layout update to arrange the buttons.
            LayoutRebuilder.ForceRebuildLayoutImmediate(ruleOptionsPanel.GetComponent<RectTransform>());
            ruleOptionsPanel.SetActive(true);
        }
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
            Debug.Log("Sucessfully parsed: " + selectedLine.logicText.text);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing expression: " + ex.Message);
            return options;
        }
        
        // Process all available rules (both equivalence and inference)
        foreach(var rule in currentPuzzle.availableEquivalenceRules)
        {
            Debug.Log($"Applying equivalence rule: {rule.ruleName} (Forward)");
            
            // Try forward transformation
            ExpressionNode transformedExpr = RuleEngine.ApplyRule(currentExpr, rule.forwardPattern, rule.forwardReplacement);
            Debug.Log($"Resulting logic after forward rule: {transformedExpr.ToLogicString()}");
            
            if (!RuleEngine.AreEqual(currentExpr, transformedExpr))
            {
                RuleOption option = new RuleOption
                {
                    ruleName = rule.ruleName + " (Fwd)",
                    resultingLogic = transformedExpr.ToLogicString(),
                    resultingEnglish = EnglishTranslator.TranslateProperFactTextWithCache(transformedExpr),
                    justification = $"From Line {selectedLine.lineNumber}: {rule.englishDescription} [Forward]"
                };
                options.Add(option);
                Debug.Log($"Added forward rule option: {option.ruleName}, Logic: {option.resultingLogic}");
            }
            
            // For equivalence rules marked symmetric, try reverse transformation.
            if (rule.ruleType == RuleType.Equivalence && rule.symmetric)
            {
                Debug.Log($"Applying equivalence rule: {rule.ruleName} (Reverse)");
                
                // Reverse transformation: swap pattern and replacement.
                ExpressionNode reverseExpr = RuleEngine.ApplyRule(currentExpr, rule.forwardReplacement, rule.forwardPattern);
                Debug.Log($"Resulting logic after reverse rule: {reverseExpr.ToLogicString()}");
                
                if (!RuleEngine.AreEqual(currentExpr, reverseExpr))
                {
                    RuleOption reverseOption = new RuleOption
                    {
                        ruleName = rule.ruleName + " (Rev)",
                        resultingLogic = reverseExpr.ToLogicString(),
                        resultingEnglish = EnglishTranslator.TranslateProperFactTextWithCache(reverseExpr),
                        justification = $"From Line {selectedLine.lineNumber}: {rule.englishDescription} [Reverse]"
                    };
                    options.Add(reverseOption);
                    Debug.Log($"Added reverse rule option: {reverseOption.ruleName}, Logic: {reverseOption.resultingLogic}");
                }
            }
        }

        // p ^ q => p (I2/Simplification Rule) is the only inference rule that works on one line.
        // Find the I2 (Simplification) rule from the available inference rules.
        var i2Rule = currentPuzzle.availableInferenceRules.FirstOrDefault(r => r.ruleName == "I2");

        if (i2Rule != null) // Ensure the rule exists before using it
        {
            Debug.Log($"Applying inference rule: {i2Rule.ruleName}");

            // Inference rules are typically one-direction.
            ExpressionNode transformedExpr = RuleEngine.ApplyRule(currentExpr, i2Rule.forwardPattern, i2Rule.forwardReplacement);
            Debug.Log($"Resulting logic after inference rule: {transformedExpr.ToLogicString()}");

            if (!RuleEngine.AreEqual(currentExpr, transformedExpr))
            {
                RuleOption option = new RuleOption
                {
                    ruleName = i2Rule.ruleName,
                    resultingLogic = transformedExpr.ToLogicString(),
                    resultingEnglish = EnglishTranslator.TranslateProperFactTextWithCache(transformedExpr),
                    justification = $"From Line {selectedLine.lineNumber}: {i2Rule.englishDescription}"
                };
                options.Add(option);
                Debug.Log($"Added inference rule option: {option.ruleName}, Logic: {option.resultingLogic}");
            }
        }

        Debug.Log($"Total options generated: {options.Count}");
        
        return options;
    }

    // This method is called when the Combine Button is pressed.
    public void ToggleCombineMode()
    {   
        if (!combineMode) {
            combineMode = true;
            combineLine1 = null;
            combineLine2 = null;
            Debug.Log("Entered combine mode. Select two proof lines to combine.");
        } else {
            // Click button again to quickly undo.
            CleanupCombineMode();
        }
    }

    private void CleanupCombineMode()
    {
        Debug.Log("Cleaning up combine mode...");
        
        if (combineLine1 != null)
        {
            combineLine1.SetHighlight(false);
            combineLine1 = null;
        }

        if (combineLine2 != null)
        {
            combineLine2.SetHighlight(false);
            combineLine2 = null;
        }

        combineMode = false;
    }

    // This method should be called by a proof line when it is clicked.
    // (For example, each ProofLine could call: proofController.OnProofLineSelectedForCombine(this);)
    public void OnProofLineSelectedForCombine(ProofLine selectedLine) {
        if (!combineMode)
            return; // Not in combine mode—ignore or do normal processing.

        // If no first line selected, record and highlight it.
        if (combineLine1 == null)
        {
            combineLine1 = selectedLine;
            combineLine1.SetHighlight(true);
            Debug.Log("First proof line selected for combination: Line " + combineLine1.lineNumber);
        }
        else if (combineLine1 != null && combineLine2 == null && selectedLine != combineLine1)
        {
            // Record second selection and highlight it.
            combineLine2 = selectedLine;
            combineLine2.SetHighlight(true);
            Debug.Log("Second proof line selected for combination: Line " + combineLine2.lineNumber);

            // Now compute rule options for combining these two lines.
            List<RuleOption> options = ComputeValidCombinationOptions(combineLine1, combineLine2);
            if (options.Count > 0)
            {
                // Show options in the rule options panel.
                // (Clear out any existing options first.)
                foreach (Transform child in ruleOptionsPanel.transform)
                    Destroy(child.gameObject);

                foreach (var option in options)
                {
                    GameObject optionButtonGO = Instantiate(ruleOptionButtonPrefab, ruleOptionsPanel.transform);
                    RuleOptionButton optionButton = optionButtonGO.GetComponent<RuleOptionButton>();
                    if (optionButton != null)
                        optionButton.Setup(option, combineLine1, this);
                }
                ruleOptionsPanel.SetActive(true);
            }
            else
            {
                Debug.Log("No valid inference rule found for combining lines " + combineLine1.lineNumber + " and " + combineLine2.lineNumber);
                
                // Reset only if no options are available (otherwise, wait for user selection)
                CleanupCombineMode();
            }
        }
    }

    private bool TryDecomposeImplication(ExpressionNode expr, out ExpressionNode left, out ExpressionNode right)
    {
        left = null;
        right = null;
        
        // Check if expr is an implication (p => q)
        if (expr is OperatorNode opNode && opNode.op == OperatorType.Implies && opNode.operands.Count == 2)
        {
            left = opNode.operands[0]; // p
            right = opNode.operands[1]; // q
            return true;
        }
        
        return false;
    }

    private List<RuleOption> ComputeValidCombinationOptions(ProofLine line1, ProofLine line2)
    {
        List<RuleOption> options = new List<RuleOption>();
        LogicParser parser = new LogicParser();
        ExpressionNode expr1, expr2;

        try {
            expr1 = parser.Parse(line1.logicText.text);
            expr2 = parser.Parse(line2.logicText.text);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing proof lines for combination: " + ex.Message);
            return options;
        }

        foreach (var rule in currentPuzzle.availableInferenceRules)
        {
            Debug.Log($"Checking inference rule: {rule.ruleName}");

            if (!rule.forwardPattern.Contains(","))
                continue;

            string[] parts = rule.forwardPattern.Split(',');
            if (parts.Length != 2)
            {
                Debug.LogError($"Inference rule {rule.ruleName} has an invalid pattern format: {rule.forwardPattern}");
                continue;
            }
            string premise1Pattern = parts[0].Trim();
            string premise2Pattern = parts[1].Trim();

            bool matchFirst = TryMatchPremise(expr1, premise1Pattern) && TryMatchPremise(expr2, premise2Pattern);
            bool matchSecond = TryMatchPremise(expr2, premise1Pattern) && TryMatchPremise(expr1, premise2Pattern);

            if (matchFirst || matchSecond)
            {
                Debug.Log($"Matched rule {rule.ruleName} with expressions: {expr1.ToLogicString()} and {expr2.ToLogicString()}");

                // Instead of calling them extractedP and extractedQ, we name them candidateA and candidateB.
                ExpressionNode candidateA = null;
                ExpressionNode candidateB = null;

                if (matchFirst)
                {
                    candidateA = expr1;
                    candidateB = expr2;
                }
                else if (matchSecond)
                {
                    candidateA = expr2;
                    candidateB = expr1;
                }

                Debug.Log($"Candidate lines: A = {candidateA.ToLogicString()}, B = {candidateB.ToLogicString()}");

                // We'll define our specific extractions (p, q, r) only in context of a specific rule.
                string replacedLogic = rule.forwardReplacement;
                ExpressionNode p = null, q = null, r = null; // for rule-specific extractions

                if (rule.ruleName.StartsWith("I3")) // Modus Ponens: (p, p => q) => q
                {
                    // Here candidateA is p and candidateB should be an implication (p => q)
                    if (TryDecomposeImplication(candidateB, out ExpressionNode left, out ExpressionNode right) 
                        && left.ToLogicString() == candidateA.ToLogicString())
                    {
                        p = candidateA;
                        q = right; // extract the consequent only
                        Debug.Log($"Modus Ponens detected: {left.ToLogicString()} => {right.ToLogicString()}");
                        replacedLogic = q.ToLogicString();
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to apply Modus Ponens for rule {rule.ruleName}");
                        continue;
                    }
                }
                else if (rule.ruleName.StartsWith("I4")) // Modus Tollens: (~q, p => q) => ~p
                {
                    if (TryDecomposeImplication(candidateB, out ExpressionNode left, out ExpressionNode right)
                        && right.ToLogicString() == candidateA.ToLogicString())
                    {
                        p = left;
                        Debug.Log($"Modus Tollens detected: {left.ToLogicString()} => {right.ToLogicString()}");
                        replacedLogic = new OperatorNode(OperatorType.Not, p).ToLogicString();
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to apply Modus Tollens for rule {rule.ruleName}");
                        continue;
                    }
                }
                else if (rule.ruleName.StartsWith("I5")) // Hypothetical Syllogism: (p => q, q => r) => (p => r)
                {
                    if (TryDecomposeImplication(candidateA, out ExpressionNode left1, out ExpressionNode right1) &&
                        TryDecomposeImplication(candidateB, out ExpressionNode left2, out ExpressionNode right2) &&
                        right1.ToLogicString() == left2.ToLogicString())
                    {
                        p = left1;
                        r = right2;
                        Debug.Log($"Hypothetical Syllogism detected: ({left1.ToLogicString()} => {right1.ToLogicString()}) and ({left2.ToLogicString()} => {right2.ToLogicString()})");
                        replacedLogic = $"{p.ToLogicString()} => {r.ToLogicString()}";
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to apply Hypothetical Syllogism for rule {rule.ruleName}");
                        continue;
                    }
                }
                else
                {
                    // For rules that don't require internal extraction (e.g. I1, I6, etc.),
                    // we simply substitute placeholders with the candidate expressions.
                    replacedLogic = replacedLogic.Replace("p", candidateA?.ToLogicString())
                                                .Replace("q", candidateB?.ToLogicString() ?? "")
                                                .Replace("r", ""); // if not used, leave blank
                }

                Debug.Log($"Final replaced logic for rule {rule.ruleName}: {replacedLogic}");

                ExpressionNode resultExpr;
                try
                {
                    resultExpr = parser.Parse(replacedLogic);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error parsing replacement in rule {rule.ruleName}: {ex.Message}");
                    continue;
                }

                RuleOption option = new RuleOption
                {
                    ruleName = rule.ruleName,
                    resultingLogic = resultExpr.ToLogicString(),
                    resultingEnglish = EnglishTranslator.TranslateProperFactTextWithCache(resultExpr),
                    justification = $"From Lines {line1.lineNumber} and {line2.lineNumber}: {rule.englishDescription}"
                };

                Debug.Log($"Added new rule option: {option.ruleName} -> {option.resultingLogic}");
                options.Add(option);
            }
        }

        // **I1 (Addition Rule) Implementation**
        foreach (var rule in currentPuzzle.availableInferenceRules)
        {
            if (rule.ruleName.StartsWith("I1"))
            {
                // Addition rule: p => (p v q), where q is arbitrary.
                // For I1, we use the full lines as candidates.
                ExpressionNode disjunction = new OperatorNode(OperatorType.Or, expr1, expr2);

                RuleOption option = new RuleOption
                {
                    ruleName = rule.ruleName,
                    resultingLogic = disjunction.ToLogicString(),
                    resultingEnglish = EnglishTranslator.TranslateProperFactTextWithCache(disjunction),
                    justification = $"From Lines {line1.lineNumber} and {line2.lineNumber}: {rule.englishDescription}"
                };
                options.Add(option);
                Debug.Log($"Added addition rule option: {option.ruleName}, resulting logic: {option.resultingLogic}");
            }
        }

        return options;
    }

    // Helper method to try matching an expression against a pattern string.
    private bool TryMatchPremise(ExpressionNode expr, string patternStr)
    {
        try
        {
            ExpressionNode patternExpr = new LogicParser().Parse(patternStr);
            Dictionary<string, ExpressionNode> subs = new Dictionary<string, ExpressionNode>();
            return RuleEngine.TryMatch(expr, patternExpr, subs);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error matching premise: " + ex.Message);
            return false;
        }
    }

    // When a rule option is selected from the panel in combination mode,
    // add the resulting proof line with the provided justification.
    public void ApplyCombinationRuleOption(ProofLine selectedLine, RuleOption option)
    {
        Debug.Log("Applying Combination Rule...");
        // In combination, the justification includes both involved line numbers.
        AddProofLine(option.resultingEnglish, option.resultingLogic, option.justification);
        ruleOptionsPanel.SetActive(false);

        CleanupCombineMode();
    }

    /// <summary>
    /// Called when a rule option is selected from the pop-up.
    /// </summary>
    public void ApplyRuleOption(ProofLine proofLine, RuleOption option)
    {
        Debug.Log($"Applying rule: {option.ruleName} to line {proofLine.lineNumber}");

        if (option.ruleName.StartsWith("E9") || option.ruleName.StartsWith("E10")){
            // No need to waste a proof line on switching options around.
            proofLine.UpdateLine(option.resultingEnglish, option.resultingLogic);
            ToggleSubmitArea(CheckProofValidity());

        } else {
            // Create the new proof line based on the rule application
            AddProofLine(option.resultingEnglish, option.resultingLogic, option.justification);
        }

        // Hide rule options panel after a rule is selected
        ruleOptionsPanel.SetActive(false);

        if (combineMode) {
            CleanupCombineMode();
        }
    }

    private void DestroyAllClones()
    {
        // Destroy all fact objects
        foreach (GameObject factGO in factObjects)
        {
            Destroy(factGO);
        }
        factObjects.Clear();

        // Destroy all rule objects
        foreach (GameObject ruleGO in ruleObjects)
        {
            Destroy(ruleGO);
        }
        ruleObjects.Clear();

        // Destroy all proof line objects (if any)
        foreach (ProofLine Line in proofLines)
        {
            Destroy(Line);
        }
        proofLines.Clear();

        // Destroy all rule option objects (if any)
        foreach (GameObject ruleOptionGO in ruleOptionObjects)
        {
            Destroy(ruleOptionGO);
        }
        ruleOptionObjects.Clear();

        Debug.Log("All dynamically created objects have been destroyed.");
    }

    public void EndPuzzle()
    {   
        DestroyAllClones();
        puzzleContent.SetActive(false);
        // Enable the closing animation to be played
        Book.SetBool("Tab2Close", true);

        EnglishTranslator.ClearCache();

        // Resume time before waiting for animation
        Time.timeScale = 1f; 

        StartCoroutine(CloseBookAndExit());
    }

    private IEnumerator WaitForOpenAnimation()
    {
        // Wait until the "Open" animation has finished
        AnimatorStateInfo stateInfo = Book.GetCurrentAnimatorStateInfo(0);

        while (stateInfo.IsName("Book Open") || stateInfo.normalizedTime >= 1f)
        {
            yield return null;
            stateInfo = Book.GetCurrentAnimatorStateInfo(0);
        }

        // Once the book is open, show the puzzle content
        puzzleContent.SetActive(true);
    }

    private IEnumerator CloseBookAndExit()
    {  
        while (true)
        {
            AnimatorStateInfo currentState = Book.GetCurrentAnimatorStateInfo(0);

            if (currentState.IsName("Book Close") && currentState.normalizedTime >= 1f && !Book.IsInTransition(0))
            {
                break;
            }
            yield return null; // Wait for next frame
        }

        // Now disable the menu
        propoBook.SetActive(false);

        isPaused = false;
    }
}
