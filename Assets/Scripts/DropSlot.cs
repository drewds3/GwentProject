using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    //Variables para controlar que las cartas se suelten en las casillas correctas
    public string faction = "Neutral";
    //private string cardFaction;

    //Variables para soltar la carta en las slots
    public GameObject DragParent;
    public GameObject item;

    //Método para verificar si sueltas una carta en la casilla correcta
    public void OnDrop(PointerEventData eventData)
    {   
        Carta cardScript = DragParent.GetComponentInChildren<Carta>();
        
        //Comprueba si la carta soltada es válida y no hay otra carta
        if(cardScript && faction == cardScript.deck && !item)
        {
                /* Si no es así, establece el objeto que se está arrastrando
                como el objeto de la slot y lo posiciona en la slot
                */
                item = DragHandler.itemDragging;

                item.transform.SetParent(transform);

                item.transform.position = transform.position;

                //cardFaction = cardScript.deck;

                Debug.Log("Carta colocada correctamente");
        }
    }

    void Update()
    {
        //Si se remueve la carta del slot, esta queda habilitada para poner otra
        if (item != null && item.transform.parent != transform)
        {
            item = null;

            Debug.Log("La carta ha sido removida");
        }
    }
}

