using System;
using UnityEngine;

[Serializable]
public class GivenRule
{
    [Tooltip("The propositional logic form (e.g., H ^ I -> ~A)")]
    public string logicalForm;
    
    [TextArea]
    [Tooltip("The English version (e.g., 'If I've been here alone and he did not come from inside the forest, then he did not just arrive in the forest.')")]
    public string englishForm;
}
