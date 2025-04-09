using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class RuleOptionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI ruleNameText;
    private static TextMeshProUGUI ruleOptionPreviewDisplay; // Cached reference

    private RuleOption option;
    private ProofLine associatedProofLine;
    private ProofController controller;

    private void Awake()
    {
        if (ruleOptionPreviewDisplay == null)
        {
            // Move up two levels in the hierarchy
            Transform grandparentTransform = transform.parent?.parent;
            if (grandparentTransform != null)
            {
                Transform previewTransform = grandparentTransform.Find("RuleOptionPreviewText");
                if (previewTransform != null)
                {
                    ruleOptionPreviewDisplay = previewTransform.GetComponent<TextMeshProUGUI>();
                    Debug.Log("[RuleOptionButton] Found RuleOptionPreviewText in parent's parent.");
                }
            }

            if (ruleOptionPreviewDisplay == null)
            {
                Debug.LogError("[RuleOptionButton] Could not find RuleOptionPreviewText in parent's parent!");
            }
        }
    }

    public void Setup(RuleOption option, ProofLine line, ProofController controller)
    {
        Debug.Log($"[RuleOptionButton] Setup called for rule: {option.ruleName}");

        this.option = option;
        associatedProofLine = line;
        this.controller = controller;

        if (ruleNameText != null)
        {
            ruleNameText.text = option.resultingLogic;
            Debug.Log($"[RuleOptionButton] Rule name set to: {ruleNameText.text}");
        }
        else
        {
            Debug.LogWarning("[RuleOptionButton] ruleNameText is NULL!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ruleOptionPreviewDisplay != null)
        {
            ruleOptionPreviewDisplay.text = !string.IsNullOrEmpty(option.englishPreview)
                ? option.englishPreview
                : $"{ruleNameText.text}'s English equivalent will go here";

            Debug.Log($"[RuleOptionButton] Preview updated on hover: {ruleOptionPreviewDisplay.text}");
        }
        else
        {
            Debug.LogWarning("[RuleOptionButton] ruleOptionPreviewDisplay is NULL!");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ruleOptionPreviewDisplay != null)
        {
            ruleOptionPreviewDisplay.text = ""; // Clear preview text
            Debug.Log("[RuleOptionButton] Preview cleared on pointer exit.");
        }
    }

    public void OnOptionSelected()
    {
        Debug.Log($"[RuleOptionButton] Option selected: {option.ruleName}");

        if (controller == null)
        {
            Debug.LogError("[RuleOptionButton] Controller is NULL! Cannot apply rule.");
            return;
        }

        if (associatedProofLine == null)
        {
            Debug.LogError("[RuleOptionButton] Associated proof line is NULL! Cannot apply rule.");
            return;
        }

        controller.ApplyRuleOption(associatedProofLine, option);
        Debug.Log($"[RuleOptionButton] Rule applied successfully: {option.ruleName}");
    }
}