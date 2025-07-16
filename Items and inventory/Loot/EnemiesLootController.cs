using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesLootController : MonoBehaviour
{
    public static bool objectWasOpen, playItneractAnimation;
    [SerializeField] GameObject notificationCanvas, itemSpawn, background;
    [SerializeField] AudioClip open, close;
    [SerializeField] TakeEverythingButton takeEveryThing;


    public List<Item> itemsToTake = new List<Item>();
    bool playerCanOpen;
    bool dontAddAnotherItem;
    GameObject lootSpawnPoint;
    GameObject currnetLootManager;

    AudioManager audioManager;
    Notification notification;
    Rigidbody2D rb2d;
    BoxCollider2D[] boxColliders = new BoxCollider2D[2];
    //AnimatorController playerAnimations;
    private void Awake()
    {
       // playerAnimations = FindFirstObjectByType<AnimatorController>();
        boxColliders = GetComponents<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        notification = GetComponent<Notification>();
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    // Update is called once per 
    void Update()
    {
        if (!playerCanOpen)
        {
            notification.SwitchNotification(notificationCanvas, false);
            return;
        }

        notification.SwitchNotification(notificationCanvas, true);

        if (Input.GetKeyDown(KeyCode.E) && !objectWasOpen)
        {
           // playerAnimations.ChangeAnimationState(playerAnimations.interactGrabAnimation);
            playItneractAnimation = true;
            Invoke("DisableInteractAnimation", 0.5f);
            OpenObject();
        }


        if (Input.GetKeyDown(KeyCode.E) && objectWasOpen)
        {
            background.SetActive(true);
        }
    }

    void OpenObject()
    {
        background.SetActive(true);
        lootSpawnPoint = GameObject.FindGameObjectWithTag("SpawnLoot").gameObject;
        for (int i = 0; i < itemsToTake.Count; i++)
        {
            if (dontAddAnotherItem)
                break;
            //loot manager pobiera id przedmiotu i 
            itemSpawn.GetComponent<LootManager>().itemId = itemsToTake[i].ID;
            itemSpawn.GetComponent<Image>().sprite = itemsToTake[i].image;

            // spawnuje przedmiot
            currnetLootManager = Instantiate(itemSpawn, lootSpawnPoint.transform.position, Quaternion.identity, lootSpawnPoint.transform);
            // dodaje przedmiot do listy wez wszystko
            takeEveryThing.lootManager.Add(currnetLootManager.GetComponent<LootManager>());

        }
        objectWasOpen = true;
        dontAddAnotherItem = true;
        audioManager.PlayClip(open);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerCanOpen = true;

        if (collision.gameObject.CompareTag("Ground"))
        {
            rb2d.gravityScale = 0;
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionY;
            BoxCollider2D nonTriggerCollider = null;
            foreach (BoxCollider2D collider in boxColliders)
            {
                if (!collider.isTrigger)
                {
                    nonTriggerCollider = collider;
                    break;
                }
            }
            nonTriggerCollider.enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        background.SetActive(false);
        playerCanOpen = false;
        objectWasOpen = false;
    }
    public void DisableLoot()
    {
        background.SetActive(false);
        notificationCanvas.SetActive(false);
        audioManager.PlayClip(close);
        objectWasOpen = false;
    }

    void DisableInteractAnimation()
    {
        playItneractAnimation = false;

    }
}

