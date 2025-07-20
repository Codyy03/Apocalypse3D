using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
public class UI : MonoBehaviour
{
    [Header("Napisy")]
    [SerializeField] List<TextMeshProUGUI> textsInMenu;

    readonly Color nativColor = new Color(1f, 0.984f, 0.902f, 1f); // #FFFBE6 
    readonly Color highlightColor = new Color(1f, 0.866f, 0.157f, 1f); // #FFDD28

    public ShowItemDescription itemDescription;

    [SerializeField] SetMedicineFromInventory fastAccessMedicine;
    [SerializeField] FastAccessUi accessUi;
    [SerializeField] AudioClip useMedicineSound;

    [SerializeField] GameObject saveNotification;
    [SerializeField] Inventory inventory;
    [SerializeField] PlayerHealth playerHealth;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }
    private void Update()
    {
        if (itemDescription.itemInSlot == null) return;

        int itemId = itemDescription.itemIdInSlot;

        if (Input.GetKeyDown(KeyCode.T) && inventory.HowManyItems(itemId) > 0)
        {
            inventory.ReduceItem(itemId, 1);

            accessUi.UpdateFastAccess(inventory.HowManyItems(itemId).ToString());

            playerHealth.ChangePlayerHealth(itemDescription.itemInSlot.health);
            audioManager.PlayClip(useMedicineSound);
        }
        else if (inventory.HowManyItemsInSlot(itemId) == 0)
        {
            fastAccessMedicine.UpdateFastAccess();
            accessUi.ResetValues();
        }
    }
    public void HighlightText(TextMeshProUGUI text) => text.color = highlightColor;
    public void DisableHighlightText(TextMeshProUGUI text) => text.color = nativColor;
    private void OnDisable()
    {
        foreach (var text in textsInMenu)
        {
            text.color = nativColor;
        }
    }
    public void LoadMenu() => SceneManager.LoadScene(0);
    public void ExitGame() => Application.Quit();

    public void ShowNotification()
    {
        saveNotification.SetActive(true);
        StartCoroutine(DisableNotification());
    }

    IEnumerator DisableNotification()
    {
        yield return new WaitForSeconds(1f);
        saveNotification.SetActive(false);
    }

}
