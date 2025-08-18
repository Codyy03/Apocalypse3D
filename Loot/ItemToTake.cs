using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Item;

public class ItemToTake : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Item item;
    public int quantity;
    Image icon;

    TextMeshProUGUI quantityText;
    TextMeshProUGUI itemNameText;
    TextMeshProUGUI itemDescriptionText;

    GameObject higtlight;

    InteractionController interactionController;

    private void Awake()
    {
        icon = transform.GetChild(0).GetComponent<Image>();
        quantityText = transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        itemNameText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        itemDescriptionText = transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        interactionController = FindFirstObjectByType<InteractionController>();

        higtlight = transform.GetChild(4).gameObject;
    }


    public void Setup(Item item, int quantity)
    {
        this.item = item;
        icon.sprite = item.image;
        this.quantity = quantity;

        quantityText.text = quantity.ToString();

        itemNameText.text = item.objectName;

        string tag = " ";

        switch (item.tag)
        {
            case Item.ItemTag.Trash: tag = "Inny"; break;
            case Item.ItemTag.Medicine: tag = "Lek"; break;
            case Item.ItemTag.Ammunition: tag = "Amunicja"; break;
            case Item.ItemTag.Weapon: tag = "Broñ"; break;
        }
        string quality = " ";

        switch (item.quality)
        {
            case Item.Quality.Broken: quality = "Œmieæ"; break;
            case Item.Quality.Common: quality = "Zwyk³y"; break;
            case Item.Quality.Epic: quality = "Epicki"; break;
            case Item.Quality.Legendary: quality = "Legendarny"; break;
        }

        itemDescriptionText.text = tag + " / " + quality;
    }

    public void OnPointerExit(PointerEventData eventData) => higtlight.SetActive(false);
    public void OnPointerEnter(PointerEventData eventData)
    {
        higtlight.SetActive(true);

        if (transform.GetSiblingIndex() != 0)
            transform.parent.GetChild(0).GetChild(4).gameObject.SetActive(false);

        InteractionController.currentChild = transform.GetSiblingIndex();
    }

    public void OnPointerClick(PointerEventData eventData) => interactionController.TakeOneItem();
}
