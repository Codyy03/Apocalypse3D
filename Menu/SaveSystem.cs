using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;
using static Quests.Quest;

[Serializable]
public class InventorySlot
{
    public int slotID;
    public int slotAmount;
    public float slotDurability;
}
[Serializable]
public class InventoryData
{
    public List<InventorySlot> slots = new List<InventorySlot>();
    public int playerGold;
    public int mainWeaponFastAcessID;
    public int secondeaponFastAcessID;
    public int medicineFastAcessID;
    public int armorFastAccessID;
    public float armoarDurability;
}
[Serializable]
public class PlayerData
{
    public Vector3 position;
    public float health;
}
[Serializable]
public class AmmunitionData
{
    public int totalAmmunition;
    public int currentAmmunition;
}
[Serializable]
public class WeaponData
{
    public List<AmmunitionData> ammunitionDatas = new List<AmmunitionData>();
}
[Serializable]
public class LootSaveData
{
    public string sceneName;
    public string lootID;  // unikalny identyfikator obiektu lootu
    public List<ItemInLootData> items;
}

[Serializable]
public class ItemInLootData
{
    public int itemID;   // ID przedmiotu (z ScriptableObject)
    public int quantity; // ile sztuk
}
[Serializable]
public class QuestData
{
    public int questID;
    public int questStage;
    public bool isActive;
    public QuestState questState;
}

[Serializable]
public class QuestsSaveData
{
    public List<QuestData> questsData = new();
}

[Serializable]
public class GameData
{
    public InventoryData inventoryData;
    public PlayerData playerData;
    public WeaponData weaponData;
    public List<LootSaveData> lootSaveData;
    public QuestsSaveData questsData;
}
public class SaveSystem
{
    public void Save(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        File.WriteAllText(path, json);
    }
    public GameData LoadedData()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");

        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<GameData>(json);
    }
}
