using Quests;
using TMPro;
using UnityEngine;

namespace Quests
{
    public class QuestUIController : MonoBehaviour
    {
        [SerializeField] private QuestManager questManager;
        [SerializeField] private TextMeshProUGUI questName;
        [SerializeField] private TextMeshProUGUI questPurpose;

        [Tooltip("Text który przedstawia œledz/ przestañ œledziæ")]
        [SerializeField] TextMeshProUGUI currentActivityText;

        [SerializeField] GameObject questPrefab;
        [SerializeField] Transform questList;
        private void OnEnable()
        {
            questManager.OnQuestActivated += ShowQuest;
            questManager.OnQuestActivated += UpdateMissionList;
            questManager.OnQuestUpdated += UpdateQuestUI;
            questManager.OnQuestDeactivated += HideQuest;
            questManager.OnQuestCompleted += HideQuest;
        }

        private void OnDisable()
        {
            questManager.OnQuestActivated -= ShowQuest;
            questManager.OnQuestUpdated -= UpdateQuestUI;
            questManager.OnQuestDeactivated -= HideQuest;
            questManager.OnQuestCompleted -= HideQuest;
        }

        private void ShowQuest(Quest quest)
        {
            questName.text = quest.questName;
            questPurpose.text = quest.stages != null && quest.stages.Length > 0
                                ? quest.stages[0]
                                : quest.questPorgress;
            SetCurrentQuestUIState(true);
        }

        private void UpdateQuestUI(Quest quest)
        {
            if (questManager.currentQuest == quest)
                questPurpose.text = quest.questPorgress;
        }
        public void UpdateMissionList(Quest quest)
        {
            QuestDescriptionUI questDescriptionUI = Instantiate(questPrefab, questList).GetComponent<QuestDescriptionUI>();
            questDescriptionUI.quest = quest;
            questDescriptionUI.SetQuestName();

            questManager.OnQuestActivated -= UpdateMissionList;
        }

        private void HideQuest(Quest quest)
        {
            SetCurrentQuestUIState(false);

        }
        public void SetCurrentQuestUIState(bool state) => transform.GetChild(0).gameObject.SetActive(state);
        public void SetCurrentQuestActivity(bool questState) => currentActivityText.text = questState ? "Dezaktywuj" : "Aktywuj";
    }
}

