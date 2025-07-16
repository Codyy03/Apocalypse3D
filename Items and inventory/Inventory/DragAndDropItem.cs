using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DragAndDropItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    Transform  frame;
    RectTransform rect;
    CanvasGroup canvasGroup;
   // Canvas slot;
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;

    }
    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;

        if (eventData.pointerDrag != null)
            eventData.pointerDrag.GetComponent<RectTransform>().localPosition = frame.localPosition;
        
        canvasGroup.blocksRaycasts = true;
      
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
       // slot = GetComponentInParent<Canvas>();
        if (frame == null)
            frame = transform.parent.GetChild(2).transform;
    }
   
}
