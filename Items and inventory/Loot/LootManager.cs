using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class LootManager : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] AudioClip take;
    public int itemId;

    Inventory inventory;
   public OpenMapObject openMapObject;
  // Animator animator;
    AudioManager audioManager;
    EnemiesLootController enemiesLootController;
    TextMeshProUGUI itemName;
    Image itemImage;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerClick != null)
        {
            TakeItem();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null)
        {
            itemImage.color = Color.yellow;
            itemName.text = inventory.ReturnItem(itemId).objectName;          
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemImage.color = Color.white;
        itemName.text = null; 
    }
    private void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
        openMapObject = GetComponentInParent<OpenMapObject>();
        enemiesLootController = GetComponentInParent<EnemiesLootController>();
      //  animator = GetComponentInParent<Animator>();
        audioManager = FindFirstObjectByType<AudioManager>();
        itemName = GetComponentInChildren<TextMeshProUGUI>();
        itemImage = GetComponent<Image>();
    }
  
    public void TakeItem()
    {
        if (inventory.HowManyItemsInSlot(itemId) < inventory.ReturnItem(itemId).maxItemsInSlot || inventory.FreeSlotsAmount() > 1)
        {
            inventory.CreateItem(itemId);

            if(openMapObject!= null)
            {
                openMapObject.howManyItemsToDraw--;

                if (openMapObject.howManyItemsToDraw == 0)
                {
                  //  if(animator!= null)
                  //  animator.SetTrigger("Open");

                    openMapObject.DisableLoot();
                }
            }

            audioManager.PlayClip(take);

            if (transform.parent.childCount == 1)
            {
                EnemiesLootController.objectWasOpen = false;
                Destroy(enemiesLootController.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
