using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
namespace Quests
{
    public class QuestDescriptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Quest quest;

        [SerializeField] GameObject onEnterImage;
        [SerializeField] TextMeshProUGUI questName;

        public void OnPointerClick(PointerEventData eventData)
        {
            DisplayQuestDescription.Instance.SetQuestData(quest.questName, quest.questDescriptions);
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


