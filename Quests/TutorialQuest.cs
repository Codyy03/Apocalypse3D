using Knife.RealBlood.SimpleController;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Dialogue;

public class TutorialQuest : Quest
{
    [SerializeField] QuestManager questManager;

    [SerializeField] int deadEnemiesCounter;

    [SerializeField] GameObject npc,eatingZombie;
    [SerializeField] AudioClip screamSound,dyingSound;

    public List<DialogueText> dialogueTexts = new List<DialogueText>();

    public List<DialogueText> dialogueTextsAfterTalk = new List<DialogueText>();

    [SerializeField] PlayerController playerController;
    [SerializeField] Dialogue dialogue;
    
    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    private void Start()
    {
        if (state == QuestState.Inactive)
            playerController.RemoveWeapon(0);
    }

    public override void StartQuest()
    {
        questStage = 1;
        questManager.AddQuest(this);
        state = QuestState.Active;
        questPorgress = stages[0];
        audioManager.PlayClip(startMission);
    }

    public override void EndQuest()
    {
        state = QuestState.Completed;
        audioManager.PlayClip(endMission);
        questManager.QuestCompleted(this);
        //npc umiera, koniec zadania
        npc.GetComponent<AudioSource>().PlayOneShot(dyingSound);
        dialogue.conversationStage = -1;
    }

    public override void UpdateQuest()
    {
        questStage++;
        audioManager.PlayClip(updateMission);

        switch (questStage)
        {
            case 2: questPorgress = stages[1]; npc.GetComponent<Animator>().SetTrigger("Death");
                eatingZombie.SetActive(true); npc.transform.position = new Vector3(-41.607f, 5.14f, 87.63f);
                audioManager.PlayClip(screamSound);
                break;

            case 3:
                questPorgress = stages[2]; break;

        }
        questManager.UpdateQuest();
    }
    public void IncreseDeadEnemiesCounter()
    {
        deadEnemiesCounter++;

        questPorgress = $"Zabij zombie {deadEnemiesCounter}/3";
        questManager.UpdateQuest();
        
        if(deadEnemiesCounter == 3)
            UpdateQuest(); 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (questStage != 2) return;

        if (other.CompareTag("Player"))
        {
            dialogue.SetConversationStage(2);
            UpdateQuest();
        }
    }
}
