using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
namespace Quests
{
    public class QuestDescriptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Quest quest;

        [SerializeField] GameObject onEnterImage;
        [SerializeField] TextMeshProUGUI questName;
        [SerializeField] QuestManager questManager;
        [SerializeField] QuestUIController questUIController;
       public Button followQuestButton;

        private void Awake()
        {
            questManager = FindFirstObjectByType<QuestManager>();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            DisplayQuestDescription.Instance.SetQuestData(quest.questName, quest.questDescriptions, quest.questPorgress);

            // znajduje przycisk, œledz / przestañ œledziæ zadania 
            if (followQuestButton == null)
                followQuestButton = GameObject.FindWithTag("Follow quest button").GetComponent<Button>();

            // usuwa wszystkie eventy z przycisku
            followQuestButton.onClick.RemoveAllListeners();

            // œledzi w UI obecnie zaznaczone zadanie
            followQuestButton.onClick.AddListener(() =>
            {
                questManager.SetActiveQuest(quest);
            });

            questUIController.SetCurrentQuestActivity(quest.isActive);
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
        public void SetQuestName() => questName.text = quest.questName;

    }
}


