using Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LootController;

public class GameManager : MonoBehaviour
{
    public static bool UIElementIsOpen;

    [Serializable]
    public class UIElementsToOpen
    {
        public GameObject UIelement;
        public KeyCode inputKey;
    }

    [SerializeField] List<UIElementsToOpen> playerUIToOpen;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject panel;
    GameObject currentActiveElement;

    public GameData gameData = new GameData();

    Transform player;
    [SerializeField] PlayerHealth playerHealth;

    [SerializeField] Inventory inventory;

    [Header("Bronie do zapisu")]
    [SerializeField] HandgunScriptLPFP handgun;
    [SerializeField] AutomaticGunScriptLPFP assaultRifle;
    [SerializeField] SniperScriptLPFP sniper;
    SaveSystem saveSystem = new SaveSystem();

    [Header("Mapa")]
    [SerializeField] Camera mapCamera;
    [SerializeField] GameObject map;

    bool mapWasOpen;

    [SerializeField] QuestManager questManager;
    [SerializeField] PlayerController playerController;

    [SerializeField] GameObject playerLoaded;
    [SerializeField] GameObject UILoaded;
    [SerializeField] GameObject mapLoaded;
    private void Awake()
    {
        player = playerController.transform;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentActiveElement != null)
        {
            DisableUIElement();
            return;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && mapWasOpen)
        {
            DisableMap();
            return;
        }
        ElementsController();
        HandleMap();
    }
    /// <summary>
    /// kontroler elementów UI
    /// </summary>
    void ElementsController()
    {
        for (int i = 0; i < playerUIToOpen.Count; i++)
        {
            UIElementsToOpen bind = playerUIToOpen[i];
            if (Input.GetKeyDown(bind.inputKey))
            {
                // Czy panel ma byæ otwarty po naciœniêciu (toggle)?
                bool willOpen = !bind.UIelement.activeSelf;

                // Zamknij wszystkie panele
                foreach (UIElementsToOpen e in playerUIToOpen)
                    e.UIelement.SetActive(false);

                // Otwórz docelowy tylko jeœli ma byæ otwarty
                bind.UIelement.SetActive(willOpen);

                if (willOpen)
                {
                    currentActiveElement = bind.UIelement;
                    HandleCursorAndTime(CursorLockMode.None, 0f);
                    UIElementIsOpen = true;
                }
                else
                {
                    UIElementIsOpen = false;
                    currentActiveElement = null;
                    HandleCursorAndTime(CursorLockMode.Locked, 1f);
                }
                break;
            }
        }
    }
    /// <summary>
    /// zarz¹dza map¹
    /// </summary>
    void HandleMap()
    {
        if (Input.GetKeyDown(KeyCode.M) && !mapWasOpen)
        {
            map.gameObject.SetActive(true);
            mapCamera.enabled = true;
            HandleCursorAndTime(CursorLockMode.None, 0f);

            mapCamera.transform.position = new Vector3(player.position.x, mapCamera.transform.position.y, player.position.z);

            mapWasOpen = true;
            UIElementIsOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.M) && mapWasOpen)
        {
            DisableMap();
        }
    }
    /// <summary>
    /// wy³¹cz mape
    /// </summary>
    void DisableMap()
    {
        map.gameObject.SetActive(false);
        mapCamera.enabled = false;
        HandleCursorAndTime(CursorLockMode.Locked, 1f);

        UIElementIsOpen = false;
        mapWasOpen = false;
    }
    /// <summary>
    /// zarz¹dza stanem kursora i czasem
    /// </summary>
    /// <param name="cursor">status kursora</param>
    /// <param name="timeScale">czas</param>
    void HandleCursorAndTime(CursorLockMode cursor, float timeScale)
    {
        Cursor.lockState = cursor;

        Time.timeScale = timeScale;
    }
    /// <summary>
    /// wy³¹cz obecnie aktywny element UI
    /// </summary>
    public void DisableUIElement()
    {
        currentActiveElement?.SetActive(false);
        currentActiveElement = null;
        UIElementIsOpen = false;
        HandleCursorAndTime(CursorLockMode.Locked, 1f);
        settings.SetActive(false);
        panel.SetActive(true);
    }
    /// <summary>
    /// zapisz dane gracza
    /// </summary>
    public void SavePlayerStats()
    {
        PlayerData playerData = new PlayerData();
        AmmunitionData handgunData = new AmmunitionData();
        AmmunitionData assaultRifleData = new AmmunitionData();
        AmmunitionData sniperData = new AmmunitionData();

        playerData.position = player.position;

        playerData.health = playerHealth.GetHealth();

        gameData.playerData = playerData;

        handgunData.currentAmmunition = handgun.GetCurrentAmmo();
        handgunData.totalAmmunition = AmmunitionStorage.handgunAmmo;
        
        assaultRifleData.currentAmmunition = assaultRifle.GetCurrentAmmo();
        assaultRifleData.totalAmmunition = AmmunitionStorage.rifleAmmo;

        sniperData.currentAmmunition = sniper.GetCurrentAmmo();
        sniperData.totalAmmunition = AmmunitionStorage.sniperAmmo;

        WeaponData weaponData = new WeaponData();

        weaponData.ammunitionDatas.Add(handgunData);
        weaponData.ammunitionDatas.Add(assaultRifleData);
        weaponData.ammunitionDatas.Add(sniperData)
            ;
        gameData.weaponData = weaponData;
    }
    /// <summary>
    /// zapisz stan loot'u
    /// </summary>
    public void SaveLoot()
    {
        // Utwórz now¹ listê lootów
        gameData.lootSaveData = new List<LootSaveData>();
        string currentScene = SceneManager.GetActiveScene().name;
      
        foreach (Loot loot in FindObjectsByType<Loot>(FindObjectsSortMode.None))
        {
            LootSaveData data = new LootSaveData
            {
                sceneName = currentScene,
                lootID = loot.lootID,
                items = new List<ItemInLootData>()
            };

            foreach (ItemInLoot slot in loot.items)
            {
                data.items.Add(new ItemInLootData
                {
                    itemID = slot.item.ID,
                    quantity = slot.quantity
                });
            }

            gameData.lootSaveData.Add(data); // dodajemy ka¿dy loot do listy
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Dostosuj tylko do scen gry, nie do menu
        if (scene.name == "Beginning" || scene.name == "CityRoad")
        {
            if (scene.name == "Beginning")
                player.position = new Vector3(-33.2999992f, 6.16099977f, 87.6100006f);

           if (scene.name == "CityRoad")
                player.position = new Vector3(180.809998f, 1.60000002f, 28.7999992f);

            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            UIElementIsOpen = false;
            SetLoadedObjectsActivity(true);
            if (Menu.loadGame)
            {
                LoadGame();
                Menu.loadGame = false; // ¿eby nie wczytywa³o w kó³ko
            }
        }
    }
    public void SetLoadedObjectsActivity(bool activity)
    {
        playerLoaded.SetActive(activity);
        UILoaded.SetActive(activity);
        mapLoaded.SetActive(activity);

        DisableUIElement();
    }
    /// <summary>
    /// zapisz gre
    /// </summary>
    public void SaveGame()
    {
        inventory.SaveInventory();
        SavePlayerStats();
        SaveLoot();

        gameData.questsData = questManager.SaveQuests();

        saveSystem.Save(gameData);
    }
    /// <summary>
    /// wczytaj grê
    /// </summary>
    public void LoadGame()
    {
        gameData = saveSystem.LoadedData();

        if (gameData == null) return;

        inventory.LoadInventory();

        player.position = gameData.playerData.position;
        playerHealth.SetHealth(gameData.playerData.health);

        handgun.LoadAmmo(gameData.weaponData.ammunitionDatas[0].currentAmmunition, gameData.weaponData.ammunitionDatas[0].totalAmmunition);

        assaultRifle.LoadAmmo(gameData.weaponData.ammunitionDatas[1].currentAmmunition, gameData.weaponData.ammunitionDatas[1].totalAmmunition);

        sniper.LoadAmmo(gameData.weaponData.ammunitionDatas[2].currentAmmunition, gameData.weaponData.ammunitionDatas[2].totalAmmunition);

        if (playerController.noWeapons)
            playerController.GetComponentInChildren<Gun>().HideWeapon();

        LoadLoot();

        questManager.LoadQuests(gameData.questsData);
    }
    /// <summary>
    /// wczytaj stan loot'u
    /// </summary>
    public void LoadLoot()
    {
        string currentActiveName = SceneManager.GetActiveScene().name;

        var lootMap = gameData.lootSaveData
        .Where(l => l.sceneName == currentActiveName)
        .ToDictionary(l => l.lootID, l => l);

        foreach (Loot loot in FindObjectsByType<Loot>(FindObjectsSortMode.None))
        {
            if (lootMap.TryGetValue(loot.lootID, out var saved))
            {
                if (saved == null) continue;

                loot.items.Clear();

                foreach (ItemInLootData slot in saved.items)
                {
                    Item baseItem = inventory.GetItem(slot.itemID);
                    if (baseItem == null) continue;
                    loot.items.Add(new ItemInLoot
                    {
                        item = baseItem,
                        quantity = slot.quantity
                    });
                }

                if (loot.items.Count == 0 && loot.destroy)
                    Destroy(loot.gameObject);
                else if (loot.items.Count == 0)
                    loot.gameObject.tag = "Untagged";
            }
        }
    }
}


