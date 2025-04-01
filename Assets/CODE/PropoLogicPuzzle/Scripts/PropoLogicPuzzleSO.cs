using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPuzzle", menuName = "Propo Logic Puzzle/Puzzle")]
public class PuzzleSO : ScriptableObject
{
    [Header("Puzzle Facts")]
    public List<Fact> facts;
    
    [Header("Puzzle Given Rules (constructed from facts)")]
    public List<GivenRule> givenRules;
    
    [Header("Available Equivalence and Inference Rules")]
    public List<RuleSO> availableEquivalenceRules;
    public List<RuleSO> availableInferenceRules;
    
    [Header("Conclusion")]
    public Fact conclusion;
    
    [Header("Proof Settings")]
    [Tooltip("Maximum number of lines allowed in a proof")]
    public int maxProofLines = 10;
    
    [Tooltip("For proof by contradiction, set true (the negated conclusion is added to the facts)")]
    public bool useProofByContradiction;
}
