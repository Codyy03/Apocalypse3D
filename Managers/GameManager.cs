using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using static LootController;

public class GameManager : MonoBehaviour
{
    public static bool UIElementIsOpen;
    [SerializeField] List<GameObject> playerUIToOpen;

    public GameData gameData = new GameData();

    Transform player;
    PlayerHealth playerHealth;

    Inventory inventory;

    [Header("Bronie do zapisu")]
    [SerializeField] HandgunScriptLPFP handgun;
    [SerializeField] AutomaticGunScriptLPFP assaultRifle;
    SaveSystem saveSystem = new SaveSystem();

    List<LootSaveData> savedLoots = new();

    [SerializeField] Camera mapCamera;
    Camera playerCamera;
    bool mapWasOpen;
    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>().transform;
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        inventory = FindFirstObjectByType<Inventory>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !InteractionController.lootIsOpen)
        {
            UIElementIsOpen = false;
            Time.timeScale = 1.0f;

            for (int i = 0; i < playerUIToOpen.Count; i++)
            {
                if (playerUIToOpen[i] != null && playerUIToOpen[i].activeInHierarchy)
                {
                    DisableAllUiCanvasElements();

                    return;
                }
            }

            ActivateObject(playerUIToOpen[1]);
        }

        // element 0 - ekwipunek
        // element 1 - menu
        // elemeny 2 - mapa
        // element 3 - misje

        if (Input.GetKeyDown(KeyCode.I))
            ActivateObject(playerUIToOpen[0]);

        if (Input.GetKeyDown(KeyCode.M) && !mapWasOpen)
        {
            ActivateObject(playerUIToOpen[2]);

            playerCamera = Camera.main;
            mapCamera.enabled = true;
            mapCamera.transform.position = new Vector3(player.position.x, mapCamera.transform.position.y, player.position.z);
          //  playerCamera.gameObject.SetActive(false);
            mapWasOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.M) && mapWasOpen)
        {
            ActivateObject(playerUIToOpen[2]);
           // playerCamera.gameObject.SetActive(true);
            mapWasOpen = false;
            mapCamera.enabled = false;
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            ActivateObject(playerUIToOpen[3]);
        }
    }

    public void ActivateObject(GameObject o)
    {
        if (o == null) return;

        if (!o.activeInHierarchy)
        {
            DisableAllUiCanvasElements();
            
            o.SetActive(true);

            Time.timeScale = 0;
            
            Cursor.lockState = CursorLockMode.None;

            UIElementIsOpen = true;
            return;
        }
        else if (o.activeInHierarchy)
        {
            DisableAllUiCanvasElements();
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            UIElementIsOpen = false;
            return;
        }
    }
    // zamyka wszystkie obiekty UI, które mozna otworzyc za pomoca klawiatury
    void DisableAllUiCanvasElements()
    {
        foreach (GameObject elements in playerUIToOpen)
        {
            if (elements != null)
                elements.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
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

    [System.Obsolete]
    public void SaveLoot()
    {
        // Utwórz now¹ listê lootów
        gameData.lootSaveData = new List<LootSaveData>();

        foreach (Loot loot in FindObjectsOfType<Loot>())
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

            gameData.lootSaveData.Add(data); // dodajemy ka¿dy loot do listy
        }
    }


    private void OnLevelWasLoaded(int level)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        UIElementIsOpen = false;
    }

    [System.Obsolete]
    public void SaveGame()
    {
        inventory.SaveInventory();
        SavePlayerStats();
        SaveLoot();
        saveSystem.Save(gameData);
    }

    [System.Obsolete]
    public void LoadGame()
    {
        gameData = saveSystem.LoadedData();

        if (gameData == null) return;

        inventory.LoadInventory();

        player.position = gameData.playerData.position;
        playerHealth.SetHealth(gameData.playerData.health);

        handgun.LoadAmmo(gameData.weaponData.ammunitionDatas[0].currentAmmunition, gameData.weaponData.ammunitionDatas[0].totalAmmunition);

        assaultRifle.LoadAmmo(gameData.weaponData.ammunitionDatas[1].currentAmmunition, gameData.weaponData.ammunitionDatas[1].totalAmmunition);

        LoadLoot();
    }

    [System.Obsolete]
    public void LoadLoot()
    {
        foreach (Loot loot in FindObjectsOfType<Loot>())
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


