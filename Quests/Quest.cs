using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Quests 
{
    public abstract class Quest : MonoBehaviour
    {
        [Header("Identyfikatory")]
        public int questID;
        public int questStage = 0;

        [Header("Dialogi")]
        public string questPorgress;
        public string questName;
        public string[] stages;

        [Header("Opis w UI")]
        [TextArea]
        public string[] questDescriptions;

        public enum QuestState { Inactive, Active, Completed }
        public QuestState state;

        public enum QuestType
        {
            Fabular,
            Side,
            Mission
        }
        public QuestType questType;

        [Header("Nagroda")]
        public int rewardXP;
        public List<Item> rewardItems;

        [System.Serializable]
        public class QuestObjective
        {
            public string description;
            public bool isCompleted;
        }

        protected AudioManager audioManager;

        [Header("DŸwiêki ogólne")]
        public AudioClip startMission;
        public AudioClip updateMission;
        public AudioClip endMission;
        public abstract void StartQuest();

        public abstract void UpdateQuest();

        public abstract void EndQuest();

    }
}


