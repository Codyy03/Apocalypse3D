using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class SetMedicineFromInventory : DropItemToFastAccess
{
    [SerializeField] FastAccessUi accessUI;
    [SerializeField] UI ui;
 
    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        SetValues();

        ui.itemDescription = currentDragItem;
    }
    public void SetValues()
    {
        if(showItemDescription.itemInSlot != null)
        {
            accessUI.medicineName.text = showItemDescription.itemInSlot.objectName;
            accessUI.medicineQuantity.text = $"x{inventory.HowManyItems(actualUseID)}";
            accessUI.medicineHp.text = $"{showItemDescription.itemInSlot.health} PZ";
            accessUI.icon.sprite = showItemDescription.itemInSlot.image;
        }
    }
}
