using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Dialogues.Dialogue;

namespace Dialogues
{
    public class DialoguesManager : MonoBehaviour
    {
        public static DialoguesManager Instance;
        public static bool dialogueIsActive;
        public string currentText;

        [Header("Dialogi UI")]
        [SerializeField] GameObject dialogueMenu;
        [SerializeField] GameObject dialogueChoicesContener;
        [SerializeField] GameObject dialogueChoiceButton;
        [SerializeField] TextMeshProUGUI dialogueText;

        public List<DialogueTextSO> currentDialogue = new();
        int currentTextIndex;

        [SerializeField] PlayerController playerController;
        Transform currentUsedWeapon;

        [SerializeField] GameObject ui;

        GameObject firstButton = null;

        UnityEvent onStageEnd;

        private void Awake()
        {
            Instance = this;
        }
        void Update()
        {
            if (!dialogueIsActive) return;
            if (BlackScreenController.instance.blackScreenIsActive) return;

            Cursor.lockState = CursorLockMode.None;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currentTextIndex >= currentDialogue.Count)
                {
                    EndDialogue();
                    return;
                }

                if (currentDialogue[currentTextIndex].dialogsChoices.Count == 0 && currentDialogue[currentTextIndex].returnToNode == null)
                    NextDialogue();

            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    var button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                    button?.onClick.Invoke();
                }
            }
        }
        /// <summary>
        /// Rozpocznij dialog
        /// </summary>
        /// <param name="texts">lista dialogów</param>
        public void StartDialogue(List<DialogueTextSO> texts)
        {
            if (dialogueIsActive) return;

            ui.SetActive(false);

            for (int i = 0; i < playerController.transform.childCount; i++)
            {
                Transform currentChild = playerController.transform.GetChild(i);
                if (currentChild.gameObject.activeSelf)
                {
                    currentUsedWeapon = currentChild;
                    break;
                }
            }
            currentUsedWeapon.rotation = new Quaternion(0, 0, 0, 0);
            currentDialogue = new List<DialogueTextSO>();

            foreach (DialogueTextSO original in texts)
            {
                DialogueTextSO copy = new DialogueTextSO
                {
                    dialogue = original.dialogue,
                    returnToNode = original.returnToNode,
                    dialogEvent = original.dialogEvent,
                    dialogsChoices = new List<DialogueChoice>()
                };

                foreach (DialogueChoice originalChoice in original.dialogsChoices)
                {
                    DialogueChoice choiceCopy = new DialogueChoice
                    {
                        text = originalChoice.text,
                        nextDialogueIndex = originalChoice.nextDialogueIndex,
                        endsDialogue = originalChoice.endsDialogue,
                        onChoiceSelected = originalChoice.onChoiceSelected
                    };

                    copy.dialogsChoices.Add(choiceCopy);
                }

                currentDialogue.Add(copy);
            }

            currentTextIndex = 0;
            dialogueIsActive = true;
            dialogueMenu.SetActive(true);

            // Schowaj broñ
            playerController.lastSelectedWeapon.currentWeapon.weapon.GetComponentInChildren<Gun>().HideWeapon();

            ShowDialogue(currentDialogue[currentTextIndex]);
        }
        /// <summary>
        /// wyœwietla kolejny dialog lub zakañcza gdy jest ostatni
        /// </summary>
        public void NextDialogue()
        {
            RemoveChoiceButtons();

            currentTextIndex++;
            if (currentTextIndex >= currentDialogue.Count)
            {
                EndDialogue();
                return;
            }
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject);

            ShowDialogue(currentDialogue[currentTextIndex]);
        }
        /// <summary>
        /// pokazuje obecny dialog
        /// </summary>
        /// <param name="textData">dialog do wyœwietlania</param>
        void ShowDialogue(DialogueTextSO textData)
        {
            currentText = textData.dialogue;
            dialogueText.text = currentText;

            textData.dialogEvent?.Invoke();
            RemoveChoiceButtons();

            List<DialogueChoice> dialogueChoices = new();
            List<GameObject> createdButtons = new();

            if (textData.returnToNode != null)
            {
                dialogueChoices = textData.returnToNode.dialogsChoices;
            }
            else if (textData.dialogsChoices != null && textData.dialogsChoices.Count > 0)
            {
                dialogueChoices = textData.dialogsChoices;
            }

            foreach (DialogueChoice choice in dialogueChoices)
            {
                GameObject buttonObj = Instantiate(dialogueChoiceButton, dialogueChoicesContener.transform);
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;

                // Dodaj do listy przycisków (do nawigacji)
                createdButtons.Add(buttonObj);

                if (firstButton == null)
                    firstButton = buttonObj;

                buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    choice.onChoiceSelected?.Invoke();
                    RemoveChoiceButtons();

                    if (choice.endsDialogue || choice.nextDialogueIndex < 0)
                    {
                        EndDialogue();
                    }
                    else if (choice.nextDialogueIndex < currentDialogue.Count)
                    {
                        currentTextIndex = choice.nextDialogueIndex;
                        ShowDialogue(currentDialogue[currentTextIndex]);
                    }
                    else
                    {
                        Debug.LogWarning("B³êdny indeks dialogu: " + choice.nextDialogueIndex);
                        EndDialogue();
                    }
                });
            }

            // Ustaw focus na pierwszy przycisk
            if (firstButton != null)
                StartCoroutine(SetFocusNextFrame(firstButton));

            // Ustaw rêcznie nawigacjê miêdzy przyciskami
            for (int i = 0; i < createdButtons.Count; i++)
            {
                Navigation nav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = i > 0 ? createdButtons[i - 1].GetComponent<Button>() : null,
                    selectOnDown = i < createdButtons.Count - 1 ? createdButtons[i + 1].GetComponent<Button>() : null
                };

                createdButtons[i].GetComponent<Button>().navigation = nav;
            }
        }
        /// <summary>
        /// usuwa przyciski opcji dialogowych
        /// </summary>
        void RemoveChoiceButtons()
        {
            foreach (Transform child in dialogueChoicesContener.transform)
            {
                Destroy(child.gameObject);
            }
            firstButton = null;
        }
        /// <summary>
        /// koñczy obecny dialog
        /// </summary>
        public void EndDialogue()
        {
            RemoveChoiceButtons();
            dialogueText.text = "";
            currentTextIndex = 0;
            currentDialogue.Clear();
            dialogueMenu.SetActive(false);

            onStageEnd?.Invoke();

            // Przywróæ broñ
            if (!playerController.noWeapons)
                playerController.lastSelectedWeapon.currentWeapon.weapon.GetComponentInChildren<Gun>().ShowWeapon();

            StartCoroutine(WaitToDisableDialog());
        }
        IEnumerator WaitToDisableDialog()
        {
            yield return new WaitForEndOfFrame();
            dialogueIsActive = false;
            Cursor.lockState = CursorLockMode.Locked;
            ui.SetActive(true);
        }
        IEnumerator SetFocusNextFrame(GameObject button)
        {
            yield return null; // poczekaj 1 frame
            EventSystem.current.SetSelectedGameObject(button);
        }

        public void SetOnStageEvent(UnityEvent unityEvent) => onStageEnd = unityEvent;
    }
}