using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Dialogues.Dialogue;

namespace Dialogues
{
    [CreateAssetMenu(fileName = "DialogueTextSO", menuName = "Dialogue/DialogueTextSO")]
    public class DialogueTextSO : ScriptableObject
    {
        [TextArea]
        public string dialogue;
        public List<DialogueChoice> dialogsChoices;
        public DialogueTextSO returnToNode; // powr�t do opcji dialogowych
        public UnityEvent dialogEvent;
    }
}

