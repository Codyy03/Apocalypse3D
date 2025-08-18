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

}
