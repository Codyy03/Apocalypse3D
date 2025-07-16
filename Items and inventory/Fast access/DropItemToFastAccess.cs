using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DropItemToFastAccess : MonoBehaviour, IDropHandler
{
    [SerializeField] Sprite basicSprite;
    [SerializeField] AudioClip dropSound;

    public int actualUseID;

    [Tooltip("Czy tworzyæ przedmiot w ekwpipunktu po tym jak zamieniasz przedmioty (znacz¹ce gdy jest ju¿ przedmiot w slocie)")]
    public bool dontCreateItemAfterChange;

    public List<Item> itemsThatFitInSlot = new List<Item>();

    protected Inventory inventory;

    AudioManager audioManager;

    protected ShowItemDescription showItemDescription;
    protected ShowItemDescription currentDragItem;
    public virtual void OnDrop(PointerEventData eventData)
    {

        currentDragItem = eventData.pointerDrag?.GetComponent<ShowItemDescription>();
        for (int i = 0; i < itemsThatFitInSlot.Count; i++)
        {
            if (itemsThatFitInSlot[i].ID == currentDragItem.itemIdInSlot)
            {
                if (itemsThatFitInSlot[i].ID == actualUseID) return;

                if (actualUseID != itemsThatFitInSlot[i].ID && actualUseID != 0 && !dontCreateItemAfterChange)
                {
                    inventory.CreateItem(actualUseID);
                }

                gameObject.GetComponent<Image>().sprite = itemsThatFitInSlot[i].image;

                showItemDescription.itemIdInSlot = itemsThatFitInSlot[i].ID;
                showItemDescription.slotNumber = currentDragItem.slotNumber;
                showItemDescription.itemInSlot = currentDragItem.itemInSlot;

                actualUseID = itemsThatFitInSlot[i].ID;

                audioManager.PlayClip(dropSound);

                return;

            }
        }

    }

    protected virtual void Awake()
    {
        GetItemDescription();
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    public void UpdateFastAccess()
    {
        gameObject.GetComponent<Image>().sprite = basicSprite;
        actualUseID = 0;
        showItemDescription.itemInSlot = null;
        showItemDescription.itemIdInSlot = 0;
        showItemDescription.slotNumber = 0;
    }
    public void GetItemDescription()
    {
        showItemDescription = GetComponent<ShowItemDescription>();
        inventory = FindFirstObjectByType<Inventory>();
    }
}
