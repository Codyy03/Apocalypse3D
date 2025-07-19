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
    }

    public void UpdateQuest()
    {
        questPurpose.text = currentQuest.questPorgress;
    }

}
