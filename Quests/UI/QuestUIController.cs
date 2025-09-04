using Quests;
using TMPro;
using UnityEngine;

namespace Quests
{
    public class QuestUIController : MonoBehaviour
    {
        [SerializeField] QuestManager questManager;
        [SerializeField] TextMeshProUGUI questName;
        [SerializeField] TextMeshProUGUI questPurpose;

        [Tooltip("Text który przedstawia œledz/ przestañ œledziæ")]
        [SerializeField] TextMeshProUGUI currentActivityText;

        [SerializeField] GameObject questPrefab;

        [Tooltip("Element UI po prawej stronie pokazuj¹cy obecnie œledzone zadanie")]
        [SerializeField] GameObject questInfoUI;

        [SerializeField] Transform questList;

        private void Awake()
        {
            questManager.OnQuestActivated += ShowQuest;
            questManager.OnQuestActivated += UpdateMissionList;
            questManager.OnQuestUpdated += UpdateQuestUI;
            questManager.OnQuestCompleted += HideQuest;
            questManager.updateCustemQuestProgress += SetCustomQuestProgress;
        }
        private void OnDisable()
        {
            questManager.OnQuestActivated -= ShowQuest;
            questManager.OnQuestUpdated -= UpdateQuestUI;
            questManager.OnQuestCompleted -= HideQuest;
            questManager.updateCustemQuestProgress -= SetCustomQuestProgress;
        }

        /// <summary>
        /// pokazuje dane zadania w UI
        /// </summary>
        /// <param name="questData"></param>
        private void ShowQuest(QuestData questData)
        {
            var def = questManager.GetQuestDefinition(questData.questID);

            if (def == null) return;
            
            questName.text = def.questName;
            questPurpose.text = def.stages != null && def.stages.Length > 0
                ? def.stages[questData.questStage - 1]
                : "";

            SetCurrentQuestUIState(true);
        }
        public void UpdateMissionList(QuestData questData)
        {
            var def = questManager.GetQuestDefinition(questData.questID);
            var questDescriptionUI = Instantiate(questPrefab, questList)
                                      .GetComponent<QuestDescriptionUI>();
            questDescriptionUI.SetData(questData, def);

          //  questManager.OnQuestActivated -= UpdateMissionList;
        }
        /// <summary>
        /// ustawia inny ni¿ w liœcie cel obecnego zadania
        /// </summary>
        /// <param name="custemProgress">progres zadnia</param>
        public void SetCustomQuestProgress(string custemProgress)
        {
            questPurpose.text = custemProgress;
        }
        private void UpdateQuestUI(QuestData questData)
        {
            if (questManager.currentQuestID == questData.questID)
            {
                QuestDefinition def = questManager.GetQuestDefinition(questData.questID);
                questPurpose.text = def.stages != null && def.stages.Length >= questData.questStage
                    ? def.stages[questData.questStage - 1]
                    : "";
            }
        }
        private void HideQuest(QuestData questData)
        {
            SetCurrentQuestUIState(false);
        }
        public void SetCurrentQuestUIState(bool state) => questInfoUI.SetActive(state);
        public void SetCurrentQuestActivity(QuestData questData) => currentActivityText.text = questData.isActive ? "Dezaktywuj" : "Aktywuj";
    }
}

