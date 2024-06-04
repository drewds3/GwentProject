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

    //Variables para el sistema de turnos
    public GameObject nextTurnPanel;
    public GameObject blockHand;
    public GameObject blockDeck;
    public GameObject passButtonBlock;
    public GameObject leaderBlock;

    //Variables para saber si se pasó el otro jugador
    private int count = 0;
    public GameObject passButton;

    //Variables para controlar el aumento
    public string type;
    public bool increase = false;
    public GameObject[] slots = new GameObject[6];

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
            //Activa los métodos que incrementan en cada carta
            for(int i = 0; i < 6; i++)
            {
                slots[i].GetComponent<DropSlot>().Increase();
            }

            Debug.Log($"Fila {type} aumentada");

            //Además pasa de turno si el otro jugador no ha pasado
            if(count%2==0)
            {
                    nextTurnPanel.SetActive(!nextTurnPanel.activeSelf);
                    blockHand.SetActive(!blockHand.activeSelf);
                    blockDeck.SetActive(!blockDeck.activeSelf);
                    passButtonBlock.SetActive(!passButtonBlock.activeSelf);
                    leaderBlock.SetActive(true);
            }

            //Al ser usada se va al cementerio y pasa de turno
            DragHandler.itemDragging.transform.SetParent(graveyard);
            DragHandler.itemDragging.transform.position = graveyard.position;
        }
    }
}
