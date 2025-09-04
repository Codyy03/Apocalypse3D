using Dialogues;
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

        [Header("Loot")]
        [SerializeField] Loot boxLoot;
        [SerializeField] Item bandage;
        BoxCollider boxCollider;

        [Header("Niezbêdne klasy")]
        [SerializeField] SceneTeleportManager sceneTeleportManager;
        [SerializeField] PlayerController playerController;
        BasicMotionAnimations basicMotion;
        GoToPointInWorld goToPointInWorld;

        [Header("Znaczniki na mapie")]
        [SerializeField] GameObject npcMapMark;
        [SerializeField] GameObject bandageMapMark;
        [SerializeField] GameObject NextSceneMapMark;

        [Header("Dialogi")]
        [SerializeField] Dialogues.DialogueTextSO endQuestDialogue;
        [SerializeField] Dialogues.Dialogue dialogue;
        [SerializeField] Dialogues.DialoguesManager dialoguesManager;

        readonly int playerStartedDialgue = 1;
        readonly int playerKiledZombies = 2;
        readonly int playerHasToGoAfterBandages = 3;
        readonly int endingDialogue = 4;

        readonly int requireDeadEnemies = 3;
        readonly int requireConversationStageToActivateOnTrigger = 4;
        readonly Vector3 teleportDestination = new Vector3(-41.607f, 5.14f, 87.63f);
        protected override void Awake()
        {
            base.Awake();

            dialoguesManager = DialoguesManager.Instance;

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
            RegisterStage(6, EndQuest, ExitEndQuest);

            if (endQuestDialogue != null)
            {
                // dla pewnosci ze jest dodany tylko raz
                endQuestDialogue.dialogEvent.RemoveAllListeners();
                endQuestDialogue.dialogEvent.AddListener(() => EndQuest(false));
            }
        }

        void Start()
        {
            boxCollider = boxLoot.GetComponent<BoxCollider>();
            playerController = PlayerController.Instance;

            HandleLoad();
            HandleStartQuest();

            if (boxLoot != null)
            {
                boxCollider.enabled = false;

                if (boxLoot.items.Count > 0 && boxLoot.items[0].item.ID == bandage.ID)
                    boxLoot.items[0]?.onLootTaken.AddListener(() => AdvanceStage(false));
            }
        }
        /// <summary>
        /// specyficzna dla tego zadania funkcja rozpoczynajaca zadanie
        /// </summary>
        void HandleStartQuest()
        {
            if (state == QuestState.Inactive)
            {
                UnityEvent unityEvent = new UnityEvent();

                unityEvent.AddListener(StartQuestFromDefinition);

                dialoguesManager.SetOnStageEvent(unityEvent);
                playerController.RemoveWeapon(0);
                dialoguesManager.StartDialogue(dialogue.dialogueStages[0].texts);
            }
        }
        /// <summary>
        /// Zacznij zadanie
        /// </summary>
        public override void StartQuest(int questID)
        {
            base.StartQuest(questID);

            StartTutorial();
        }

        /// <summary>
        /// Akcja specyficzna dla tego zadania
        /// </summary>
        void StartTutorial()
        {
            goToPointInWorld.SetGoToPoint(true);
            dialogue.conversationStage = playerStartedDialgue;

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
        void EnterStage1(bool fromLoad) => StartTutorial();
        void ExitStage1(bool fromLoad) => SetMapMark(npcMapMark, true);

        // zombie zosta³y zabite, gracz wraca
        void EnterStage2(bool fromLoad)
        {
            if (!fromLoad)
                dialogue.IncreaseConversationStage();
            else
                dialogue.SetConversationStage(playerKiledZombies);
        }
        void ExitStage2(bool fromLoad) => SetMapMark(npcMapMark, false);

        // gracz wróci³ i musi isæ po banda¿e
        void EnterStage3(bool fromLoad)
        {
            SetMapMark(bandageMapMark, true);

            SetMapMark(npcMapMark, false);

            boxCollider.enabled = true;
            if (fromLoad)
            {
                DestroyZombies();
                dialogue.SetConversationStage(playerHasToGoAfterBandages);
            }
        }
        void ExitStage3(bool fromLoad)
        {
            SetMapMark(npcMapMark, true);
            SetMapMark(bandageMapMark, false);
        }
        // gracz wzi¹³ banda¿e, musi sprawdziæ co sie sta³o
        void EnterStage4(bool fromLoad)
        {
            if (eatingZombie != null)
                eatingZombie.SetActive(true);

            boxCollider.enabled = true;

            KillNpcAtDestination();

            // DŸwiêk tylko przy normalnym przejœciu, nie przy wczytaniu
            if (!fromLoad && screamSound != null)
                audioManager?.PlayClip(screamSound);

            dialogue.SetConversationStage(playerHasToGoAfterBandages);

            if (!fromLoad) return;

            DestroyZombies();
        }
        private void ExitStage4(bool fromLoad) { }

        // gracz zobaczy³ co sie sta³o
        void EnterStage5(bool fromLoad)
        {
            goToPointInWorld?.SetGoToPoint(false);
            dialogue.IncreaseConversationStage();

            boxCollider.enabled = true;

            if (!fromLoad) return;

            dialogue.SetConversationStage(endingDialogue);
            DestroyZombies();

            if (npc != null)
            {
                KillNpcAtDestination();
                dialogue.EndConversation();
            }
        }

        void ExitStage5(bool fromLoad) { }

        public override void EndQuest(bool fromLoad)
        {
            base.EndQuest(fromLoad);

            //npc umiera, koniec zadania
            if (npc != null)
                npc.GetComponent<AudioSource>()?.PlayOneShot(dyingSound);

            boxCollider.enabled = true;
            dialogue.EndConversation();

            SetMapMark(npcMapMark, false);
            SetMapMark(bandageMapMark, false);
            SetMapMark(NextSceneMapMark, true);

            if (!fromLoad) return;

            if (boxCollider != null)
                boxCollider.enabled = true;

            sceneTeleportManager.enabled = true;
            DestroyZombies();
        }
        void ExitEndQuest(bool fromLoad) { }
        public void IncreaseDeadEnemiesCounter()
        {
            deadEnemiesCounter++;

            questProgress = $"Zabij zombie {deadEnemiesCounter}/3";
            questManager.SetCustomQuestProgress(questProgress);

            if (deadEnemiesCounter == requireDeadEnemies)
                AdvanceStage();
        }
        void OnTriggerEnter(Collider other)
        {
            if (questStage != requireConversationStageToActivateOnTrigger) return;

            if (other.CompareTag("Player"))
                AdvanceStage();
        }
        void DestroyZombies()
        {
            foreach (ZombieController zombie in zombies)
                Destroy(zombie?.gameObject);

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

