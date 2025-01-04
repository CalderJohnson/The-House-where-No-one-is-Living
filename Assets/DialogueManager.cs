using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> lines;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    // Start is called before the first frame update
    void Start()
    {
        lines = new Queue<string>();
    }

    public void StartDialogue (Dialogue dialogue)
    {
        Debug.Log("Starting conversation with " + dialogue.name);

        nameText.text = dialogue.nameComponent.text;

        lines.Clear();

        foreach (string sentence in dialogue.lines)
        {
            lines.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence ()
    {
                
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = lines.Dequeue();
        Debug.Log(sentence);
        dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        Debug.Log("End of conversation.");
    }

}