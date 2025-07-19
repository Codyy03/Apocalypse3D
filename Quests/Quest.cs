using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : MonoBehaviour
{
    public int questID;

    public int questStage = 0;

    public string questPorgress;

    public string questName;

    public string[] stages;

    public enum QuestState { Inactive, Active, Completed }
    public QuestState state;

    public int rewardXP;
    public List<Item> rewardItems;

    [System.Serializable]
    public class QuestObjective
    {
        public string description;
        public bool isCompleted;
    }

    protected AudioManager audioManager;
    public AudioClip startMission;
    public AudioClip updateMission;
    public AudioClip endMission;
    public abstract void StartQuest();

    public abstract void UpdateQuest();

    public abstract void EndQuest();

}
