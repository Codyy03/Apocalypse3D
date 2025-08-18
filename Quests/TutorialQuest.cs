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

        [Header("Znaczniki na mapie")]
        [SerializeField] GameObject npcMapMark;
        [SerializeField] GameObject bandageMapMark;

        [Header("Dialogi")]
        [SerializeField] Dialogues.DialogueTextSO endQuestDialogue;
        [SerializeField] Dialogues.DialoguesManager dialoguesManager;
        [SerializeField] Dialogues.Dialogue dialogue;

        readonly int requireDeadEnemies = 3;
        readonly int requireConversationStageToActivateOnTrigger = 4;
        readonly Vector3 teleportDestination = new Vector3(-41.607f, 5.14f, 87.63f);
        protected override void Awake()
        {
            base.Awake();

            if (npc != null)
            {
                goToPointInWorld = npc.GetComponent<GoToPointInWorld>();
                basicMotion = npc.GetComponent<BasicMotionAnimations>();
            }

            RegisterStage(1, EnterStage1, ExitStage1);
            RegisterStage(2, EnterStage2, ExitStage2);
            RegisterStage(3, EnterStage3, ExitStage3);
            RegisterStage(4, EnterStage4, ExitStage4);
            RegisterStage(5, EnterStage5, ExitStage5);

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

        // zaczyna questa
        void EnterStage1(bool fromLoad)
        {
            StartTutorial();
        }
        // zombie zosta³y zabite, gracz wraca
        void EnterStage2(bool fromLoad)
        {
            dialogue.IncreaseConversationStage();
        }
        // gracz wróci³ i musi isæ po banda¿e
        void EnterStage3(bool fromLoad)
        {
            bandageMapMark.SetActive(true);
            npcMapMark.SetActive(false);

            if (fromLoad)
                DestroyZombies();
        }
        // gracz wzi¹³ banda¿e, musi sprawdziæ co sie sta³o
        void EnterStage4(bool fromLoad)
        {
            if (eatingZombie != null)
                eatingZombie.SetActive(true);

            KillNpcAtDestination();

            // DŸwiêk tylko przy normalnym przejœciu, nie przy wczytaniu
            if (!fromLoad && screamSound != null)
                audioManager?.PlayClip(screamSound);

            if (!fromLoad) return;

            DestroyZombies();
        }
        // gracz zobaczy³ co sie sta³o
        void EnterStage5(bool fromLoad)
        {
            goToPointInWorld?.SetGoToPoint(false);
            dialogue.IncreaseConversationStage();

            if (!fromLoad) return;

            DestroyZombies();

            if (npc != null)
            {
                KillNpcAtDestination();
                dialogue.EndConversation();
            }
        }
        private void ExitStage1(bool fromLoad) 
        {
            npcMapMark.SetActive(true);
        }
        private void ExitStage2(bool fromLoad)
        {
            npcMapMark.SetActive(false);
        }
        private void ExitStage3(bool fromLoad) {

            npcMapMark.SetActive(true);
            bandageMapMark.SetActive(false);
        }
        private void ExitStage4(bool fromLoad) { }
        private void ExitStage5(bool fromLoad) { }
        public override void EndQuest()
        {
            base.EndQuest();

            //npc umiera, koniec zadania
            if (npc != null)
                npc.GetComponent<AudioSource>()?.PlayOneShot(dyingSound);

            dialogue.EndConversation();
        }
        public void IncreaseDeadEnemiesCounter()
        {
            deadEnemiesCounter++;

            questPorgress = $"Zabij zombie {deadEnemiesCounter}/3";
            questManager.NotifyQuestUpdated(this);

            if (deadEnemiesCounter == requireDeadEnemies)
                AdvanceStage();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (questStage != requireConversationStageToActivateOnTrigger) return;

            if (other.CompareTag("Player"))
                AdvanceStage();
        }
        void DestroyZombies()
        {
            foreach (ZombieController zombie in zombies)
            {
                Destroy(zombie?.gameObject);
            }
            zombies.Clear();
        }
        void KillNpcAtDestination()
        {
            if (npc != null)
            {
                npc.transform.position = teleportDestination;
                basicMotion?.ChangeAnimation(basicMotion.death);
            }
        }
    }
}

