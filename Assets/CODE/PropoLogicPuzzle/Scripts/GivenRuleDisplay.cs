using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GivenRuleDisplay : MonoBehaviour
{
    public TextMeshProUGUI ruleLogicalText;
    public TextMeshProUGUI ruleEnglishText;
    
    public void Setup(GivenRule rule)
    {
        if(ruleLogicalText != null)
            ruleLogicalText.text = rule.logicalForm;
        if(ruleEnglishText != null)
            ruleEnglishText.text = rule.englishForm;
    }
}
