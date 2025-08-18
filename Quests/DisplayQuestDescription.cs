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
    /// <summary>
    /// aktualizuje informacje o zadaniu w dzienniku zadañ
    /// </summary>
    /// <param name="questName">nazwa zadania</param>
    /// <param name="questDescription">pocz¹tkowy opis zadania</param>
    /// <param name="currentQuestPurpose">opis celu zadania</param>
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
