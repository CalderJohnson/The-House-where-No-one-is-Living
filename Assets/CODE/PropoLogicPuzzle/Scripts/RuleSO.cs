using UnityEngine;

public enum RuleType { Equivalence, Inference }

[CreateAssetMenu(fileName = "NewRule", menuName = "Propo Logic Puzzle/Rule")]
public class RuleSO : ScriptableObject
{
    [Tooltip("Short name (e.g., E2, I3)")]
    public string ruleName;
    
    public RuleType ruleType;
    
    [Header("Logic Representations")]
    [Tooltip("The pattern for the forward transformation (e.g., for E2: 'a v ~a')")]
    public string forwardPattern;
    
    [Tooltip("The replacement for the forward transformation (e.g., for E2: 'true')")]
    public string forwardReplacement;

    [Tooltip("For equivalence rules only: check this if the rule is symmetric so it can also be applied in reverse.")]
    public bool symmetric;
    
    // For inference rules the transformation is only one–direction.
    
    [Header("English Description")]
    [Tooltip("A brief description (e.g., 'Negation Law (OR)')")]
    public string englishDescription;
    
    [Tooltip("Enable/disable this rule for the current puzzle")]
    public bool enabledInPuzzle = true;
}