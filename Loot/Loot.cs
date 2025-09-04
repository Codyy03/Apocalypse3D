using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static LootController;

[RequireComponent(typeof(LootIDAssignerZ))]
public class Loot : MonoBehaviour
{
    public string lootID;
    public List<ItemInLoot> items;

    [Tooltip("Je¿eli zaznaczone to zniszcz obiekt po zebraniu z niego przedmiotów")]
    public bool destroy;

    private void OnValidate()
    {
        if (items == null) return;

        foreach (ItemInLoot item in items)
        {
            if (item != null && item.quantity <= 0) 
                Debug.LogError(
                    $"B³¹d w {gameObject.name}" +
                    $"Przemiot '{item.item.objectName}'"+
                    $"ma quantity = {item.quantity} (musi byæ > 0)",
                    this
                    );
        }
    }
}
