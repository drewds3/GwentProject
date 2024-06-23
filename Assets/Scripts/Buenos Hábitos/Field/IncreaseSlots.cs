using UnityEngine;
using UnityEngine.EventSystems;
using static SetPlayers;

public class IncreaseSlots : MonoBehaviour, IDropHandler
{
   //Variables para controlar que las cartas se suelten en las casillas correctas
    public bool isPlayer1Slot;
    private string faction;
    private bool isP1Neutral;
    private bool isP2Neutral;

    //Variables para soltar la carta en las slots
    public GameObject DragParent;

    //Variables para controlar el aumento
    public string type;
    public bool increase = false;
    public GameObject[] slots = new GameObject[6];

    //Variable para mandar al cementerio los aumentos usados
    private Transform graveyard;

    //Método para verificar si sueltas una carta en la casilla correcta y pasar de turno
    public void OnDrop(PointerEventData eventData)
    {   
        //Comprueba de quién es el slot
        if(isPlayer1Slot)
        {
            faction = player1.Faction;
            graveyard = player1.Graveyard;
        } 
        else 
        {
            faction = player2.Faction;
            graveyard = player2.Graveyard;
        }

        Card cardScript = DragParent.GetComponentInChildren<Card>();

        //Comprueba si es una carta de aumento de facción "Neutral" y es la casilla correcta
        if(DragHandler.startParent == GameObject.Find("Hand").transform && cardScript.Faction == "Neutral" && isPlayer1Slot) isP1Neutral = true;
        else if(DragHandler.startParent == GameObject.Find("HandEnemy").transform && cardScript.Faction == "Neutral" && !isPlayer1Slot) isP2Neutral = true;
        
        //Comprueba si la carta soltada es válida
        if(cardScript && (faction == cardScript.Faction || isP1Neutral || isP2Neutral) && cardScript.Type == "Increase")
        {   
            //Activa los métodos que incrementan en cada carta
            for(int i = 0; i < 6; i++)
            {
                slots[i].GetComponent<DropSlot>().Increase();
            }

            Debug.Log($"Fila {type} aumentada");

            TurnsBasedSystem turnsController = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>();

            //Además pasa de turno si el otro jugador no ha pasado
            if(turnsController.passCount % 2 == 0)
            {
                turnsController.NextTurn();
            }

            //Al ser usada se va al cementerio y pasa de turno
            DragHandler.itemDragging.transform.SetParent(graveyard);
            DragHandler.itemDragging.transform.position = graveyard.position;
        }
    }
}
