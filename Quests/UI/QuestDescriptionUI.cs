using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Quests
{
    public class QuestDescriptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private QuestData questData;
        private QuestDefinition questDefinition;

        [SerializeField] GameObject onEnterImage;
        [SerializeField] GameObject questCompletedImage;
        [SerializeField] TextMeshProUGUI questName;
        [SerializeField] QuestManager questManager;
        [SerializeField] QuestUIController questUIController;

        private Button followQuestButton;

        private void Awake()
        {
            questManager = FindFirstObjectByType<QuestManager>();
            questUIController = FindFirstObjectByType<QuestUIController>();
        }
        private void OnEnable()
        {
            if (questData.questState == Quest.QuestState.Completed)
                questCompletedImage.SetActive(true);

        }
        public void SetData(QuestData data, QuestDefinition def)
        {
            questData = data;
            questDefinition = def;
            questName.text = def.questName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HandleQuestData();
            HandleButton();
        }

        void HandleQuestData()
        {
            string questProgress = "";

            if (questData.questState == Quest.QuestState.Active)
                questProgress = questDefinition.stages.Length >= questData.questStage
                    ? questDefinition.stages[questData.questStage - 1]
                    : "";
            else
                questProgress = questDefinition.onEndQuestDescriptions;

            DisplayQuestDescription.Instance.SetQuestData(
                questDefinition.questName,
                questDefinition.questDescriptions,
                questProgress
            );
        }
        void HandleButton()
        {
            DataSynchronization();

            questUIController.SetCurrentQuestActivity(questData);

            // znajdz przycisk jezeli go jeszcze nie znaleziono
            if (followQuestButton == null)
                followQuestButton = GameObject.FindWithTag("Follow quest button").GetComponent<Button>();

            followQuestButton.onClick.RemoveAllListeners();

            // jezeli zadanie ukonczone nie pozwalaj przejœæ dalej
            if (questData.questState == Quest.QuestState.Completed) return;


            // aktywuj zadanie
            followQuestButton.onClick.AddListener(() =>
            {
                questManager.SetActiveQuest(questData.questID);

                questUIController.SetCurrentQuestUIState(questData?.isActive ?? false);

                // odœwie¿ dane 
                DataSynchronization();
                questUIController.SetCurrentQuestActivity(questData);
            });

        }
        void DataSynchronization()
        {
            var data = questManager.GetQuestState(questData.questID);
            questData = data; // synchronizacja najnowszych danych
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            onEnterImage.SetActive(true);
            questName.fontStyle = FontStyles.Bold;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onEnterImage.SetActive(false);
            questName.fontStyle = FontStyles.Normal;
        }
    }
}