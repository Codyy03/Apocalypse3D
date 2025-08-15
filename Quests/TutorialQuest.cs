using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Quests
{
    public class TutorialQuest : Quest
    {
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
        BasicMotionAnimations basicMotion;
        GoToPointInWorld goToPointInWorld;

        [Header("Dialogi")]
        [SerializeField] Dialogues.DialogueTextSO endQuestDialogue;
        [SerializeField] Dialogues.DialoguesManager dialoguesManager;
        [SerializeField] Dialogues.Dialogue dialogue;

        Dictionary<int, Action<bool>> enter;
        Dictionary<int, Action<bool>> exit;

        protected override void Awake()
        {
            base.Awake();

            if (npc != null)
            {
                goToPointInWorld = npc.GetComponent<GoToPointInWorld>();
                basicMotion = npc.GetComponent<BasicMotionAnimations>();
            }

            enter = new()
            {
                {1, EnterStage1},
                {2, EnterStage2},
                {3, EnterStage3}
            };

            exit = new()
            {
                {1, ExitStage1},
                {2, ExitStage2},
                {3, ExitStage3}
            };

            if (endQuestDialogue != null)
            {
                // dla pewnosci ze jest dodany tylko raz
                endQuestDialogue.dialogEvent.RemoveListener(EndQuest);
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
        /// <summary>
        /// Zacznij zadanie
        /// </summary>
        public override void StartQuest()
        {
            base.StartQuest();

            StartTutorial();
        }
        /// <summary>
        /// Akcja specyficzna dla tego zadania
        /// </summary>
        void StartTutorial()
        {
            goToPointInWorld.SetGoToPoint(true);
            dialogue.conversationStage = 1;

            foreach (ZombieController zombie in zombies)
                zombie?.SetMarkMapVisibility(true);
        }
        protected override void OnStageEnter(int s, bool fromLoad)
        {
            if (enter.TryGetValue(s, out var enterStage)) enterStage(fromLoad);
        }
        protected override void OnStageExit(int s, bool fromLoad)
        {
            if (exit.TryGetValue(s, out var exitStage)) exitStage(fromLoad);
        }

        void EnterStage1(bool fromLoad)
        {
            StartTutorial();
        }
        void EnterStage2(bool fromLoad)
        {
            // Ustaw docelowy stan sceny dla etapu 2
            if (basicMotion != null)
                basicMotion.ChangeAnimation(basicMotion.death);

            if (eatingZombie != null)
                eatingZombie.SetActive(true);

            if (npc != null)
                npc.transform.position = new Vector3(-41.607f, 5.14f, 87.63f);

            // Jeœli w etapie 2 NPC nadal ma iœæ do punktu, w³¹cz to:
           // goToPointInWorld?.SetGoToPoint(true);

            // DŸwiêk tylko przy normalnym przejœciu, nie przy wczytaniu
            if (!fromLoad && screamSound != null)
                audioManager?.PlayClip(screamSound);

            if (!fromLoad) return;

            foreach (ZombieController zombie in zombies)
            {
                Destroy(zombie.gameObject);
            }
            zombies.Clear();

        }
        void EnterStage3(bool fromLoad)
        {
            goToPointInWorld?.SetGoToPoint(false);
            dialogue.conversationStage = 2; 
        }
        private void ExitStage1(bool fromLoad) 
        {
        }
        private void ExitStage2(bool fromLoad)
        {
        }
        private void ExitStage3(bool fromLoad) { }

        public override void EndQuest()
        {
            base.EndQuest();

            //npc umiera, koniec zadania
            npc.GetComponent<AudioSource>().PlayOneShot(dyingSound);
            dialogue.conversationStage = -1;
        }
        public void IncreseDeadEnemiesCounter()
        {
            deadEnemiesCounter++;

            questPorgress = $"Zabij zombie {deadEnemiesCounter}/3";
            questManager.NotifyQuestUpdated(this);

            if (deadEnemiesCounter == 3)
                AdvanceStage();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (questStage != 2) return;

            if (other.CompareTag("Player"))
            {
                dialogue.SetConversationStage(2);
                AdvanceStage();
            }
        }
    }
}

