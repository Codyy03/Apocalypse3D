using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
public class Dialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueText
    {
        [TextArea]
        public string dialogue;
        public List<DialogueChoice> dialogsChoices;
        public int getDialogueChoices = -1; // powróæ do opcji z dialogu nr ..., jezeli -1 nie wracaj
        public UnityEvent dialogEvent;
    }
    [System.Serializable]
    public class DialogueChoice
    {
        public string text;
        public int nextDialogueIndex; //  -1 koñczy rozmowê
        public bool endsDialogue = false;
        public UnityEvent onChoiceSelected;
    }

    [System.Serializable]
    public class DialogueStage
    {
        public List<DialogueText> texts;
    }

    [Tooltip("Etapy konwersacji NPC-a — ka¿dy to osobna faza.")]
    [SerializeField] public List<DialogueStage> dialogueStages = new();

    [SerializeField] DialoguesManager manager;
    [SerializeField] Transform player;
    public float distanceToTalk;
    public int conversationStage = 0; // -1 = brak mozliwoœci dlaszej rozmowy
    [SerializeField] GameObject notification;
 
    private void Update()
    {
        if (Vector3.Distance(player.position, transform.position) <= distanceToTalk)
        {
            notification.SetActive(true);
            if (Input.GetKeyUp(KeyCode.E))
            {
                if (conversationStage < dialogueStages.Count && conversationStage!=-1)
                {
                    manager.StartDialogue(dialogueStages[conversationStage].texts);
                }
            }
        }
        else notification.SetActive(false);

    }
    public void IncreaseConversationStage()
    {
        conversationStage++;
    }

    public void SetConversationStage(int value)
    {
        conversationStage = value;
    }
}
