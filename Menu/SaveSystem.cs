using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;

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
[System.Serializable]
public class LootSaveData
{
    public string lootID;            // unikalny identyfikator obiektu lootu
    public List<ItemInLootData> items;
}

[System.Serializable]
public class ItemInLootData
{
    public int itemID;               // ID przedmiotu (z ScriptableObject)
    public int quantity;             // ile sztuk
}
[Serializable]
public class GameData
{
    public InventoryData inventoryData;
    public PlayerData playerData;
    public WeaponData weaponData;
    public List<LootSaveData> lootSaveData;
}
public class SaveSystem
{
    public void Save(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(Application.dataPath + "/save.json", json);

    }
    public GameData LoadedData()
    {
        string path = Application.dataPath + "/save.json";

        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);

        GameData data = JsonUtility.FromJson<GameData>(json);

        return data;
    }
}
