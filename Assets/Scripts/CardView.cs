using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerClickHandler
{
     //Carta con el script
     public static GameObject card;

     //Al hacerle click se muestra en el panel de visualización
     public void OnPointerClick(PointerEventData eventData)
     {    
        card = gameObject;
        GameObject.Find("CardViewPanel").GetComponent<Image>().sprite = card.GetComponent<Image>().sprite;
     }
}