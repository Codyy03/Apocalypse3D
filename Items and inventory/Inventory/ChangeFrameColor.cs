using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ChangeFrameColor : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] Image frame;
    [SerializeField] bool changeToDiffrentColor;
    [SerializeField] Color nativColor, colorAfterPointerEnter;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!changeToDiffrentColor)
        {
          frame.color = Color.red;
          return;
        }
        frame.color = colorAfterPointerEnter;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!changeToDiffrentColor)
        {
            frame.color = Color.white;
            return;
        }
        frame.color = nativColor;
    }
}
