using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour, IPointerEnterHandler
{
     //Propiedades de la carta a visualizar
     public string description;

     //Al pasar el cursor por encima se llama al método
     public void OnPointerEnter(PointerEventData eventData)
     {    
         ViewCard();
     }

    //Método para que al pasar el cursor por encima se muestre en el panel de visualización
     public void ViewCard()
     {
        //Se le asignan los sprites del diseño de la cartas y de la imagen de la misma
        GameObject.Find("CardViewPanel").GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
        GameObject.Find("CVImagen").GetComponent<Image>().sprite = transform.Find("Imagen").GetComponent<Image>().sprite;
        
        //Se le asignan las propiedades
        CardImages data = GameObject.Find("CardViewPanel").GetComponent<CardImages>();
        Card card = gameObject.GetComponent<Card>();

        //Se le asigna el nombre de la carta
        GameObject.Find("CVNombre").GetComponent<TMP_Text>().text = card.cardName;

        //Tipo de carta
        if(card.typeCard == "Silver" || card.typeCard == "Gold" || card.typeCard == "Lure") 
        GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.powerZone;
        else if(card.typeCard == "Cleareance")
        GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.clearanceSymbol;
        else if(card.typeCard == "Climate")
        GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.climateSymbol;
        else if(card.typeCard == "Increase")
        GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.increaseSymbol;
        else if(card.typeCard == "Leader")
        GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.leaderSymbol;
        else GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.nullImage;

        //Verifica si es Oro o Señuelo
        if(card.typeCard == "Gold") GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.goldSymbol;
        else if(card.typeCard == "Lure") GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.lureSymbol;
        else GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.nullImage;

        //Asigna el rango
        if(card.typeCard == "Gold")
        {
            if(card.typeCard2 == "Melee") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.typeCard2 == "Ranged") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.typeCard2 == "Siege") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.nullImage;

            if(card.typeCard3 == "Melee") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.typeCard3 == "Ranged") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.typeCard3 == "Siege") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.nullImage;

            if(card.typeCard4 == "Melee") GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.typeCard4 == "Ranged") GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.typeCard4 == "Siege") GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.nullImage;
        }
        else if(card.typeCard == "Silver")
        {
            if(card.typeCard == "Melee") GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.typeCard == "Ranged") GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.typeCard == "Siege") GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.nullImage;

            if(card.typeCard2 == "Melee") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.typeCard2 == "Ranged") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.typeCard2 == "Siege") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.nullImage;

            if(card.typeCard3 == "Melee") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.typeCard3 == "Ranged") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.typeCard3 == "Siege") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.nullImage;

            GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.nullImage;
        }
        else if(card.typeCard == "Lure")
        {
            GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.nullImage;
            GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.nullImage;
            GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.nullImage;
        }
        else
        {   
            GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.nullImage;
            GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.nullImage;
            GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.nullImage;
            GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.nullImage;
        }

        //Se le asigna la facción
        if(card.faction == "Dragon") GameObject.Find("CVFacción").GetComponent<Image>().sprite = data.dragonFaction;
        else if(card.faction == "Raven") GameObject.Find("CVFacción").GetComponent<Image>().sprite = data.ravenFaction;
        else GameObject.Find("CVFacción").GetComponent<Image>().sprite = data.nullImage;
    
        //Comprueba si es necesario mostrar el poder de la carta
        if(card.typeCard == "Silver" || card.typeCard == "Gold" || card.typeCard == "Lure")
        {
            GameObject.Find("CVPuntos").GetComponent<TMP_Text>().text = $"{card.puntosPoder}";
        }
        else
        {
            GameObject.Find("CVPuntos").GetComponent<TMP_Text>().text = "";
        }

        //Se le asigna la descripción
        GameObject.Find("CVDescripción").GetComponent<TMP_Text>().text = transform.Find("Descripción").GetComponent<TMP_Text>().text;
     }
}