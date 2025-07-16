using UnityEngine;
using UnityEngine.EventSystems;

public class SetArmorFromInventory : DropItemToFastAccess
{
    public float durability;
    PlayerHealth playerHealth;
    protected override void Awake()
    {
        base.Awake();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            ChangeDurablility(-10);
    }
    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);

        for (int i = 0; i < itemsThatFitInSlot.Count; i++)
        {
            if (currentDragItem.itemIdInSlot == itemsThatFitInSlot[i].ID)
            {
                durability = inventory.slots[currentDragItem.slotNumber].durability;
                inventory.slots[currentDragItem.slotNumber].durability = 0;
                inventory.ReduceItem(currentDragItem.itemIdInSlot, 1, currentDragItem.slotNumber);
                break;
            }
        }
        playerHealth.SetVestDurabilityTextInUI(durability);
    }

    public void ChangeDurablility(float value)
    {
        durability = Mathf.Clamp(durability + value, 1, showItemDescription.itemInSlot.durability);
    }
}

