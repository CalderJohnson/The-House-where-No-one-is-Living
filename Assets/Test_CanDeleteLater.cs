using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    [Header("NPC Settings")]
    public string defaultDialogue = "Hello, traveler!";
    public string afterWardrobeDialogue = "I see you've been rummaging through the wardrobe...";
    public string decisionNodeID = "TalkedToNPCFirst"; // Node to set if player talks first

    public bool affectsDecisionTree = true; // Set to false if this NPC doesn't affect the tree

    public void Interact()
    {
        string currentNode = DataPersistenceManager.Instance.gameData.currentDecisionNode;
        string dialogueToShow = defaultDialogue;

        // If the wardrobe has been opened, change the dialogue
        if (currentNode == "WardrobeOpened")
        {
            dialogueToShow = afterWardrobeDialogue;
        }

        // Display the dialogue (You can replace this with your actual dialogue system)
        Debug.Log(dialogueToShow);

        // Update the decision tree if needed
        if (affectsDecisionTree && currentNode == "start")
        {
            DataPersistenceManager.Instance.gameData.currentDecisionNode = decisionNodeID;
            DataPersistenceManager.Instance.SaveGame();
            Debug.Log($"Decision node updated to: {decisionNodeID}");
        }
    }
}
