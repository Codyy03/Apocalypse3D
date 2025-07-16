using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecificLoot : MonoBehaviour
{
    [SerializeField] List<Item> items;
    [SerializeField] GameObject lootPrefab;
    [SerializeField] int howManyItems;

    public Item RandomLoot()
    {
        Item currentItem;

        currentItem= items[Random.Range(0, items.Count)];

        items.Remove(currentItem);
        return currentItem;
    }

    public void CreateLootPrefab(Vector2 position)
    {
        GameObject loot = null;
        List<Item> itemsToTake = new List<Item>();

        for(int i = 0; i < howManyItems; i++)
        {
            itemsToTake.Add(RandomLoot());
        }
        loot = Instantiate(lootPrefab, position,transform.rotation);

        loot.GetComponent<EnemiesLootController>().itemsToTake = itemsToTake;
    }
}
