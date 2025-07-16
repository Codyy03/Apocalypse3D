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

    public List<DialogueText> dialogueTexts = new List<DialogueText>();

    public List<DialogueText> dialogueTextsAfterTalk = new List<DialogueText>();

    DialoguesManager manager;
    Transform player;
    public float distanceToTalk;
    public int conversationStage = 0;
    [SerializeField] GameObject notification;
    private void Awake()
    {
        manager = FindFirstObjectByType<DialoguesManager>();
        player = FindFirstObjectByType<PlayerController>().transform;
      
    }
    private void Update()
    {
        if (Vector3.Distance(player.position, transform.position) <= distanceToTalk)
        {
            notification.SetActive(true);
            if (Input.GetKeyUp(KeyCode.E))
            {
                switch (conversationStage)
                {
                    case 0:  manager.StartDialogue(dialogueTexts); conversationStage++; break;
                    case 1:  manager.StartDialogue(dialogueTextsAfterTalk); break;
                }

               

            }
        }
        else notification.SetActive(false);

    }
}
