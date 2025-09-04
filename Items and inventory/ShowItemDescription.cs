using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Linq;


public class ShowItemDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum WhereIsSlot
    {
        inventory,
        fastAccess,
        shop
    }

    readonly float ScreenX = 0.2f;

    Image slotImage;
    [Tooltip("Domy�lny sprite")]
    [SerializeField] Sprite nativeSprite;

    [Tooltip("Sprite po najechaniu kursorem na slot")]
    [SerializeField] Sprite pointerOnSlotSprite;

    [Tooltip("Obiekt, kt�ry wy�wietla informacje o itemie po najechaniu kursorem na item")]
    public GameObject descriptionImage;

    public WhereIsSlot whereIsSlot;

    public int itemIdInSlot, slotNumber;
    public Item itemInSlot;
    public float distanceToSlot = 0;

    GameObject parentGameObject;

    TextMeshProUGUI itemName, description, value, other, priceForFull, quality;

    Inventory inventorySystem;

    Image qualityTextBackground;
    
    ItemsQualitySpritesList qualitySpritesList;

    List<string> details = new List<string>();
    
    SetArmorFromInventory setArmorFromInventory;

    Canvas canvas;

    private void Awake()
    {
        slotImage = transform.parent.GetComponent<Image>();
        inventorySystem = FindFirstObjectByType<Inventory>();
        parentGameObject = transform.parent.gameObject;
        qualitySpritesList = descriptionImage.GetComponent<ItemsQualitySpritesList>();
        setArmorFromInventory = GetComponent<SetArmorFromInventory>();

        var root = descriptionImage.transform;
        itemName = root.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        description = root.GetChild(1).GetComponent<TextMeshProUGUI>();
        value = root.GetChild(2).GetComponent<TextMeshProUGUI>();
        other = root.GetChild(3).GetComponent<TextMeshProUGUI>();
        quality = root.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
        qualityTextBackground = root.GetChild(4).GetComponent<Image>();

        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        slotImage.sprite = pointerOnSlotSprite;
        ShowDescription();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slotImage.sprite = nativeSprite;
        parentGameObject.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        descriptionImage.SetActive(false);
        details.Clear();
    }

    public void ShowDescription()
    {
        if (itemInSlot == null) return;

        descriptionImage.SetActive(true);
        itemName.text = itemInSlot.objectName;
        description.text = itemInSlot.descripotion;
        value.text = itemInSlot.value > 1 ? $"Warto�� przedmiotu: {itemInSlot.value}" : "";

        BuildItemDetails();
        DisplayDetails();
        SetPosition();
        DisplayQuality();
    }

    void BuildItemDetails()
    {
        details.Clear();

        if (itemInSlot.damage > 0)
            details.Add($"Obra�enia broni: {itemInSlot.damage}");

        if (itemInSlot.armor > 0)
            details.Add($"Reukcja obra�e� przy pe�nej wytrzyma�o�ci: {itemInSlot.armor * 100} %");

        if (itemInSlot.health > 0)
            details.Add($"Przedmiot odnawia: {itemInSlot.health} �ycia");

        float durability = setArmorFromInventory?.durability ?? inventorySystem.slots[slotNumber].durability;

        if (durability > 0)
            details.Add($"Wytrzyma�o�� pancerza: {durability}");
    }

    void DisplayDetails()
    {
        other.gameObject.SetActive(details.Count > 0);
        other.text = string.Join("\n", details);
    }

    void SetPosition()
    {
        RectTransform descRect = descriptionImage.GetComponent<RectTransform>();
        RectTransform slotRect = GetComponent<RectTransform>();

        // �wiatowa pozycja slotu -> lokalna wzgl�dem Canvas
        Vector2 slotPosInCanvas;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            descRect.parent as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, slotRect.position),
            null,
            out slotPosInCanvas
        );

        // ustaw bazow� pozycj�
        Vector2 anchoredPos = slotPosInCanvas;

        // oblicz przesuni�cie w jednostkach Canvas
        float offsetX = (descRect.parent as RectTransform).rect.width * ScreenX;

        // automatyczne wykrycie strony � je�li slot po prawej, przesu� tooltip w lewo
        bool isRightSide = slotPosInCanvas.x > 0;
        if (isRightSide)
            offsetX = -offsetX;

        anchoredPos.x += offsetX;

        descRect.anchoredPosition = anchoredPos;
    }

    void DisplayQuality()
    {
        Dictionary<Item.Quality, (string label, Sprite sprite)> qualityMap = new()
        {
            { Item.Quality.Broken, ("Jako��: �mie�", qualitySpritesList.broken) },
            { Item.Quality.Common, ("Jako��: zwyk�a", qualitySpritesList.common) },
            { Item.Quality.Epic, ("Jako��: epicka", qualitySpritesList.epic) },
            { Item.Quality.Legendary, ("Jako��: legendarna", qualitySpritesList.legendary) }
        };

        var q = qualityMap[itemInSlot.quality];
        quality.text = q.label;
        qualityTextBackground.sprite = q.sprite;
    }
}


