using Knife.RealBlood.SimpleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] AudioClip equipSound;
    int nativSlot, slotToChange, id;

    Inventory inventory;

    AudioManager audioManager;
    //  PlayerHealth playerHealth;
    bool pointerIsOnItem;
    PlayerController playerController;
    ShowItemDescription showItemDescription;
    private void Awake()
    {
        showItemDescription = GetComponent<ShowItemDescription>();
        inventory = FindFirstObjectByType<Inventory>();
        audioManager = FindFirstObjectByType<AudioManager>();
        playerController = FindFirstObjectByType<PlayerController>();
        //  playerHealth = FindFirstObjectByType<PlayerHealth>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag?.GetComponent<ShowItemDescription>();
        var fastAccessWeapon = eventData.pointerDrag?.GetComponent<SetWeaponFromInventory>();
        var fastAccessMedicine = eventData.pointerDrag?.GetComponent<SetMedicineFromInventory>();
        var fastAccessArmor = eventData.pointerDrag?.GetComponent<SetArmorFromInventory>();

        if (draggedItem == null || draggedItem.itemIdInSlot == 0) return;

        nativSlot = draggedItem.slotNumber;
        id = draggedItem.itemIdInSlot;
        slotToChange = showItemDescription.slotNumber;

        if (fastAccessWeapon == null && fastAccessMedicine == null && fastAccessArmor == null)
        {
            if (showItemDescription.itemIdInSlot == 0)
            {
                inventory.ChangeItemSlot(nativSlot, slotToChange, id);
                audioManager.PlayClip(equipSound);
                return;
            }
            if (showItemDescription.itemIdInSlot != draggedItem.itemIdInSlot)
            {
                inventory.ChangeItemSlot(nativSlot, slotToChange, id, false);
                audioManager.PlayClip(equipSound);
                return;
            }

            if (showItemDescription.itemIdInSlot == id)
            {
                MergeItemStacks(nativSlot, slotToChange, id);
            }
        }
        else if (fastAccessWeapon != null)
        {
            inventory.CreateItem(fastAccessWeapon.actualUseID);
            playerController.RemoveWeapon(fastAccessWeapon.weaponType);
            playerController.UpdateWeaponAfterInventoryChange(fastAccessWeapon.weaponType == 0 ? 1 : 0);
            fastAccessWeapon.UpdateFastAccess();

        }
        else if (fastAccessMedicine != null)
        {
            fastAccessMedicine.UpdateFastAccess();
        }
        else if (fastAccessArmor != null)
        {
            inventory.CreateItem(fastAccessArmor.actualUseID, fastAccessArmor.durability);
            fastAccessArmor.UpdateFastAccess();
        }
    }
    private void MergeItemStacks(int from, int to, int itemId)
    {
        int currentAmount = inventory.HowManyItemsInSlot(itemId, to);
        int maxAmount = inventory.ReturnItem(itemId).maxItemsInSlot;
        int canAdd = maxAmount - currentAmount;

        if (canAdd <= 0) return;

        if (canAdd >= inventory.slots[from].slotAmount)
        {
            inventory.slots[to].slotAmount += inventory.slots[from].slotAmount;
            inventory.DeleteItem(itemId, from);
        }
        else
        {
            inventory.slots[to].slotAmount += canAdd;
            inventory.slots[from].slotAmount -= canAdd;
            inventory.UpdateInventoryData();
        }

        audioManager.PlayClip(equipSound);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerIsOnItem = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerIsOnItem = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pointerIsOnItem)
            return;

        if (Input.GetKeyDown(KeyCode.D))
            inventory.ReduceItem(showItemDescription.itemIdInSlot, 1, showItemDescription.slotNumber);
    }

}
