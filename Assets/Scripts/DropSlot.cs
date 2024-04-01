using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    //Variables para controlar que las cartas se suelten en las casillas correctas
    public string faction;
    public string type;

    //Variables para soltar la carta en las slots
    public GameObject DragParent;
    public GameObject item;

    //Variables para el sistema de turnos
    public GameObject nextTurnPanel;
    public GameObject blockHand;
    public GameObject blockDeck;
    public GameObject passButtonBlock;

    //Variables para el sistema de rondas
    private int count = 0;
    public GameObject passButton;

    //Método para verificar si sueltas una carta en la casilla correcta y pasar de turno
    public void OnDrop(PointerEventData eventData)
    {   
        Carta cardScript = DragParent.GetComponentInChildren<Carta>();
        
        //Comprueba si la carta soltada es válida y no hay otra carta
        if(cardScript && faction == cardScript.faction && !item && (type == cardScript.typeCard || type == cardScript.typeCard2 || type == cardScript.typeCard3))
        {
                /* Si no es así, establece el objeto que se está arrastrando
                como el objeto de la slot y lo posiciona en la slot
                */
                item = DragHandler.itemDragging;
                item.transform.SetParent(transform);
                item.transform.position = transform.position;

                /*Se le otorga un tag dependiendo de su facción para luego
                ser enviada a sus respectivos cementerios desde otro script*/
                if(cardScript.faction == "Dragon")
                {
                    item.tag = "CartaJugada1";
                }
                else if(cardScript.faction == "Raven")
                {
                    item.tag = "CartaJugada2";
                }
               
                Debug.Log("Carta colocada correctamente");

                //Se le quita la movilidad a la carta
                item.GetComponent<DragHandler>().enabled = false;

                //Además pasa de turno si el otro jugador no ha pasado
                if(count%2==0)
                {
                    nextTurnPanel.SetActive(!nextTurnPanel.activeSelf);
                    blockHand.SetActive(!blockHand.activeSelf);
                    blockDeck.SetActive(!blockDeck.activeSelf);
                    passButtonBlock.SetActive(!passButtonBlock.activeSelf);
                }
        }
    }

    void Update()
    {
        //Se actualiza el contador de pase
        count = passButton.GetComponent<PassButton>().passCount;

        //Si se remueve la carta del slot, esta queda habilitada para poner otra
        if (item != null && item.transform.parent != transform)
        {
            item = null;

            Debug.Log("La carta ha sido removida");
        }
    }
}

