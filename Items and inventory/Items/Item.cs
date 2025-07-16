using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/item",fileName = "Object")]
public class Item : ScriptableObject
{
    public int ID;

    public enum Quality
    {
        Broken,     // bardzo s³aby stan, mo¿e mieæ negatywne efekty
        Common,     // podstawowa jakoœæ 
        Epic,       // bardzo silne przedmioty
        Legendary,  // wyj¹tkowe, czêsto z unikalnym efektem
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

    [Header("Wartoœci dodatkowe")]
    public float health;
    public int damage;

    [Header("Wartoœci pancerza")]
    public float armor;
    public float durability;

}
