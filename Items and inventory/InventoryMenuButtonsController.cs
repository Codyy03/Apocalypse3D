using System.Collections.Generic;
using UnityEngine;

public class InventoryMenuButtonsController : MonoBehaviour
{
    List<InventoryMenuButton> inventoryMenuButtons = new List<InventoryMenuButton>();

    Inventory inventory;

    private void Awake()
    {
        inventory = GetComponentInParent<Inventory>();
        inventoryMenuButtons.AddRange(FindObjectsByType<InventoryMenuButton>(FindObjectsSortMode.None));
    }

    public void PrepereToClick(InventoryMenuButton inventoryMenuButton)
    {
        bool wasSorted = false;
        foreach (InventoryMenuButton button in inventoryMenuButtons)
        {
            if (!button.wasClick && button != inventoryMenuButton)
            {
                wasSorted = true;
                button.wasClick = true;
            }

            button.iconFocus.SetActive(false);
        }

        if(wasSorted)
        {
            inventory.RollbackInventory();
        }

    }
}
