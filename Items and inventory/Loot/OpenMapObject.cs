using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OpenMapObject : MonoBehaviour
{
    [SerializeField] GameObject notificationCanvas,itemSpawn, background;
    [SerializeField] AudioClip open, close;
    [SerializeField] TakeEverythingButton takeEveryThing;
    [SerializeField] string objectQuality;
    public int howManyItemsToDraw;

    bool playerCanOpen, objectWasOpen;

    GameObject currnetLootManager;
    Item currentItem;

    GameObject lootSpawnPoint;
    Notification notification;
    RandomLoot randomLoot;
    AudioManager audioManager;
    SpecificLoot specificLoot;
    public enum LootType
    {
        Random,
        Specific
    }
    public LootType type;
    void Start()
    {
        notification = GetComponent<Notification>();
        if(type==LootType.Random)
        randomLoot = GameObject.Find("GameManager").GetComponent<RandomLoot>();

        if (type == LootType.Specific)
            specificLoot = GetComponent<SpecificLoot>();


        audioManager = GameObject.Find("GameManager").GetComponent<AudioManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!playerCanOpen || howManyItemsToDraw<=0)
        {
            notification.SwitchNotification(notificationCanvas, false);
            return;
        }


        notification.SwitchNotification(notificationCanvas, true);
        

        if(Input.GetKeyDown(KeyCode.E) && !objectWasOpen)
          OpenObject();


        if (Input.GetKeyDown(KeyCode.E) && objectWasOpen && howManyItemsToDraw>0)
        {
            background.SetActive(true);
        }

     }
    public void DisableLoot()
    {
        background.SetActive(false);
        notificationCanvas.SetActive(false);
        audioManager.PlayClip(close);
    }
    void OpenObject()
    {
        background.SetActive(true);
        lootSpawnPoint = GameObject.FindGameObjectWithTag("SpawnLoot").gameObject;
        for (int i = 0; i < howManyItemsToDraw; i++)
        {

            if (type == LootType.Random)
                // losuje przedmiot
                currentItem = randomLoot.DrawnItem(objectQuality);
            if (type == LootType.Specific)
                currentItem = specificLoot.RandomLoot();

            //loot manager pobiera id przedmiotu i 
            itemSpawn.GetComponent<LootManager>().itemId = currentItem.ID;
            itemSpawn.GetComponent<Image>().sprite = currentItem.image;
            // spawnuje przedmiot
            currnetLootManager = Instantiate(itemSpawn, lootSpawnPoint.transform.position, Quaternion.identity, lootSpawnPoint.transform);
            // dodaje przedmiot do listy wez wszystko
            takeEveryThing.lootManager.Add(currnetLootManager.GetComponent<LootManager>());

        }
        objectWasOpen = true;
        audioManager.PlayClip(open);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerCanOpen = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        background.SetActive(false);
        playerCanOpen = false;
        
    }

   
}
