using Knife.RealBlood.SimpleController;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Dialogue;

public class DialoguesManager : MonoBehaviour
{
    public string currentText;

    [Header("Dialogi UI")]
    [SerializeField] GameObject dialogueMenu;
    [SerializeField] GameObject dialogueChoicesContener;
    [SerializeField] GameObject dialogueChoiceButton;
    [SerializeField] TextMeshProUGUI dialogueText;

    public List<DialogueText> currentDialogue = new();
    public static bool dialogueIsActive;
    int currentTextIndex;

    PlayerController playerController;
    GameObject firstButton = null;

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (!dialogueIsActive) return;

        Cursor.lockState = CursorLockMode.None;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentTextIndex >= currentDialogue.Count)
            {
                EndDialogue();
                return;
            }

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

    public void StartDialogue(List<DialogueText> texts)
    {
        currentDialogue = new List<DialogueText>();

        foreach (DialogueText original in texts)
        {
            DialogueText copy = new DialogueText
            {
                dialogue = original.dialogue,
                dialogEvent = original.dialogEvent, // zostaje orygina³!
                getDialogueChoices = original.getDialogueChoices,
                dialogsChoices = new List<DialogueChoice>()
            };

            foreach (DialogueChoice originalChoice in original.dialogsChoices)
            {
                DialogueChoice choiceCopy = new DialogueChoice
                {
                    text = originalChoice.text,
                    nextDialogueIndex = originalChoice.nextDialogueIndex,
                    endsDialogue = originalChoice.endsDialogue,
                    onChoiceSelected = originalChoice.onChoiceSelected // równie¿ zostaje!
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

    public void NextDialogue()
    {
        RemoveChoiceButtons();

        currentTextIndex++;
        if (currentTextIndex >= currentDialogue.Count)
        {
            EndDialogue();
            return;
        }

        ShowDialogue(currentDialogue[currentTextIndex]);
    }

    void ShowDialogue(DialogueText textData)
    {
        currentText = textData.dialogue;
        dialogueText.text = currentText;

        textData.dialogEvent?.Invoke();
        RemoveChoiceButtons();

        List<DialogueChoice> dialogueChoices = new();
        if (textData.getDialogueChoices != -1)
        {
            dialogueChoices = currentDialogue[textData.getDialogueChoices].dialogsChoices;
        }
        else if (textData.dialogsChoices != null && textData.dialogsChoices.Count > 0)
        {
            dialogueChoices = textData.dialogsChoices;
        }


        foreach (DialogueChoice choice in dialogueChoices)
        {
            GameObject buttonObj = Instantiate(dialogueChoiceButton, dialogueChoicesContener.transform);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            // Zapisz pierwszy wygenerowany przycisk
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
        // Ustaw focus dopiero po wygenerowaniu wszystkich przycisków
        if (firstButton != null)
            StartCoroutine(SetFocusNextFrame(firstButton));

    }

    void RemoveChoiceButtons()
    {
        foreach (Transform child in dialogueChoicesContener.transform)
        {
            Destroy(child.gameObject);

        }
        firstButton = null;
    }

    public void EndDialogue()
    {
        RemoveChoiceButtons();
        dialogueText.text = "";
        currentTextIndex = 0;
        currentDialogue.Clear();
        dialogueMenu.SetActive(false);


        // Przywróæ broñ
        playerController.lastSelectedWeapon.currentWeapon.weapon.GetComponentInChildren<Gun>().ShowWeapon();
        StartCoroutine(WaitToDisableDialog());
    }

    IEnumerator WaitToDisableDialog()
    {
        yield return new WaitForSeconds(0.3f);
        dialogueIsActive = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    IEnumerator SetFocusNextFrame(GameObject button)
    {
        yield return null; // poczekaj 1 frame
        EventSystem.current.SetSelectedGameObject(button);
    }
}