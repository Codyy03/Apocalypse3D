using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Quests 
{
    public abstract class Quest : MonoBehaviour
    {
        public enum QuestState 
        { 
            Inactive, 
            Active, 
            Completed 
        }
        public enum QuestType
        {
            Fabular,
            Side,
            Mission
        }

        [Header("Identyfikatory")]
        public int questID;
        public int questStage = 0;
        public QuestState state;

        [Header("Dialogi")]
        public string questPorgress;
        public string questName;
        public string[] stages;

        [Header("Opis w UI")]
        [TextArea] public string questDescriptions;
        [TextArea] public string onEndQuestDescriptions;

        public QuestType questType;

        [Header("Nagroda")]
        public int rewardXP;
        public List<Item> rewardItems;

        [Header("DŸwiêki ogólne")]
        protected AudioManager audioManager;
        public AudioClip startMission;
        public AudioClip updateMission;
        public AudioClip endMission;

        public bool isActive;

        [Header("Manager")]
        [SerializeField] protected QuestManager questManager;
        protected virtual void Awake()
        {
            audioManager = FindFirstObjectByType<AudioManager>();
        }

        public virtual void StartQuest()
        {
            state = QuestState.Active;
            SetStage(1);
            questManager.AddQuest(this);

            if (startMission != null)
                audioManager.PlayClip(startMission);
        }

        public void AdvanceStage(bool fromLoad = false)
        {
            SetStage(questStage + 1, fromLoad);
        }
        public void SetStage(int newStage, bool fromLoad = false)
        {
            if (questStage == newStage) return;

            OnStageExit(questStage, fromLoad);

            int prevStage = questStage;
            questStage = newStage;

            questPorgress = SafeStageText(questStage);

            if (!fromLoad && updateMission != null && questStage > 1 && prevStage > 0) 
                audioManager?.PlayClip(updateMission);

            OnStageEnter(questStage, fromLoad);

            if (questManager != null && questManager.currentQuest == this)
                questManager.NotifyQuestUpdated(this);
        }
        protected virtual string SafeStageText(int s)
        {
            if (stages != null && s - 1 >= 0 && s -1 < stages.Length)
                return stages[s - 1];
            return questPorgress;
        }
        protected virtual void OnStageEnter(int s, bool fromLoad) { }
        protected virtual void OnStageExit(int s, bool fromLoad) { }

        public virtual void EndQuest()
        {
            state = QuestState.Completed;

            if (endMission != null)
                audioManager?.PlayClip(endMission);

            questManager?.QuestCompleted(this);

        }
    }
}


