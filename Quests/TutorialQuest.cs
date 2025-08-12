using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Quests
{
    public class TutorialQuest : Quest
    {
        [Header("Manager")]
        [SerializeField] QuestManager questManager;

        [SerializeField] int deadEnemiesCounter;

        [Header("Postacie w zadaniu")]
        [SerializeField] GameObject npc;

        [Header("Przeciwnicy w zadaniu")]
        [SerializeField] GameObject eatingZombie;
        [SerializeField] List<ZombieController> zombies;

        [Header("DŸwiêki w zadaniu")]
        [SerializeField] AudioClip screamSound;
        [SerializeField] AudioClip dyingSound;

        [Header("Niezbêdne klasy")]
        [SerializeField] PlayerController playerController;
        [SerializeField] Dialogues.Dialogue dialogue;
        [SerializeField] Dialogues.DialoguesManager dialoguesManager;

        [Header("Dialogi")]
        [SerializeField] Dialogues.DialogueTextSO endQuestDialogue;

        BasicMotionAnimations basicMotion;
        GoToPointInWorld goToPointInWorld;
        bool listenerAdded = false;

        private void Awake()
        {
            goToPointInWorld = npc.GetComponent<GoToPointInWorld>();
            audioManager = FindFirstObjectByType<AudioManager>();
            basicMotion = npc.GetComponent<BasicMotionAnimations>();

            if (!listenerAdded)
            {
                endQuestDialogue.dialogEvent.AddListener(EndQuest);
            }
        }
        private void Start()
        {

            if (state == QuestState.Inactive)
            {
                UnityEvent unityEvent = new UnityEvent();

                unityEvent.AddListener(StartQuest);

                dialoguesManager.SetOnStageEvent(unityEvent);
                playerController.RemoveWeapon(0);
                dialoguesManager.StartDialogue(dialogue.dialogueStages[0].texts);
            }
        }
        public override void StartQuest()
        {
            questStage = 1;
            questManager.AddQuest(this);
            state = QuestState.Active;
            questPorgress = stages[0];
            audioManager.PlayClip(startMission);

            goToPointInWorld.SetGoToPoint(true);

            dialogue.conversationStage = 1;

            foreach (var zombie in zombies)
                zombie.SetMarkMapVisibility(true);
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
                case 2: HandeFirstQuestStage(); break;
                case 3: HandeSecondQuestStage(); break;
            }
            questManager.UpdateQuest();
        }
        void HandeFirstQuestStage()
        {
            questPorgress = stages[1]; basicMotion.ChangeAnimation((basicMotion.death));
            eatingZombie.SetActive(true); npc.transform.position = new Vector3(-41.607f, 5.14f, 87.63f);
            audioManager.PlayClip(screamSound);
        }
        void HandeSecondQuestStage()
        {
            questPorgress = stages[2];
        }
        public void IncreseDeadEnemiesCounter()
        {
            deadEnemiesCounter++;

            questPorgress = $"Zabij zombie {deadEnemiesCounter}/3";
            questManager.UpdateQuest();

            if (deadEnemiesCounter == 3)
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
}

