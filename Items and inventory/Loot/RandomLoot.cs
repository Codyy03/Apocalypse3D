using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLoot : MonoBehaviour
{
    [SerializeField] Item[] itemsToBeDrawn;
    List<Item> lowQualityItems = new List<Item>();
    List<Item> normalQualityItems = new List<Item>();
    List<Item> highQualityItems = new List<Item>();
    // Start is called before the first frame update
    void Start()
    {
        ReturnListOfQualityItems(0, lowQualityItems);
        ReturnListOfQualityItems(1, normalQualityItems);
        ReturnListOfQualityItems(2, highQualityItems);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Item DrawnItem(string objectQuality)
    {
        int randomItemQuality = Random.Range(0, 100);
        Item itemToReurn = null;
        int lowQuality = 0;
        int normalQuality = 0;
        int normalMaxQuality = 0;
        int higtQuality = 0;

        switch(objectQuality)
        {
            case "low": lowQuality = 80; higtQuality = 95; normalQuality = 80; normalMaxQuality = 95;  break;
            case "noraml": lowQuality = 60; higtQuality = 85; normalQuality = 60; normalMaxQuality = 85; break;
            case "high": lowQuality = 5; higtQuality = 30; normalQuality = 5; normalMaxQuality = 30; break;

        }
        if (randomItemQuality <= lowQuality)
            itemToReurn = lowQualityItems[Random.Range(0, lowQualityItems.Count)];


        if (randomItemQuality > normalQuality && randomItemQuality <= normalMaxQuality)
            itemToReurn = normalQualityItems[Random.Range(0, normalQualityItems.Count)];

        if (randomItemQuality > higtQuality)
            itemToReurn = highQualityItems[Random.Range(0, highQualityItems.Count)];

        return itemToReurn;

    }

    void ReturnListOfQualityItems(int quality, List<Item> items)
    {
        for (int i = 0; i < itemsToBeDrawn.Length; i++)
        {
          // if(itemsToBeDrawn[i].quality == quality)
                  items.Add(itemsToBeDrawn[i]);
        }
        
     }

   



    }


   
 
