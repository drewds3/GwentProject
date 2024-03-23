using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerClickHandler
{
    public static GameObject card;
   public void OnPointerClick(PointerEventData eventData)
   {    
        
        card = gameObject;
        GameObject.Find("CardViewPanel").GetComponent<Image>().sprite = card.GetComponent<Image>().sprite;
   }
}