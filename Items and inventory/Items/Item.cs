using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/item",fileName = "Object")]
public class Item : ScriptableObject
{
    public int ID;

    public enum Quality
    {
        Broken,     // bardzo s�aby stan, mo�e mie� negatywne efekty
        Common,     // podstawowa jako�� 
        Epic,       // bardzo silne przedmioty
        Legendary,  // wyj�tkowe, cz�sto z unikalnym efektem
    }
    public enum ItemTag
    {
        Trash,
        Weapon,
        Ammunition,
        Medicine,
        BulletproofVests
    }
    public Quality quality;
    public ItemTag tag;
    public Sprite image;

    public string objectName;
    public int maxItemsInSlot;

    [TextArea]
    public string descripotion;

    public int value;

    [Header("Warto�ci dodatkowe")]
    public float health;
    public int damage;

    [Header("Warto�ci pancerza")]
    public float armor;
    public float durability;

}
