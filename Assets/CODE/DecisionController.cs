using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DecisionController : MonoBehaviour
{
    public GameObject[] decisions;          // unused right now, but kept for future
    public Image[] highlights;             // the Image component you want to blink
    public TextMeshProUGUI[] decisionTexts;
    private int currentIndex = 0;
    private bool isChoosing = false;

    public delegate void OnDecisionSelected(int decisionIndex);
    public OnDecisionSelected decisionCallback;

    private Coroutine blinkCoroutine;

    void Start()
    {
        Hide();
    }

    void Update()
    {
        if (!isChoosing) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            SwitchSelection(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            SwitchSelection(1);
        else if (Input.GetKeyDown(KeyCode.Space))
            ConfirmDecision();
    }

    public void Show(string[] options, OnDecisionSelected callback)
    {
        decisionCallback = callback;
        isChoosing = true;
        gameObject.SetActive(true);

        // Set texts and only enable the first highlight
        for (int i = 0; i < decisionTexts.Length; i++)
        {
            decisionTexts[i].text = options[i];
            highlights[i].enabled = (i == currentIndex);
        }

        blinkCoroutine = StartCoroutine(BlinkHighlight());
    }

    public void Hide()
    {
        isChoosing = false;
        gameObject.SetActive(false);

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        // disable all highlights
        foreach (var h in highlights)
            h.enabled = false;
    }

    void SwitchSelection(int dir)
    {
        // Debug.Log($"Switching selection from {currentIndex} with dir {dir}");

        // turn off the old
        highlights[currentIndex].enabled = false;

        // move index
        currentIndex = (currentIndex + dir + decisionTexts.Length) % decisionTexts.Length;

        // turn on the new
        highlights[currentIndex].enabled = true;

        // Debug.Log("Now Selected Index: " + currentIndex);
    }

    void ConfirmDecision()
    {
        Hide();
        decisionCallback?.Invoke(currentIndex);
        // Debug.Log($"Decision made! Index: {currentIndex}");
    }

    IEnumerator BlinkHighlight()
    {
        while (isChoosing)
        {
            for (int i = 0; i < highlights.Length; i++)
            {
                highlights[i].enabled = (i == currentIndex) ? !highlights[i].enabled : false;
            }
            yield return new WaitForSeconds(0.4f);
        }
    }
}