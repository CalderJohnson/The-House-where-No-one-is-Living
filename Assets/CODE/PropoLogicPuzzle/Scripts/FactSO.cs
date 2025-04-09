using UnityEngine;

[CreateAssetMenu(fileName = "NewFact", menuName = "Propo Logic Puzzle/Fact")]
public class Fact : ScriptableObject
{
    [Tooltip("Unique identifier for this fact (e.g., T, F, A, etc.)")]
    public string factID;
    
    [TextArea]
    [Tooltip("The English sentence (without negation)")]
    public string englishSentence;
    
    [TextArea]
    [Tooltip("The English sentence when negated. For example, insert 'not' at the proper index.")]
    public string negatedEnglishSentence;
}
