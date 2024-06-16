using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour, IPointerEnterHandler
{
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
        GameObject.Find("CVNombre").GetComponent<TMP_Text>().text = card.Name;

        //Tipo de carta
        if(card.Type == "Silver" || card.Type == "Gold" || card.Type == "Lure") 
        GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.powerZone;
        else if(card.Type == "Clearance")
        GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.clearanceSymbol;
        else if(card.Type == "Climate")
        GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.climateSymbol;
        else if(card.Type == "Increase")
        GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.increaseSymbol;
        else if(card.Type == "Leader")
        GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.leaderSymbol;
        else GameObject.Find("CVPuntosDePoder").GetComponent<Image>().sprite = data.nullImage;

        //Verifica si es Oro o Señuelo
        if(card.Type == "Gold") GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.goldSymbol;
        else if(card.Type == "Lure") GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.lureSymbol;
        else GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.nullImage;

        //Asigna el rango
        if(card.Type == "Gold")
        {
            if(card.Range1 == "Melee") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.Range1 == "Ranged") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.Range1 == "Siege") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.nullImage;

            if(card.Range2 == "Melee") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.Range2 == "Ranged") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.Range2 == "Siege") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.nullImage;

            if(card.Range3 == "Melee") GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.Range3 == "Ranged") GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.Range3 == "Siege") GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.nullImage;
        }
        else if(card.Type == "Silver")
        {
            if(card.Range1== "Melee") GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.Range1 == "Ranged") GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.Range1 == "Siege") GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta").GetComponent<Image>().sprite = data.nullImage;

            if(card.Range2 == "Melee") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.Range2 == "Ranged") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.Range2 == "Siege") GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta2").GetComponent<Image>().sprite = data.nullImage;

            if(card.Range3 == "Melee") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.meleeSymbol;
            else if(card.Range3 == "Ranged") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.rangedSymbol;
            else if(card.Range3 == "Siege") GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.siegeSymbol;
            else GameObject.Find("CVTipoDeCarta3").GetComponent<Image>().sprite = data.nullImage;

            GameObject.Find("CVTipoDeCarta4").GetComponent<Image>().sprite = data.nullImage;
        }
        else if(card.Type== "Lure")
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
        if(card.Faction == "Dragon") GameObject.Find("CVFacción").GetComponent<Image>().sprite = data.dragonFaction;
        else if(card.Faction == "Raven") GameObject.Find("CVFacción").GetComponent<Image>().sprite = data.ravenFaction;
        else GameObject.Find("CVFacción").GetComponent<Image>().sprite = data.nullImage;
    
        //Comprueba si es necesario mostrar el poder de la carta
        if(card.Type == "Silver" || card.Type == "Gold" || card.Type == "Lure")
        {
            GameObject.Find("CVPuntos").GetComponent<TMP_Text>().text = $"{card.Power}";
        }
        else
        {
            GameObject.Find("CVPuntos").GetComponent<TMP_Text>().text = "";
        }

        //Se le asigna la descripción
        GameObject.Find("CVDescripción").GetComponent<TMP_Text>().text = transform.Find("Descripción").GetComponent<TMP_Text>().text;
     }
}