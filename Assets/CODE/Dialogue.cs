using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

[System.Serializable]
public class Dialogue : MonoBehaviour
{
    public string charaName;

    [TextArea(3, 10)]
    public string[] lines;
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI nameComponent;
    public RectTransform textComponentRect; // Reference to the text box's RectTransform
    public RectTransform nameComponentRect; // Reference to the name box's RectTransform
    public float textSpeed;
    private int index;

    private Vector2 originalTextSize; // Store default text box size

    // File path for external dialogue file
    public string dialogueFileName; // Set this in Unity Inspector

    // Start is called before the first frame update
    void Start()
    {
        textComponentRect = textComponent.GetComponent<RectTransform>();

        // Store original sizes
        originalTextSize = textComponentRect.sizeDelta;

        //Construct full file path
        string fullFilePath = $"Assets/Resources/Dialogue/{dialogueFileName}.txt";
        
        if (!string.IsNullOrEmpty(dialogueFileName) && File.Exists(fullFilePath))
        {
            LoadLinesFromFile(fullFilePath);
        }

        textComponent.text = string.Empty;
        nameComponent.text = charaName;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        DisplayCurrentLine();
    }

    IEnumerator TypeLine()
    {
        // Type each character 1 by 1
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void DisplayCurrentLine()
    {
        // Reset textComponent size to default
        textComponentRect.sizeDelta = originalTextSize; 

        string[] splitLine = lines[index].Split(new[] { ": " }, 2, System.StringSplitOptions.None);

        if (splitLine.Length == 2)
        {
            // Line contains "Chara Name: Dialogue"
            charaName = splitLine[0];
            nameComponent.text = charaName;
            nameComponent.gameObject.SetActive(true);

            // Set textComponent to only display the dialogue part
            lines[index] = splitLine[1];
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());

            // Ensure textComponent retains original top value
            textComponentRect.offsetMax = new Vector2(textComponentRect.offsetMax.x, -60);
        }
        else
        {
            // Line does not contain a name
            nameComponent.text = string.Empty;
            nameComponent.gameObject.SetActive(false);

            // Adjust textComponent's top value to occupy nameComponent's space
            textComponentRect.offsetMax = new Vector2(textComponentRect.offsetMax.x, 0);

            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            DisplayCurrentLine();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void LoadLinesFromFile(string filePath)
    {
        try
        {
            // Read all lines from the file
            string[] fileLines = File.ReadAllLines(filePath);

            // Initialize lines array
            lines = new string[fileLines.Length];

            for (int i = 0; i < fileLines.Length; i++)
            {
                // Ensure each line follows the "Chara Name: Dialogue" format
                lines[i] = fileLines[i];
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"Error reading dialogue file: {e.Message}");
        }
    }
}