using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TakeEverythingButton : MonoBehaviour,IPointerClickHandler, IPointerEnterHandler,IPointerExitHandler
{
   
    public List<LootManager> lootManager;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerClick != null)
        {
            for (int i = 0; i < lootManager.Count; i++)
            {
                if(lootManager[i])
                lootManager[i].TakeItem();
                EnemiesLootController.objectWasOpen = false;
            }
            Destroy(GetComponentInParent<EnemiesLootController>().gameObject);
           
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       if(eventData!=null)
        {
            GetComponent<Image>().color = Color.yellow;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.gray;
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
