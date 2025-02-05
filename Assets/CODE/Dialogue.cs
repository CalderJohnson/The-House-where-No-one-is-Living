using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue : MonoBehaviour
{
    public string charaName;

    [TextArea(3, 10)]
    public string[] lines;
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI nameComponent;
    public float textSpeed;
    private int index;

    public Image dialogueSprite; // Sprite UI element for displaying character sprites
    private RectTransform textComponentRect; // RectTransform for resizing the text box
    private RectTransform nameComponentRect; // RectTransform for resizing the name box

    // File path for external dialogue file
    public string dialogueFileName; // Set this in Unity Inspector

    void Start()
    {
        textComponentRect = textComponent.GetComponent<RectTransform>();
        nameComponentRect = nameComponent.GetComponent<RectTransform>();

        if (!string.IsNullOrEmpty(dialogueFileName))
        {
            LoadDialogueFile();
        }

        textComponent.text = string.Empty;
        nameComponent.text = charaName;

        // Hide sprite by default
        dialogueSprite.gameObject.SetActive(false);

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
        if(dialogueSprite.gameObject.activeSelf){
            textComponentRect.offsetMin = new Vector2(110, textComponentRect.offsetMin.y);
            nameComponentRect.offsetMin = new Vector2(110, nameComponentRect.offsetMin.y);
            dialogueSprite.gameObject.SetActive(false);
        }

        string[] splitLine = lines[index].Split(new[] { ": " }, 2, System.StringSplitOptions.None);

        if (splitLine.Length == 2)
        {
            // Line contains "Chara Name: Dialogue"
            charaName = splitLine[0];
            nameComponent.text = charaName;
            nameComponent.gameObject.SetActive(true);

            // Set textComponent to only display the dialogue part
            lines[index] = splitLine[1];
            textComponentRect.offsetMax = new Vector2(textComponentRect.offsetMax.x, -60);
            
            System.Text.RegularExpressions.Regex spriteRegex = new System.Text.RegularExpressions.Regex(@"\{(\d+)\}$");
            var match = spriteRegex.Match(lines[index]);
            if (match.Success)
            {
                // Extract sprite index
                int spriteIndex = int.Parse(match.Groups[1].Value);

                // Load the sprite dynamically
                string spritePath = $"Sprites/DialogueSprites/{charaName}/{charaName}_{spriteIndex}";
                Sprite loadedSprite = Resources.Load<Sprite>(spritePath);

                if (loadedSprite != null)
                {
                    // Adjust the text box to account for the sprite
                    textComponentRect.offsetMin = new Vector2(500, textComponentRect.offsetMin.y);
                    nameComponentRect.offsetMin = new Vector2(500, nameComponentRect.offsetMin.y);

                    // Display the sprite
                    dialogueSprite.sprite = loadedSprite;
                    dialogueSprite.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"Sprite not found at path: {spritePath}");
                    ResetSpriteAndTextBox();
                }

                // Remove the sprite tag from the dialogue text
                lines[index] = spriteRegex.Replace(lines[index], string.Empty);
            }
            else
            {
                ResetSpriteAndTextBox();
            }
        }
        else
        {
            // Line does not contain a name
            nameComponent.text = string.Empty;
            nameComponent.gameObject.SetActive(false);

            if (textComponentRect.offsetMax.y == -60) {
                textComponentRect.offsetMax = new Vector2(textComponentRect.offsetMax.x, 0);
            }
        }
        
        textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
    
    }

    void AdjustForSpriteIfPresent(string currentLine)
    { 
        // Check for a sprite tag at the end of the line
        System.Text.RegularExpressions.Regex spriteRegex = new System.Text.RegularExpressions.Regex(@"\{(\d+)\}$");
        var match = spriteRegex.Match(currentLine);

        if (match.Success)
        {
            // Extract sprite index
            int spriteIndex = int.Parse(match.Groups[1].Value);

            // Load the sprite dynamically
            string spritePath = $"Sprites/DialogueSprites/{charaName}/{charaName}_{spriteIndex}";
            Sprite loadedSprite = Resources.Load<Sprite>(spritePath);

            if (loadedSprite != null)
            {
                // Adjust the text box to account for the sprite
                textComponentRect.offsetMax = new Vector2(600, textComponentRect.offsetMax.y);
                nameComponentRect.offsetMax = new Vector2(600, nameComponentRect.offsetMax.y);

                // Display the sprite
                dialogueSprite.sprite = loadedSprite;
                dialogueSprite.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Sprite not found at path: {spritePath}");
                ResetSpriteAndTextBox();
            }

            // Remove the sprite tag from the dialogue text
            lines[index] = spriteRegex.Replace(currentLine, string.Empty);
        }
        else
        {
            ResetSpriteAndTextBox();
        }

        textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
    }

    void ResetSpriteAndTextBox()
    {
        // Hide the sprite and reset text box to original size
        dialogueSprite.gameObject.SetActive(false);
        textComponentRect.offsetMin = new Vector2(110, textComponentRect.offsetMin.y);
        nameComponentRect.offsetMin = new Vector2(110, nameComponentRect.offsetMin.y);
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

    void LoadDialogueFile()
    {
        string fullFilePath = $"Assets/Resources/Dialogue/{dialogueFileName}.txt";

        if (!string.IsNullOrEmpty(dialogueFileName) && File.Exists(fullFilePath))
        {
            LoadLinesFromFile(fullFilePath);
        }
        else
        {
            Debug.LogError("Dialogue file not found or filename is empty.");
        }
    }

    // New method to dynamically set the dialogue file name
    public void SetDialogueFileName(string newFileName)
    {
        dialogueFileName = newFileName;
        LoadDialogueFile(); // Reload dialogue from the new file
    }
}