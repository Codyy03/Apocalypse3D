using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Quests
{
    public class QuestManager : MonoBehaviour
    {
        public Quest currentQuest;

        public List<Quest> allQuests;

        public List<Quest> activeQuests = new List<Quest>();

        public List<Quest> questsCompleted;

        [SerializeField] TextMeshProUGUI questName;
        [SerializeField] TextMeshProUGUI questPurpose;

        [SerializeField] Transform questList;
        [SerializeField] GameObject questPrefab;
        public void AddQuest(Quest quest)
        {
            activeQuests.Add(quest);
            currentQuest = quest;

            questName.text = quest.questName;
            questPurpose.text = quest.stages[0];

            // pokaz UI z informacjami o zadaniu
            transform.GetChild(0).gameObject.SetActive(true);

            // dodaj zadanie do listy
            UpdateMissionList(quest);
        }
        public void UpdateQuest()
        {
            questPurpose.text = currentQuest.questPorgress;
        }
        public void QuestCompleted(Quest quest)
        {
            questsCompleted.Add(quest);

            activeQuests.Remove(quest);

            transform.GetChild(0).gameObject.SetActive(false);
        }

        public void UpdateMissionList(Quest quest)
        {
            QuestDescriptionUI questDescriptionUI = Instantiate(questPrefab, questList).GetComponent<QuestDescriptionUI>();

            questDescriptionUI.quest = quest;
            questDescriptionUI.SetQuestName();
        }
    }
}

