using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static LootController;

public class Loot : MonoBehaviour
{
    public string lootID;
    public List<ItemInLoot> items;

    [Tooltip("Je¿eli zaznaczone to zniszcz obiekt po zebraniu z niego przedmiotów")]
    public bool destroy;

}
