using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ProofLine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI lineNumberText;
    public TextMeshProUGUI  englishText;
    public TextMeshProUGUI  logicText;
    public Button modifyButton; // To trigger rule options.

    public int lineNumber;
    
    public void Initialize(int lineNum, string eng, string logic)
    {
        lineNumber = lineNum;
        if(lineNumberText != null)
            lineNumberText.text = lineNum.ToString();
        if(englishText != null)
            englishText.text = eng;
        if(logicText != null)
            logicText.text = logic;
        
        if(modifyButton != null)
        {
            modifyButton.onClick.AddListener(() =>
            {
                // Assume a singleton or find the controller in the scene.
                ProofController controller = FindObjectOfType<ProofController>();
                if(controller != null)
                    controller.ShowRuleOptions(this);
            });
        }
    }
    
    public void UpdateLine(string newEnglish, string newLogic)
    {
        if(englishText != null)
            englishText.text = newEnglish;
        if(logicText != null)
            logicText.text = newLogic;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.ShowTooltip("Proof line " + lineNumber + " details here...");
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}
