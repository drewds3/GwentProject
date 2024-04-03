using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IncreaseSlots : MonoBehaviour, IDropHandler
{
   //Variables para controlar que las cartas se suelten en las casillas correctas
    public string faction;

    //Variables para soltar la carta en las slots
    public GameObject DragParent;
    public GameObject item;

    //Variables para el sistema de turnos
    public GameObject nextTurnPanel;
    public GameObject blockHand;
    public GameObject blockDeck;
    public GameObject passButtonBlock;

    //Variables para saber si se pasó el otro jugador
    private int count = 0;
    public GameObject passButton;

    //Variables para controlar el efecto del clima
    public string type;
    public bool increaseM = false;
    public bool increaseR = false;
    public bool increaseS = false;

    //Variable para mandar al cementerio los aumentos usados
    public Transform graveyard;

    void Update()
    {
        //Se actualiza el contador de pase
        count = passButton.GetComponent<PassButton>().passCount;
    }

    //Método para verificar si sueltas una carta en la casilla correcta y pasar de turno
    public void OnDrop(PointerEventData eventData)
    {   
        Carta cardScript = DragParent.GetComponentInChildren<Carta>();
        
        //Comprueba si la carta soltada es válida
        if(cardScript && faction == cardScript.faction && cardScript.typeCard == "Increase")
        {       
            //Zona a la que afecta
            if(type == "M")
            {
                increaseM = true;

                Debug.Log("Fila M aumentada");
            }
            else if(type == "R")
            {
                increaseR = true;

                Debug.Log("Fila R aumentadas");
            }
            else
            {
                increaseS = true;

                Debug.Log("Fila S aumentadas");
            } 

            //Además pasa de turno si el otro jugador no ha pasado
            if(count%2==0)
            {
                    nextTurnPanel.SetActive(!nextTurnPanel.activeSelf);
                    blockHand.SetActive(!blockHand.activeSelf);
                    blockDeck.SetActive(!blockDeck.activeSelf);
                    passButtonBlock.SetActive(!passButtonBlock.activeSelf);
            }

            //Al ser usada se va al cementerio y pasa de turno
            DragHandler.itemDragging.transform.SetParent(graveyard);
            DragHandler.itemDragging.transform.position = graveyard.position;
        }
    }
}
