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

    public void SetQuestData(string questName, string[] questDescription)
    {
        questNameText.text = questName;

        string descriptionSteps = "";

        foreach (string descriptionStep in questDescription)
        {
            descriptionSteps += descriptionStep;
            descriptionSteps += "\n\n";
        }

        descriptionText.text = descriptionSteps;
    }
}
