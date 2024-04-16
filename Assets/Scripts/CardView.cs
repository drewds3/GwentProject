using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour, IPointerClickHandler
{
     //Carta con el script
     public static GameObject card;

     //Propiedades de la carta a visualizar
     public Sprite cardImage;
     public Sprite puntosPoder;
     public Sprite typeCard1;
     public Sprite typeCard2;
     public Sprite typeCard3;
     public Sprite faction;
     public string nameCard;
     public int puntos;
     public string description;

     //Al hacerle click se muestra en el panel de visualización
     public void OnPointerClick(PointerEventData eventData)
     {    
         card = gameObject;

         GameObject.Find("CardViewPanel").GetComponent<Image>().sprite = card.GetComponent<Image>().sprite;

         GameObject.Find("CVImagen").GetComponent<Image>().sprite = cardImage;
         GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = puntosPoder;
         GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = typeCard1;
         GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = typeCard2;
         GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = typeCard3;
         GameObject.Find("CVFacción").GetComponent<Image>().sprite = faction;

         puntos = gameObject.GetComponent<Carta>().puntosPoder;

         GameObject.Find("CVPuntos").GetComponent<TMP_Text>().text = $"{puntos}";
         GameObject.Find("CVNombre").GetComponent<TMP_Text>().text = nameCard;
         GameObject.Find("CVDescripción").GetComponent<TMP_Text>().text = description;
     }
}