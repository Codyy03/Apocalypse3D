using UnityEngine;
using TMPro;
public class DisplayQuestDescription : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI questNameText;
    public static DisplayQuestDescription Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void SetQuestData(string questName, string questDescription, string currentQuestPurpose)
    {
        questNameText.text = questName;

        string descriptionSteps = "";

        descriptionSteps += questDescription;
        descriptionSteps += "\n\n";

        descriptionSteps += currentQuestPurpose;

        descriptionText.text = descriptionSteps;
    }
}
