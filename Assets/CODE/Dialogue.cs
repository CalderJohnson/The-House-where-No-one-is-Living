using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class DialogueSegment { }

public class DialogueTextSegment : DialogueSegment
{
    public List<string> lines;
    public DialogueTextSegment(List<string> lines) { this.lines = lines; }
}

public class DecisionSegment : DialogueSegment
{
    public string key;
    public string prompt;
    public List<Option> options;
    public Dictionary<string,List<string>> blocks;
    public DecisionSegment(string key, string prompt, List<Option> options, Dictionary<string,List<string>> blocks)
    {
        this.key     = key;
        this.prompt  = prompt;
        this.options = options;
        this.blocks  = blocks;
    }
}

public class Option
{
    public string name;
    public string text;
    public Option(string name, string text) { this.name = name; this.text = text; }
}

[System.Serializable]
public class Dialogue : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────
    // 1) SINGLETON BOILERPLATE
    // ─────────────────────────────────────────────────────────────
    private static Dialogue _instance;
    public static Dialogue Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Dialogue>();
                if (_instance == null)
                    Debug.LogWarning("Dialogue instance not found!");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Singleton logic
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            string prevFile = _instance.dialogueFileName;
            Destroy(_instance.gameObject);
            _instance = this;
            _instance.dialogueFileName = prevFile;
        }

        textComponentRect = textComponent.rectTransform;
        nameComponentRect = nameComponent.rectTransform;
    }

    // ─────────────────────────────────────────────────────────────
    // 2) EXISTING FIELDS & INSPECTOR REFERENCES
    // ─────────────────────────────────────────────────────────────
    [Header("Basic Dialogue UI")]
    public string charaName;
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI nameComponent;
    public Image dialogueSprite;
    public GameObject dialogueBox;
    public float textSpeed = 0.03f;

    private RectTransform textComponentRect;
    private RectTransform nameComponentRect;

    [Header("External File")]
    public string dialogueFileName; // e.g. "TestDecision"

    [Header("Decision Panel")]
    public DecisionController decisionController;

    // ─────────────────────────────────────────────────────────────
    // 3) INTERNAL STATE
    // ─────────────────────────────────────────────────────────────
    private List<DialogueSegment> segments;
    private int segmentIndex = 0;
    private int lineIndex    = 0;
    private bool isDialogueActive = false;
    private string currentContent;


    // ─────────────────────────────────────────────────────────────
    // 4) STARTUP
    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        ParseDialogueFile();
        StartDialogue();
    }

    // ─────────────────────────────────────────────────────────────
    // 5) PARSING INTO SEGMENTS
    // ─────────────────────────────────────────────────────────────
    void ParseDialogueFile()
    {
        segments = new List<DialogueSegment>();
        string path = Path.Combine(Application.dataPath, "Resources/Dialogue", dialogueFileName + ".txt");
        if (!File.Exists(path))
        {
            Debug.LogError($"Dialogue file not found: {path}");
            return;
        }

        var raw = File.ReadAllLines(path);
        int i = 0;
        while (i < raw.Length)
        {
            if (raw[i].StartsWith("[Decision:"))
            {
                // Read key
                string key = raw[i++].Replace("[Decision:", "").Replace("]", "").Trim();
                // Read prompt
                string prompt = raw[i++];
                // Read options lines
                var opts = new List<Option>();
                while (i < raw.Length && raw[i].StartsWith("- ["))
                {
                    var m = Regex.Match(raw[i], @"- \[(.+?)\] (.+)");
                    if (m.Success) opts.Add(new Option(m.Groups[1].Value, m.Groups[2].Value));
                    i++;
                }
                // Read per-choice blocks
                var blocks = new Dictionary<string,List<string>>();
                foreach (var opt in opts)
                {
                    if (raw[i].Trim() == $"[{opt.name}]") i++;
                    var blockLines = new List<string>();
                    while (i < raw.Length && !raw[i].StartsWith($"[ChoiceEnd: {opt.name}]"))
                    {
                        blockLines.Add(raw[i++]);
                    }
                    // Skip the [ChoiceEnd: …]
                    i++;
                    blocks[opt.name] = blockLines;
                }
                // Skip [EndDecision: key]
                i++;
                segments.Add(new DecisionSegment(key, prompt, opts, blocks));
            }
            else
            {
                var normal = new List<string>();
                while (i < raw.Length && !raw[i].StartsWith("[Decision:"))
                    normal.Add(raw[i++]);
                segments.Add(new DialogueTextSegment(normal));
            }
        }
    }

    // ─────────────────────────────────────────────────────────────
    // 6) START / END DIALOGUE
    // ─────────────────────────────────────────────────────────────
    void StartDialogue()
    {
        if (isDialogueActive) return;
        isDialogueActive = true;
        Time.timeScale = 0f;
        segmentIndex = 0;
        lineIndex    = 0;
        gameObject.SetActive(true);
        DisplayCurrentLine();
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    // ─────────────────────────────────────────────────────────────
    // 7) DISPLAYING LINES
    // ─────────────────────────────────────────────────────────────
    void DisplayCurrentLine()
    {
        var seg = segments[segmentIndex];
        if (seg is DialogueTextSegment textSeg)
            DisplayLine(textSeg.lines[lineIndex]);
        else if (seg is DecisionSegment decisionSeg)
            DisplayLine(decisionSeg.prompt);
    }

    void DisplayLine(string rawLine)
    {
        // Reset sprite and layout
        dialogueSprite.gameObject.SetActive(false);
        if (dialogueBox != null && dialogueBox.name == "DialogueBox") {
            textComponentRect.offsetMin = new Vector2(110, textComponentRect.offsetMin.y);
            nameComponentRect.offsetMin = new Vector2(110, nameComponentRect.offsetMin.y);
            textComponentRect.offsetMax = new Vector2(textComponentRect.offsetMax.x, 0);
        }

        // Split "Name: Dialogue" if present
        var parts = rawLine.Split(new[] { ": " }, 2, StringSplitOptions.None);
        string content = rawLine;

        if (parts.Length == 2)
        {
            charaName = parts[0];
            nameComponent.text = charaName;
            nameComponent.gameObject.SetActive(true);
            content = parts[1];
            textComponentRect.offsetMax = new Vector2(textComponentRect.offsetMax.x, -60);

            // Check for sprite tag {n}
            var m = Regex.Match(content, @"\{(\d+)\}$");
            if (m.Success)
            {
                int idx = int.Parse(m.Groups[1].Value);
                string spritePath = $"Sprites/DialogueSprites/{charaName}/{charaName}_{idx}";
                var sp = Resources.Load<Sprite>(spritePath);
                if (sp != null)
                {
                    dialogueSprite.sprite = sp;
                    dialogueSprite.gameObject.SetActive(true);
                    textComponentRect.offsetMin = new Vector2(500, textComponentRect.offsetMin.y);
                    nameComponentRect.offsetMin = new Vector2(500, nameComponentRect.offsetMin.y);
                }
                content = Regex.Replace(content, @"\{(\d+)\}$", "");
            }
        }
        else
        {
            nameComponent.text = "";
            nameComponent.gameObject.SetActive(false);
        }

        // ... after stripping name and sprite-tag ...
        currentContent = content;

        StopAllCoroutines();
        textComponent.text = "";
        if (!dialogueBox.activeSelf) dialogueBox.SetActive(true);
        StartCoroutine(TypeLine(currentContent));

    }

    IEnumerator TypeLine(string line)
    {
        foreach (char c in line)
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // 8) INPUT & ADVANCING
    // ─────────────────────────────────────────────────────────────
    void Update()
    {
        if (!isDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // If still typing, jump to full processed content:
            if (textComponent.text != currentContent)
            {
                StopAllCoroutines();
                textComponent.text = currentContent;
            }
            else
            {
                NextLine();
            }
        }
    }

    void NextLine()
    {
        var seg = segments[segmentIndex];
        if (seg is DialogueTextSegment textSeg)
        {
            if (lineIndex < textSeg.lines.Count - 1)
            {
                lineIndex++;
                DisplayCurrentLine();
                return;
            }
            // Advance out of text block
            segmentIndex++;
            lineIndex = 0;
            if (segmentIndex < segments.Count)
                DisplayCurrentLine();
            else
                EndDialogue();
        }
        else if (seg is DecisionSegment decisionSeg)
        {
            // Finished typing the prompt; trigger decision panel.
            TriggerDecision(decisionSeg);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // 9) DECISION LOGIC
    // ─────────────────────────────────────────────────────────────
    void TriggerDecision(DecisionSegment ds)
    {
        Time.timeScale = 0f;
        var optionTexts = ds.options.Select(o => o.text).ToArray();
        decisionController.Show(optionTexts, choiceIdx =>
        {
            // Save decision using key and chosen option name.
            SaveDecision(ds.key, ds.options[choiceIdx].name);

            // Inject the chosen branch's dialogue.
            var chosenName = ds.options[choiceIdx].name;
            var chosenLines = ds.blocks[chosenName];

            // Parse the lines of the selected branch using the same logic as ParseDialogueFile
            var newSegs = new List<DialogueSegment>();
            int i = 0;
            while (i < chosenLines.Count)
            {
                if (chosenLines[i].StartsWith("[Decision:"))
                {
                    string key = chosenLines[i++].Replace("[Decision:", "").Replace("]", "").Trim();
                    string prompt = chosenLines[i++];
                    var opts = new List<Option>();
                    while (i < chosenLines.Count && chosenLines[i].StartsWith("- ["))
                    {
                        var m = Regex.Match(chosenLines[i], @"- \[(.+?)\] (.+)");
                        if (m.Success) opts.Add(new Option(m.Groups[1].Value, m.Groups[2].Value));
                        i++;
                    }
                    var blocks = new Dictionary<string, List<string>>();
                    foreach (var opt in opts)
                    {
                        if (chosenLines[i].Trim() == $"[{opt.name}]") i++;
                        var blockLines = new List<string>();
                        while (i < chosenLines.Count && !chosenLines[i].StartsWith($"[ChoiceEnd: {opt.name}]"))
                            blockLines.Add(chosenLines[i++]);
                        i++; // Skip [ChoiceEnd: …]
                        blocks[opt.name] = blockLines;
                    }
                    i++; // Skip [EndDecision: key]
                    newSegs.Add(new DecisionSegment(key, prompt, opts, blocks));
                }
                else
                {
                    var lines = new List<string>();
                    while (i < chosenLines.Count && !chosenLines[i].StartsWith("[Decision:"))
                        lines.Add(chosenLines[i++]);
                    newSegs.Add(new DialogueTextSegment(lines));
                }
            }

            // Add back the remaining original segments after the decision
            for (int j = segmentIndex + 1; j < segments.Count; j++)
                newSegs.Add(segments[j]);

            segments = newSegs;
            segmentIndex = 0;
            lineIndex = 0;
            DisplayCurrentLine();
        });
    }

    void SaveDecision(string key, string choiceName)
    {
        Debug.Log($"[Decision Saved] Key: {key} | Choice: {choiceName}");
        
        UpdateNode updateNode = GetComponent<UpdateNode>();
        if (updateNode != null && updateNode.affectsDecisionTree)
        {
            // Find the choice index based on choiceName
            int choiceIndex = updateNode.possibleDecisionNodeIDs.IndexOf(choiceName);

            if (choiceIndex != -1)
            {
                updateNode.TryUpdateDecisionNode(choiceIndex);
            }
            else
            {
                Debug.LogWarning($"[SaveDecision] Choice name '{choiceName}' not found in possibleDecisionNodeIDs.");
            }
        }
    }


    // ─────────────────────────────────────────────────────────────
    // 10) EXPOSED METHOD FOR SETTING DIALOGUE FILE
    // ─────────────────────────────────────────────────────────────
    public void SetDialogueFileName(string newFileName)
    {
        // Only allow resetting if dialogue isn't active.
        if (isDialogueActive)
            return;
        dialogueFileName = newFileName;
        Debug.Log($"Dialogue set to: {newFileName}");
        ParseDialogueFile();
        StartDialogue();
    }
}
