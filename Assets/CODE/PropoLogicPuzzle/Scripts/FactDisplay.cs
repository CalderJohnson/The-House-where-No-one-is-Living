using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FactDisplay : MonoBehaviour
{
    public TextMeshProUGUI factIDText;
    public TextMeshProUGUI englishText;
    
    public void Setup(Fact fact)
    {
        if(factIDText != null)
            factIDText.text = fact.factID;
        if(englishText != null)
            englishText.text = fact.englishSentence;
    }
}
