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
    AudioManager audioManager;
    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }
    private void Update()
    {
        // Jeœli gracz nie jest w zasiêgu lub brak loot, przerwij

        if (!playerInItemSphere || loot == null) return;

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
            while (loot.items.Count > 0)
            {
                var currentLoot = loot.items[0];
                inventory.CreateManyItems(currentLoot.item, currentLoot.quantity);
                loot.items.RemoveAt(0);
                Destroy(lootController.contentParent.GetChild(0).gameObject);
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
    public void TakeOneItem()
    {
        inventory.CreateManyItems(loot.items[currentChild].item, loot.items[currentChild].quantity);
        loot.items.RemoveAt(currentChild);
        Destroy(lootController.contentParent.GetChild(currentChild).gameObject);
        howManyItems--;
        audioManager.PlayClip(takeSound);

        if (howManyItems == 0)
            TookItems();
    }
    void TookItems()
    {
        playerInItemSphere = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (loot != null && loot.destroy)
            Destroy(loot.gameObject);
        else if(loot != null && !loot.destroy)
        {
            loot.gameObject.tag = "Untagged";
            loot.transform.GetChild(0).gameObject.SetActive(false);
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
            currentInteraction.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactive") && currentInteraction != null )
            DisableLootUI();
    }
    void DisableLootUI()
    {
        int childCount = lootController.contentParent.childCount;

        for (int i = 0; i < childCount; i++)
            Destroy(lootController.contentParent.GetChild(0).gameObject);

        loot = null;
        playerInItemSphere = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentInteraction?.SetActive(false);
        currentInteraction = null;
        lootController.lootPanel.SetActive(false);
        lootIsOpen = false;

    }
}
