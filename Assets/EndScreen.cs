using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public GameObject goodEndingText;
    public GameObject badEndingText;

    void Start()
    {
        DecisionNode currentNode = DecisionManager.Instance.GetCurrentNode();

        if (currentNode.nodeID == "Level1Exit1")
        {
            goodEndingText.SetActive(true);
            badEndingText.SetActive(false);
        }
        else if (currentNode.nodeID == "Level1Exit2")
        {
            goodEndingText.SetActive(false);
            badEndingText.SetActive(true);
        }
        else
        {
            goodEndingText.SetActive(false);
            badEndingText.SetActive(false);
        }
    }
}
