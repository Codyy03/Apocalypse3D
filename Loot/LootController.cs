using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
public class LootController : MonoBehaviour
{
    public GameObject lootPanel;
    public Transform contentParent;
    public GameObject lootItemPrefab;

    List<GameObject> lootItems;
    [Serializable]
    public class ItemInLoot
    {
        public Item item;
        public int quantity;
        public UnityEvent onLootTaken = new();
    }

    private void Update()
    {
        if (contentParent.childCount <= 0) return;

        if (CheckHighlightStatus())
        { 
            contentParent.GetChild(0).transform.GetChild(4).gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Wyœwietl liste przedmiotów do zebrania
    /// </summary>
    /// <param name="items">lista przedmiotów do zebrania</param>
    public void ShowLoot(List<ItemInLoot> items)
    {
        lootPanel.SetActive(true);

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        lootItems = new List<GameObject>();

        foreach (ItemInLoot itemToTake in items)
        {
            GameObject slot = Instantiate(lootItemPrefab, contentParent);
            slot.GetComponent<ItemToTake>().Setup(itemToTake.item, itemToTake.quantity);
            lootItems.Add(slot);
        }
    }
    /// <summary>
    /// sprawdz czy któryœ z przedmiotów jest zaznaczony
    /// </summary>
    /// <returns>zaznaczony/niezaznaczony</returns>
    public bool CheckHighlightStatus()
    {
        for(int i = 0; i< contentParent.childCount; i++)
        {
            if (contentParent.GetChild(i).transform.GetChild(4).gameObject.activeInHierarchy)
                return false;

        }
        InteractionController.currentChild = 0;
        return true;
    }
}
