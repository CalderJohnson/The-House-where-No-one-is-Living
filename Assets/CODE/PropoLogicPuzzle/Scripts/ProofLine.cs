using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ProofLine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI lineNumberText;
    public TextMeshProUGUI englishText;
    public TextMeshProUGUI logicText;
    public Button modifyButton; // To trigger rule options.
    public string tooltipText = "";

    public int lineNumber;
    public Image backgroundImage; // Used for highlight effect
    private ProofController proofController;
    private Color defaultColor;
    private Color highlightColor = new Color(1f, 1f, 0.6f, 1f); // Light yellow

    private void Awake()
    {
        proofController = FindObjectOfType<ProofController>();
        if (backgroundImage != null)
            defaultColor = backgroundImage.color;
    }

    public void Initialize(int lineNum, string eng, string logic, string justification)
    {
        lineNumber = lineNum;
        tooltipText = justification;
        if (lineNumberText != null)
            lineNumberText.text = lineNum.ToString();
        if (englishText != null)
            englishText.text = eng;
        if (logicText != null)
            logicText.text = logic;

        if (modifyButton != null)
        {
            modifyButton.onClick.AddListener(() =>
            {
                if (proofController != null)
                    proofController.ShowRuleOptions(this);
            });
        }
    }

    public void UpdateLine(string newEnglish, string newLogic)
    {
        if (englishText != null)
            englishText.text = newEnglish;
        if (logicText != null)
            logicText.text = newLogic;
    }

    public void SetHighlight(bool highlight)
    {   
        if (backgroundImage != null)
        {   
            Color newColor = highlight ? highlightColor : defaultColor;
            newColor.a = 1f; // Ensure it's fully opaque.
            backgroundImage.color = newColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (proofController != null)
        {
            proofController.OnProofLineSelectedForCombine(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.ShowTooltip(tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}
