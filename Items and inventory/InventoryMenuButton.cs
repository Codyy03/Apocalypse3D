using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Item.ItemTag itemTag;
    private GameObject focusTab;
    private GameObject IconFocus { get; set; }
    public bool wasClick;

    private Inventory inventory;
    private InventoryMenuButtonsController controller;

    private void Awake()
    {
        controller = GetComponentInParent<InventoryMenuButtonsController>();
        inventory = FindFirstObjectByType<Inventory>();
        IconFocus = transform.GetChild(0).gameObject;
        focusTab = transform.GetChild(1).gameObject;

        wasClick = true;

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        controller.PrepereToClick(this);

        if (wasClick)
            inventory.SortSlotsByTag(itemTag);
        else
            inventory.RollbackInventory();


        IconFocus.SetActive(wasClick);

        wasClick = !wasClick;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        focusTab.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        focusTab.SetActive(false);
    }
    public GameObject iconFocus
    {
        get { return IconFocus; }
    }

}
