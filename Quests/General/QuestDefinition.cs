using System.Collections.Generic;
using UnityEngine;

namespace Quests
{
    [CreateAssetMenu(fileName = "QuestDefinition", menuName = "Quests/QuestDefinition")]
    public class QuestDefinition : ScriptableObject
    {
        [Header("Dialogi")]
        public int questID;
        public string questName;
        public string[] stages;

        [Header("Opis w UI")]
        [TextArea] public string questDescriptions;
        [TextArea] public string onEndQuestDescriptions;

        [Header("Nagroda")]
        public int rewardXP;
        public List<Item> rewardItems;
    }
}

