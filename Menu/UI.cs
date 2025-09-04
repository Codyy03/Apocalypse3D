using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UI : MonoBehaviour
{
    public static UI Instance;

    readonly Color nativColor = new Color(1f, 0.984f, 0.902f, 1f); // #FFFBE6 
    readonly Color highlightColor = new Color(1f, 0.866f, 0.157f, 1f); // #FFDD28

    [Header("Napisy")]
    [SerializeField] List<TextMeshProUGUI> textsInMenu;

    [Header("Ekwipunek UI")]
    public ShowItemDescription itemDescription;
    [SerializeField] Inventory inventory;

    [Header("Szybki dostêp UI")]
    [SerializeField] SetMedicineFromInventory fastAccessMedicine;
    [SerializeField] FastAccessUi accessUi;

    [Header("DŸwiêki")]
    [SerializeField] AudioClip useMedicineSound;

    [Header("Powiadomienie")]
    [SerializeField] GameObject saveNotification;

    [Header("Celownik")]
    [SerializeField] GameObject crosshair;

    [Header("Niezbêdne klasy")]
    [SerializeField] GameManager gameManager;
    [SerializeField] PlayerHealth playerHealth;

    AudioManager audioManager;
    private void Awake()
    {
        Instance = this;
        audioManager = FindFirstObjectByType<AudioManager>();
    }
    private void Update()
    {
        if (itemDescription == null) return;

        HandleFastAccessMedicine();
    }

    void HandleFastAccessMedicine()
    {
        int itemId = itemDescription.itemIdInSlot;
        if (Input.GetKeyDown(KeyCode.T) && inventory.HowManyItems(itemId) > 0)
        {
            float currentMedicine = itemDescription.itemInSlot.health;
            inventory.ReduceItem(itemId, 1);

            playerHealth.ChangePlayerHealth(currentMedicine);

            accessUi.UpdateFastAccess(inventory.HowManyItems(itemId).ToString());

            audioManager.PlayClip(useMedicineSound);

            if (inventory.HowManyItemsInSlot(itemId) == 0)
            {
                fastAccessMedicine.UpdateFastAccess();
                accessUi.ResetValues();
                itemDescription = null;
            }
        }
    }
    public void HighlightText(TextMeshProUGUI text) => text.color = highlightColor;
    public void DisableHighlightText(TextMeshProUGUI text) => text.color = nativColor;

    private void OnEnable()
    {
        CrosshairController.CrosshairAction += SetCrosshairStatus;
    }
    private void OnDisable()
    {
        foreach (var text in textsInMenu)
        {
            text.color = nativColor;
        }

        CrosshairController.CrosshairAction -= SetCrosshairStatus;
    }
    /// <summary>
    /// ustawia widocznoœæ celownika: widoczny/niewidoczny
    /// </summary>
    /// <param name="status">widocznoœæ</param>
    void SetCrosshairStatus(bool status)
    {
        crosshair?.SetActive(status);
    }
    /// <summary>
    /// za³aduj menu g³ówne
    /// </summary>
    public void LoadMenu()
    {
        gameManager.DisableUIElement();
        gameManager.SetLoadedObjectsActivity(false);
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// wyjdŸ z gry
    /// </summary>
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
