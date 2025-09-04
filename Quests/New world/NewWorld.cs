using Dialogues;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static LootController;

namespace Quests
{
    public class NewWorld : Quest
    {
        readonly Vector3 positionAtTheGenerators = new Vector3(159.753998f, 0.0429992676f, 191.817993f);
        readonly Quaternion rotiationAtTheGenerators = new Quaternion(0, -180f, 0, 0);
        [Header("Kanistry benzyny")]
        [SerializeField] List<Loot> gasolineCanisters = new();

        [Header("Znaczniki na mapie")]
        [SerializeField] GameObject firstStageMapMark;
        [SerializeField] GameObject secondStageMapMark;
        [SerializeField] GameObject thirdStageMapMark;
        [SerializeField] GameObject fourStageMapMark;
        [SerializeField] GameObject sixStageMapMark;

        [Header("DŸwiêki")]
        [SerializeField] AudioClip fillingSound;
        [SerializeField] AudioSource generatorAudioSource;

        [Header("Dialogi")]
        [SerializeField] DialogueTextSO fillingGeneratorDialgue;
        [SerializeField] DialogueTextSO endStage3Dialogue;
        [SerializeField] DialogueTextSO giveAmmoDialogue;
        Dialogue dialogue;

        [Header("Miejsce odpoczynku")]
        [SerializeField] RestPlace restPlace;

        BasicMotionAnimations basicMotion;

        [Header("Amunicja do pistotu")]
        [SerializeField] Item ammoItem;
        Inventory invetory;

        [Header("Obiekt snajperki")]
        [SerializeField] GameObject sniperRfile;

        [Header("Elementy hordy")]
        [SerializeField] PlayerEnterPlace playerEnterPlace;
        [SerializeField] GameObject zombieHord;

        Transform player;
        protected override void Awake()
        {
            base.Awake();

            invetory = Inventory.Instance;
            player = PlayerController.Instance.transform;

            dialogue = GetComponent<Dialogue>();
            basicMotion = GetComponent<BasicMotionAnimations>();

            RegisterStage(1, s => SetMapMark(firstStageMapMark, true), s => SetMapMark(firstStageMapMark, false));
            RegisterStage(2, EnterStage2, ExitStage2);
            RegisterStage(3, EnterStage3, ExitStage3);
            RegisterStage(4, EnterStage4, ExitStage4);
            RegisterStage(5, EnterStage5, ExitStage5);
            RegisterStage(6, EnterStage6, ExitStage6);

            fillingGeneratorDialgue.dialogEvent.AddListener(StartFillingGenerators);
            endStage3Dialogue.dialogEvent.AddListener(() => AdvanceStage(false));
            giveAmmoDialogue.dialogEvent.AddListener(GiveAmmunition);
        }
        void Start()
        {
            if (state == QuestState.Inactive)
                StartCoroutine(WaitForFrameToStartQuest());
        }
        public override void StartQuest(int questID)
        {
            base.StartQuest(questID);
        }
        protected override void OnStageEnter(int s, bool fromLoad)
        {
            if (enter.TryGetValue(s, out var enterStage)) enterStage(fromLoad);
        }
        protected override void OnStageExit(int s, bool fromLoad)
        {
            if (exit.TryGetValue(s, out var exitStage)) exitStage(fromLoad);
        }

        void EnterStage2(bool fromLoad)
        {
            dialogue.IncreaseConversationStage();

            SetMapMark(secondStageMapMark, true);
            EnableGasolineLoot(() => AdvanceStage(false));

        }
        void ExitStage2(bool fromLoad) 
        {
            SetMapMark(secondStageMapMark, false);
        }

        void EnterStage3(bool fromLoad) 
        {
            SetMapMark(thirdStageMapMark, true);

            DisableGasolineLoot();

            basicMotion.ChangeAnimation(basicMotion.idle);
            transform.SetPositionAndRotation(positionAtTheGenerators, rotiationAtTheGenerators);

            dialogue.IncreaseConversationStage();
        }

        void ExitStage3(bool fromLoad)
        {
            SetMapMark(thirdStageMapMark, false);
            restPlace.onRestEvent += () => AdvanceStage(false);
        }
        void EnterStage4(bool fromLoad)
        {
            dialogue.IncreaseConversationStage();
            restPlace.enabled = true;
            SetMapMark(fourStageMapMark, true);
        }
        void ExitStage4(bool fromLoad)
        {
            DayNightSwitcher.Instance.SetNight(true);
            SetMapMark(fourStageMapMark, false);
        }

        void EnterStage5(bool fromLoad)
        {
            player.transform.localPosition = new Vector3(68.4967651f, -4.86099958f, 54.2391357f);
            player.transform.localRotation = new Quaternion(0, 0.135110334f, 0, 0.990830541f);

            transform.localPosition = new Vector3(-9f, -0.499000013f, -1.83500004f);
            transform.localRotation = new Quaternion(0f, 1f, 0f, 0f);

            dialogue.IncreaseConversationStage();

            dialogue.StartCurrentStageDialgue();

            sniperRfile.SetActive(true);

        }
        void ExitStage5(bool fromLoad)
        {
            SetMapMark(sixStageMapMark, true);
        }

        void EnterStage6(bool fromLoad)
        {
            playerEnterPlace.playerEnterField += StartHord;
        }
        void ExitStage6(bool fromLoad)
        {
        }
        public override void EndQuest(bool fromLoad)
        {
            base.EndQuest(fromLoad);
        }
        void EnableGasolineLoot(UnityAction action)
        {
            foreach (Loot loot in gasolineCanisters)
            {
                loot.gameObject.SetActive(true);
                foreach (ItemInLoot itemLoot in loot.items)
                    itemLoot.onLootTaken.AddListener(action);
            }
        }
        void DisableGasolineLoot()
        {
            foreach (var loot in gasolineCanisters)
                foreach (var itemLoot in loot.items)
                    itemLoot.onLootTaken.RemoveAllListeners();
        }

        public void StartFillingGenerators()
        {
            BlackScreenController.instance.ActivateBlackScreen(3f);
            StartGeneratorsSound();
            audioManager?.PlayClip(fillingSound);
        }

        public void StartGeneratorsSound() => generatorAudioSource.enabled = true;

        public void GiveAmmunition() => invetory.CreateManyItems(ammoItem, 60);

        void StartHord()
        {
            zombieHord.SetActive(true);
        }

        IEnumerator WaitForFrameToStartQuest()
        {
            yield return new WaitForEndOfFrame();
            StartQuest(questDefition.questID);
        }
    }
}