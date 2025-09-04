using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Serializable]
    // klasa reprezentuj¹ca slot
    public class Slot
    {
        public TextMeshProUGUI slotAmountText;
        public GameObject inventorySlot;
        public Item item;
        public int slotID;
        public int slotAmount;
        public float durability;
        // przypisuje wartosc ilosc elementow w slocie jako text
        public void DisplaySlotAmount()
        {
            if (slotAmount == 0) slotAmountText.transform.parent.gameObject.SetActive(false);

            if (slotAmount > 0) slotAmountText.transform.parent.gameObject.SetActive(true);

            slotAmountText.text = slotAmount.ToString();
        }
        public Slot Copy()
        {
            return new Slot
            {
                inventorySlot = this.inventorySlot,
                item = this.item,
                slotID = this.slotID,
                slotAmount = this.slotAmount
            };

        }
    }
    [SerializeField] Sprite nullSprite;
    [SerializeField] TextMeshProUGUI goldAmount;

    [Header("Sloty na przedmioty")]
    public Slot[] slots;

    [Header("Wszystkie dostêpne przedmioty")]
    public Item[] items;

    public int playerGold = 0;

    List<ShowItemDescription> itemsDescription = new List<ShowItemDescription>();
    bool itemAddToSlot;

    List<Slot> slotsToSort = new List<Slot>();
    List<Slot> othersItems = new List<Slot>();
    List<Slot> allItems = new List<Slot>();

    public GameManager gameManager;

    [Header("Zapis szybkiego dostêpu")]
    [SerializeField] SetWeaponFromInventory mainWeapon;
    [SerializeField] SetWeaponFromInventory secondWeapon;
    [SerializeField] SetMedicineFromInventory medicine;
    [SerializeField] SetArmorFromInventory armor;
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerHealth playerHealth;
    // inicjalizacja zmiennych

    private void Awake()
    {
        Instance = this;
        
        for (int i = 0; i < slots.Length; i++)
        {
            itemsDescription.Add(slots[i].inventorySlot.transform.GetChild(1).GetComponent<ShowItemDescription>());
            itemsDescription[i].slotNumber = i;
            slots[i].DisplaySlotAmount();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            foreach (Item item in items)
                CreateItem(item.ID);

        if (!transform.GetChild(0).gameObject.activeInHierarchy) return;

        goldAmount.text = "" + playerGold;

        if (Input.GetKeyDown(KeyCode.S))
            SortSlots();

    }
    /// <summary>
    /// sprawdza czy przedmiot istnieje, je¿eli tak dodaje do slotu
    /// </summary>
    /// <param name="id">identyfikator przedmiotu</param>
    void CreateShortcut(int id)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].slotID == id && slots[i].item.maxItemsInSlot > slots[i].slotAmount)
            {
                slots[i].slotAmount++;
                itemAddToSlot = true;
                UpdateInventoryData();
                return;
            }
        }

        itemAddToSlot = false;
    }
    /// <summary>
    /// Próbuje stworzyæ nowy przedmiot w pustym slocie
    /// </summary>
    /// <param name="item">przedmiot do stworzenia</param>
    /// <param name="amount">iloœæ przedmiotów</param>
    /// <param name="durability">wytrzyma³oœæ, opcjonalna tylko do pancerza</param>
    /// <returns></returns>
    bool TryCreateNewSlot(Item item, int amount, float durability = 0)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].slotID == 0)
            {
                slots[i].item = item;
                slots[i].slotID = item.ID;

                if(durability > 0)
                    slots[i].durability = durability;
                else if( item.durability > 0 )
                    slots[i].durability = item.durability;

                slots[i].slotAmount = amount;
                slots[i].inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = item.image;

                UpdateInventoryData();
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// tworzy jeden przedmiot na podstawie identyfikatora
    /// </summary>
    /// <param name="id">identyfikator</param>
    /// <param name="durability">wytrzyma³oœæ, opcjonalna tylko do pancerza</param>
    public void CreateItem(int id, float durability = 0)
    {
        Item item = Array.Find(items, i => i.ID == id);
        if (item == null) return;

        CreateItem(item,durability);
    }
    // tworzy przedmiot w slocie za pomoc¹ danych przekazanego item'u
    public void CreateItem(Item item, float durability = 0)
    {
        CreateShortcut(item.ID);
        if (itemAddToSlot) return;

        TryCreateNewSlot(item, 1,durability);
    }
    public void UpdateInventoryData()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            itemsDescription[i].itemIdInSlot = slots[i].slotID;
            itemsDescription[i].itemInSlot = slots[i].item;
            slots[i].DisplaySlotAmount();
        }
    }

    // tworzy wiele przedmiotów za pomoc¹ parametru 'id'
    public void CreateManyItems(Item item, int howManyItems, float durability = 0)
    {
        if (howManyItems <= 0) return;

        if (item.tag == Item.ItemTag.Ammunition)
        {
            switch (item.ID)
            {
                case 150: AmmunitionStorage.ChangeHandgunAmmo(howManyItems); return; // handgun ammo
                case 151: AmmunitionStorage.ChangeRifleAmmo(howManyItems); return; // rifle ammo
            }
        }

        int remaining = howManyItems;

        // Dodawanie do istniej¹cych slotów
        while (remaining > 0)
        {
            CreateShortcut(item.ID);
            if (itemAddToSlot)
            {
                remaining--;
                continue;
            }

            int amountToAdd = Mathf.Min(remaining, item.maxItemsInSlot);
            if (!TryCreateNewSlot(item, amountToAdd, durability)) break;

            remaining -= amountToAdd;
        }
    }
    public void ChangeItemSlot(int nativSlot, int slotToChangeNumber, int ID, bool emptySlot = true)
    {
        SetSlotSprite(slotToChangeNumber, ID);

        var native = slots[nativSlot];
        var target = slots[slotToChangeNumber];

        int previousID = target.slotID;
        int previousAmount = target.slotAmount;

        // przypisujemy dane do slotu docelowego
        target.slotID = ID;
        target.slotAmount = native.slotAmount;
        target.item = native.item;

        if (emptySlot)
        {
            // czyœcimy slot Ÿród³owy
            native.slotID = 0;
            native.slotAmount = 0;
            native.item = null;
            SetSlotSprite(nativSlot, 0);
        }
        else
        {
            // zamiana danych (swap)
            native.slotID = previousID;
            native.slotAmount = previousAmount;
            native.item = items.FirstOrDefault(i => i.ID == previousID);
            SetSlotSprite(nativSlot, previousID);
        }

        UpdateInventoryData();
    }
    // wybiera sprite z listy przedmiotów
    private void SetSlotSprite(int slotIndex, int itemId)
    {
        var img = slots[slotIndex].inventorySlot.transform.GetChild(1).GetComponent<Image>();
        var item = items.FirstOrDefault(i => i.ID == itemId);
        img.sprite = item != null ? item.image : nullSprite;
    }

    // sprawdza ile jest wolnych slotów
    public int FreeSlotsAmount()
    {
        int freeSlots = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].slotID == 0)
                freeSlots++;
        }
        return freeSlots;
    }
    // sprawdza ile jest wsystkich przedmiotów w pierwszym slocie który ma dane 'id'
    public int HowManyItemsInSlot(int id)
    {
        return slots.FirstOrDefault(s => s.slotID == id)?.slotAmount ?? 0;
    }
    // sprawdza ile jest wsystkich przedmiotów danego 'id' w konkretnym slocie
    public int HowManyItemsInSlot(int id, int slotNumber)
    {
        int howManyItems = 0;

        if (slots[slotNumber].slotID == id)
            howManyItems = slots[slotNumber].slotAmount;

        return howManyItems;


    }
    // sprawdza ile jest wsystkich przedmiotów danego 'id'
    public int HowManyItems(int id)
    {
        int howManyItems = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].slotID == id)
            {
                howManyItems += slots[i].slotAmount;
            }
        }
        return howManyItems;
    }
    // sprawdza czy przedmiot istnieje
    public bool CheckIfItemExist(int id)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].slotID == id)
                return true;
        }
        return false;
    }
    // zwraca ca³y przedmiot o ponadnym 'id'
    public Item GetItem(int id)
    {
        Item itemToReturn = null;
        for (int i = 0; i < items.Length; i++)
        {
            if (id == items[i].ID)
            {
                itemToReturn = items[i];
                break;
            }
        }
        return itemToReturn;
    }
    // usuwa okreœlon¹ iloœæ elementów w okreœlonym slocie
    public void ReduceItem(int id, int howManyItems, int slotNumber)
    {
        slots[slotNumber].slotAmount -= howManyItems;

        if (slots[slotNumber].slotAmount <= 0)
            DeleteItem(id, slotNumber);

        UpdateInventoryData();
        return;
    }
    // usuwa okreœlon¹ iloœæ elementów 
    public void ReduceItem(int id, int howManyItems)
    {
        int howManyItemsLeft = howManyItems;
        for (int i = 0; i < slots.Length; i++)
        {
            if (id == slots[i].slotID)
            {
                howManyItemsLeft -= slots[i].slotAmount;
                slots[i].slotAmount -= howManyItems;

                if (slots[i].slotAmount == 0)
                    DeleteItem(id);

                if (slots[i].slotAmount <= 0 && howManyItemsLeft > 0)
                    ReduceItem(id, howManyItemsLeft);

                UpdateInventoryData();
                return;
            }
        }
    }
    // usuwa przedmiot w okreœlonym slocie
    public void DeleteItem(int ID, int slotNumber)
    {

        slots[slotNumber].slotAmount = 0;
        slots[slotNumber].slotID = 0;
        slots[slotNumber].item = null;
        slots[slotNumber].inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = nullSprite;
        UpdateInventoryData();
        return;
    }
    // usuwa przedmiot o okreœlonym 'id'
    public void DeleteItem(int ID)
    {
        if (ID == 0)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].slotID == ID)
            {

                slots[i].slotAmount = 0;
                slots[i].slotID = 0;
                slots[i].item = null;
                slots[i].inventorySlot.transform.GetChild(1).GetComponent<Image>().sprite = nullSprite;
                UpdateInventoryData();

                return;
            }
        }
    }
    public void SaveInventory()
    {
        InventoryData data = new InventoryData();

        for (int i = 0; i < slots.Length; i++)
        {
            data.slots.Add(new InventorySlot
            {
                slotID = slots[i].slotID,
                slotAmount = slots[i].slotAmount,
                slotDurability = slots[i].durability

            });
        }
        data.playerGold = playerGold;
        data.mainWeaponFastAcessID = mainWeapon.actualUseID;
        data.secondeaponFastAcessID = secondWeapon.actualUseID;
        data.medicineFastAcessID = medicine.actualUseID;
        data.armorFastAccessID = armor.actualUseID;
        data.armoarDurability = armor.durability;

        gameManager.gameData.inventoryData = data;
    }

    // wczytanie ekwipunku
    public void LoadInventory()
    {
        InventoryData data = new InventoryData();
        data = gameManager.gameData.inventoryData;
        for (int i = 0; i < slots.Length; i++)
        {
            DeleteItem(slots[i].slotID, i);
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (data.slots[i].slotID != 0)
                CreateManyItems(GetItem(data.slots[i].slotID), data.slots[i].slotAmount, data.slots[i].slotDurability);
        }
        playerGold = data.playerGold;

        SetFastAccess(mainWeapon, data.mainWeaponFastAcessID);
        SetFastAccess(secondWeapon, data.secondeaponFastAcessID);
        SetFastAccess(medicine, data.medicineFastAcessID);
        SetFastAccess(armor, data.armorFastAccessID);
        armor.durability = data.armoarDurability;

        playerHealth.SetVestDurabilityTextInUI(armor.durability);

        playerController.ChangeWeapon(1, GetItem(data.mainWeaponFastAcessID));
        playerController.ChangeWeapon(0, GetItem(data.secondeaponFastAcessID));

        medicine.GetItemDescription();
        medicine.SetValues();
    }
    void SetFastAccess(DropItemToFastAccess slot, int itemId)
    {
        slot.actualUseID = itemId;

        var description = slot.GetComponent<ShowItemDescription>();
        var item = GetItem(itemId);

        description.itemIdInSlot = itemId;
        description.itemInSlot = item;

        if (item != null)
        slot.GetComponent<Image>().sprite = item.image;
    }
    public void SortSlots()
    {
        foreach (Slot slot in slots)
            if (slot.item != null)
                slotsToSort.Add(slot.Copy());

        if (slotsToSort.Count > 0)
        {
            QuickSort(0, slotsToSort.Count - 1);

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {

                    DeleteItem(slots[i].item.ID);
                }
            }

            for (int i = 0; i < slotsToSort.Count; i++)
            {
                CreateManyItems(slotsToSort[i].item, slotsToSort[i].slotAmount);
            }

        }
        slotsToSort.Clear();
    }
    public void SortSlotsByTag(Item.ItemTag itemTag)
    {
        RollbackInventory();

        allItems.Clear();
        slotsToSort.Clear();
        othersItems.Clear();

        foreach (Slot slot in slots)
        {
            if (slot.item == null) continue;

            var copy = slot.Copy();
            if (copy.item.tag == itemTag)
            {
                slotsToSort.Add(copy);
                allItems.Add(copy);
            }
            else
                allItems.Add(copy);
        }

        if (slotsToSort.Count > 0)
        {
            ClearInventory();

            foreach (var slot in slotsToSort)
                CreateManyItems(slot.item, slot.slotAmount, slot.durability);

        }

        slotsToSort.Clear();
        othersItems.Clear();
    }
    public void RollbackInventory()
    {
        if (allItems.Count == 0) return;

        ClearInventory();
        foreach (var slot in allItems)
        {
            if (slot.slotAmount == 0) continue;

            CreateManyItems(slot.item, slot.slotAmount, slot.durability);
        }

        allItems.Clear();
    }
    private void ClearInventory()
    {
        foreach (var slot in slots)
        {
            if (slot.item != null && slot.slotID != 0)
                DeleteItem(slot.item.ID);
        }
    }

    void QuickSort(int left, int right)
    {
        if (left < right)
        {
            Slot pivot = slotsToSort[left];
            int s = left;

            for (int i = left + 1; i <= right; i++)
            {
                if (slotsToSort[i].item.value > pivot.item.value)
                {
                    s++;
                    Swap(s, i);
                }
            }

            Swap(left, s);

            QuickSort(left, s - 1);
            QuickSort(s + 1, right);
        }
    }

    void Swap(int one, int two)
    {
        Slot temp = slotsToSort[one];
        slotsToSort[one] = slotsToSort[two];
        slotsToSort[two] = temp;
    }
}






