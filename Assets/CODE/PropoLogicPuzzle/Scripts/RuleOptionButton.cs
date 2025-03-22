using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuleOptionButton : MonoBehaviour
{
    public TextMeshProUGUI ruleNameText;
    public TextMeshProUGUI previewText;
    
    private RuleOption option;
    private ProofLine associatedProofLine;
    private ProofController controller;
    
    public void Setup(RuleOption option, ProofLine line, ProofController controller)
    {
        this.option = option;
        associatedProofLine = line;
        this.controller = controller;
        if(ruleNameText != null)
            ruleNameText.text = option.ruleName;
        if(previewText != null)
            previewText.text = option.englishPreview;
    }
    
    // Hook this to the Button’s OnClick in the prefab.
    public void OnOptionSelected()
    {
        controller.ApplyRuleOption(associatedProofLine, option);
    }
}
