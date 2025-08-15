using Quests;
using System;
using System.Collections.Generic;
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
    GameObject currentActiveElement;

    public GameData gameData = new GameData();

    Transform player;
    PlayerHealth playerHealth;

    Inventory inventory;

    [Header("Bronie do zapisu")]
    [SerializeField] HandgunScriptLPFP handgun;
    [SerializeField] AutomaticGunScriptLPFP assaultRifle;
    SaveSystem saveSystem = new SaveSystem();

    [Header("Mapa")]
    [SerializeField] Camera mapCamera;
    [SerializeField] GameObject map;

    bool mapWasOpen;

    QuestManager questManager;
    PlayerController playerController;
    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        player = playerController.transform;
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        inventory = FindFirstObjectByType<Inventory>();
        questManager = FindFirstObjectByType<QuestManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && currentActiveElement != null)
        {
            currentActiveElement.SetActive(false);
            currentActiveElement = null;
            UIElementIsOpen = false;
            HandleCursorAndTime(CursorLockMode.Locked, 1f);
            return;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && mapWasOpen)
        {
            DisableMap();
            return;
        }
        for (int i = 0; i < playerUIToOpen.Count; i++)
        {
            UIElementsToOpen bind = playerUIToOpen[i];
            if (Input.GetKeyDown(bind.inputKey))
            {
                // Czy panel ma by� otwarty po naci�ni�ciu (toggle)?
                bool willOpen = !bind.UIelement.activeSelf;

                // Zamknij wszystkie panele
                foreach (UIElementsToOpen e in playerUIToOpen)
                    e.UIelement.SetActive(false);

                // Otw�rz docelowy tylko je�li ma by� otwarty
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
        HandleMap();
    }

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
    void DisableMap()
    {
        map.gameObject.SetActive(false);
        mapCamera.enabled = false;
        HandleCursorAndTime(CursorLockMode.Locked, 1f);

        UIElementIsOpen = false;
        mapWasOpen = false;
    }
    void HandleCursorAndTime(CursorLockMode cursor, float timeScale)
    {
        Cursor.lockState = cursor;

        Time.timeScale = timeScale;
    }
    public void SavePlayerStats()
    {
        PlayerData playerData = new PlayerData();
        AmmunitionData handgunData = new AmmunitionData();
        AmmunitionData assaultRifleData = new AmmunitionData();

        playerData.position = player.position;

        playerData.health = playerHealth.GetHealth();

        gameData.playerData = playerData;

        handgunData.currentAmmunition = handgun.GetCurrentAmmo();
        handgunData.totalAmmunition = AmmunitionStorage.handgunAmmo;
        
        assaultRifleData.currentAmmunition = assaultRifle.GetCurrentAmmo();
        assaultRifleData.totalAmmunition = AmmunitionStorage.rifleAmmo;

        WeaponData weaponData = new WeaponData();

        weaponData.ammunitionDatas.Add(handgunData);
        weaponData.ammunitionDatas.Add(assaultRifleData);

        gameData.weaponData = weaponData;
    }

    public void SaveLoot()
    {
        // Utw�rz now� list� loot�w
        gameData.lootSaveData = new List<LootSaveData>();

        foreach (Loot loot in FindObjectsByType<Loot>(FindObjectsSortMode.None))
        {
            LootSaveData data = new LootSaveData
            {
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

            gameData.lootSaveData.Add(data); // dodajemy ka�dy loot do listy
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        // Tutaj wklejasz logik� z OnLevelWasLoaded
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        UIElementIsOpen = false;

        if (Menu.loadGame)
            LoadGame();
    }

    public void SaveGame()
    {
        inventory.SaveInventory();
        SavePlayerStats();
        SaveLoot();

        gameData.questsData = questManager.SaveQuests();

        saveSystem.Save(gameData);
    }

    public void LoadGame()
    {
        gameData = saveSystem.LoadedData();

        if (gameData == null) return;

        inventory.LoadInventory();

        player.position = gameData.playerData.position;
        playerHealth.SetHealth(gameData.playerData.health);

        handgun.LoadAmmo(gameData.weaponData.ammunitionDatas[0].currentAmmunition, gameData.weaponData.ammunitionDatas[0].totalAmmunition);

        assaultRifle.LoadAmmo(gameData.weaponData.ammunitionDatas[1].currentAmmunition, gameData.weaponData.ammunitionDatas[1].totalAmmunition);

        if (playerController.noWeapons)
            playerController.GetComponentInChildren<Gun>().HideWeapon();

        LoadLoot();

        questManager.LoadQuests(gameData);
    }


    public void LoadLoot()
    {
        foreach (Loot loot in FindObjectsByType<Loot>(FindObjectsSortMode.None))
        {
            LootSaveData saved = gameData.lootSaveData.Find(l => l.lootID == loot.lootID);
            if (saved == null) continue;

            loot.items.Clear();

            foreach (ItemInLootData slot in saved.items)
            {
                Item baseItem = inventory.ReturnItem(slot.itemID);
                loot.items.Add(new ItemInLoot
                {
                    item = baseItem,
                    quantity = slot.quantity
                });
            }

            if (loot.items.Count == 0 && loot.destroy)
                Destroy(loot.gameObject);
            else if(loot.items.Count == 0)
                loot.gameObject.tag = "Untagged";
        }
    }
}


