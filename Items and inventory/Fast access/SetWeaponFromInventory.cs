using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetWeaponFromInventory : DropItemToFastAccess
{
    [Tooltip("1 = Main weapon, 0 = Second Weapon")]
    public int weaponType = 0;

    PlayerController controller;

    protected override void Awake()
    {
        base.Awake();
        controller = FindFirstObjectByType<PlayerController>();
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        for (int i = 0; i < itemsThatFitInSlot.Count; i++)
        {
            if(currentDragItem.itemIdInSlot == itemsThatFitInSlot[i].ID)
            {
                inventory.ReduceItem(currentDragItem.itemIdInSlot, 1, currentDragItem.slotNumber);
                break;
            }
        }

        controller.ChangeWeapon(weaponType, showItemDescription.itemInSlot);
    }


}
