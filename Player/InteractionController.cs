using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static LootController;
public class InteractionController : MonoBehaviour
{
    public static bool lootIsOpen;
    public static int currentChild;
    public float maxInteracionDistance;

    [Header("DŸwiêki")]
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip takeSound;
    [SerializeField] AudioClip takeAllSound;
    [SerializeField] AudioClip closeSound;

    GameObject currentInteraction;

    public bool playerInItemSphere;
    [SerializeField] LootController lootController;
    public Loot loot;

    int howManyItems;
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject pickupPromptUI;
    AudioManager audioManager;
    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }
    private void Update()
    {
        // Jeœli gracz nie jest w zasiêgu lub brak loot, przerwij

        if (!playerInItemSphere || loot == null || Dialogues.DialoguesManager.dialogueIsActive) return;

        // Otwieranie panelu lootu
        if (Input.GetKeyDown(KeyCode.E) && !lootController.lootPanel.activeInHierarchy)
        {
            lootController.ShowLoot(loot.items);
            howManyItems = loot.items.Count;
            Cursor.lockState = CursorLockMode.None;
            lootIsOpen = true;
            audioManager.PlayClip(openSound);
            lootController.contentParent.GetChild(0).transform.GetChild(4).gameObject.SetActive(true);
        }

        // WeŸ wszystko klawiszem X
        else if (Input.GetKeyDown(KeyCode.X) && lootController.lootPanel.activeInHierarchy)
        {
            int i = 0;

            while (loot.items.Count > 0)
            {
                var currentLoot = loot.items[0];
                inventory.CreateManyItems(currentLoot.item, currentLoot.quantity);

                loot.items[i].onLootTaken?.Invoke();

                loot.items.RemoveAt(0);
                Destroy(lootController.contentParent.GetChild(0).gameObject);

                i++;
            }
            audioManager.PlayClip(takeAllSound);
            TookItems();
        }

        // Pobieranie jednego przedmiotu klawiszem E
        else if (Input.GetKeyDown(KeyCode.E) && lootController.lootPanel.activeInHierarchy && howManyItems > 0)
        {
            TakeOneItem();
           
            if(howManyItems > 0)
              lootController.contentParent.GetChild(1).transform.GetChild(4).gameObject.SetActive(true);
        }

        // Wyjœcie z panelu lootu ESC
        else if (Input.GetKeyDown(KeyCode.Escape) && lootController.lootPanel.activeInHierarchy)
        {
            audioManager.PlayClip(closeSound);
            DisableLootUI();
        }
    }

    /// <summary>
    /// Bierze obecenie zaznaczony loot z wyœwietlanej listy
    /// </summary>
    public void TakeOneItem()
    {
        inventory.CreateManyItems(loot.items[currentChild].item, loot.items[currentChild].quantity);

        loot.items[currentChild].onLootTaken?.Invoke();

        loot.items.RemoveAt(currentChild);
        
        Destroy(lootController.contentParent.GetChild(currentChild).gameObject);
        howManyItems--;
        
        audioManager.PlayClip(takeSound);

        if (howManyItems == 0)
            TookItems();
    }
    /// <summary>
    /// Bierze wszystkie przedmioty z listy wyœwietlanego lootu
    /// </summary>
    void TookItems()
    {
        playerInItemSphere = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (loot != null && loot.destroy)
            Destroy(loot.gameObject);
        else if(loot != null && !loot.destroy)
        {
            loot.gameObject.tag = "Untagged";
            pickupPromptUI.SetActive(false);
        }

        lootController.lootPanel.SetActive(false);
        loot = null;
        lootIsOpen = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactive"))
        {
            GameObject newInteraction = other.transform.GetChild(0).gameObject;

            if (currentInteraction != newInteraction)
            {
                if (currentInteraction != null )
                {
                    if (currentInteraction.gameObject != null) // sprawdza czy referencja nie zosta³a zniszczona
                        currentInteraction.SetActive(false);
                }
                currentInteraction = newInteraction;
            }

            playerInItemSphere = true;
            loot = currentInteraction.GetComponentInParent<Loot>();
            
            PickupPrompt pickupPrompt = pickupPromptUI.GetComponent<PickupPrompt>();

            pickupPrompt.targetWorld = currentInteraction.transform;
            pickupPromptUI.SetActive(true);
        }
    }
    /// <summary>
    /// Je¿eli gracz jest za daleko wy³¹cza UI lootu
    /// </summary>
    /// <param name="other">Obecnie otwarty obiekt</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactive") && currentInteraction != null )
            DisableLootUI();
    }
    /// <summary>
    /// Wy³¹cza wszystkie elementy UI zwiazane z lootem
    /// </summary>
    void DisableLootUI()
    {
        int childCount = lootController.contentParent.childCount;

        for (int i = 0; i < childCount; i++)
            Destroy(lootController.contentParent.GetChild(0).gameObject);

        playerInItemSphere = false;
        lootIsOpen = false;

        Cursor.lockState = CursorLockMode.Locked;

        pickupPromptUI.SetActive(false);
        lootController.lootPanel.SetActive(false);

        loot = null;
        currentInteraction = null;

    }
}
