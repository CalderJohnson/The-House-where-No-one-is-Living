using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        tooltipPanel.SetActive(false);
    }
    
    public void ShowTooltip(string text)
    {
        tooltipText.text = text;
        tooltipPanel.SetActive(true);
    }
    
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
