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
    public GameObject leaderBlock;

    //Variables para el sistema de rondas
    private int count = 0;
    public GameObject passButton;

    //Variables para el sistema de aumentos
    public GameObject increaseSlot;
    public bool increaseOn = false;

    //Variables para el sistema de señuelos
    public Transform hand;

    //Método para verificar si sueltas una carta en la casilla correcta y pasar de turno
    public void OnDrop(PointerEventData eventData)
    {   
        Carta cardScript = DragParent.GetComponentInChildren<Carta>();
        
        //Comprueba si la carta soltada es un señuelo válido y si hay una carta tipo Plata en el slot
        if(cardScript.typeCard == "Lure" && item != null && cardScript && faction == cardScript.faction && item.GetComponent<Carta>().typeCard4 == "Silver")
        {       
                //Le activa el arrastre a la carta en la slot y la manda a la mano
                item.GetComponent<DragHandler>().enabled = true;

                item.transform.SetParent(hand);
                item.transform.position = hand.position;
                item.transform.rotation = Quaternion.Euler(0, 0, 0);

                //Establece como el objeto de la slot al señuelo
                item = DragHandler.itemDragging;
                item.transform.SetParent(transform);
                item.transform.position = transform.position;

                //Se le quita la movilidad al señuelo
                item.GetComponent<DragHandler>().enabled = false;

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
               
                Debug.Log("Señuelo colocada correctamente");


                //Además pasa de turno si el otro jugador no ha pasado
                if(count%2==0)
                {
                    nextTurnPanel.SetActive(!nextTurnPanel.activeSelf);
                    blockHand.SetActive(!blockHand.activeSelf);
                    blockDeck.SetActive(!blockDeck.activeSelf);
                    passButtonBlock.SetActive(!passButtonBlock.activeSelf);
                    leaderBlock.SetActive(true);
                }
        }
        //De lo contrario comprueba si la carta soltada es válida y no hay otra
        else if(cardScript && faction == cardScript.faction && !item && (type == cardScript.typeCard || type == cardScript.typeCard2 || type == cardScript.typeCard3))
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
                    leaderBlock.SetActive(true);
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

        //Si se activa un aumento en la fila se duplica el poder de la carta en este slot si y solo si la carta es tipo Plata
        if(increaseSlot.GetComponent<IncreaseSlots>().increase)
        {
            if(item != null)
            {
                Carta cardScript = item.GetComponent<Carta>();

                if(cardScript.typeCard4 == "Silver")
                {
                    cardScript.IncreasePower();

                    increaseOn = true;
                }
                else increaseOn = true;
            }
            else increaseOn = true;

        }
    }
}

