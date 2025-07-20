using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class QuestManager : MonoBehaviour
{
    public Quest currentQuest;

    public List<Quest> allQuests;

    public List<Quest> activeQuests = new List<Quest>();

    public List<Quest> questsCoompleted;

    [SerializeField] TextMeshProUGUI questName;
    [SerializeField] TextMeshProUGUI questPurpose;

    public void AddQuest(Quest quest)
    {
        activeQuests.Add(quest);
        currentQuest = quest;

        questName.text = quest.questName;
        questPurpose.text = quest.stages[0];

        // pokaz UI z informacjami o zadaniu

        transform.GetChild(0).gameObject.SetActive(true);
    }
    public void UpdateQuest()
    {
        questPurpose.text = currentQuest.questPorgress;
    }
    public void QuestCompleted(Quest quest)
    {
        questsCoompleted.Add(quest);

        activeQuests.Remove(quest);

        transform.GetChild(0).gameObject.SetActive(false);

    }

}
