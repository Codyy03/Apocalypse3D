using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InventoryArrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum WhereToGo 
    { 
        DontGo,
        FirstRight,
        ReturnToFirst,

    }
    [SerializeField] WhereToGo whereToGo;
    [SerializeField] List<GameObject> panels;
    Image arrowImage;

    private void Awake()
    {
        arrowImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData != null)
            arrowImage.color = new Color32(0x4B, 0x4B, 0x4B, 255);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        arrowImage.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData == null) return;

        switch (whereToGo)
        {
            case WhereToGo.DontGo: return;
            case WhereToGo.FirstRight: panels[0].SetActive(false); panels[1].SetActive(true); break;
            case WhereToGo.ReturnToFirst: panels[0].SetActive(true); panels[1].SetActive(false); break;

        }
        arrowImage.color = Color.white;
    }
}
