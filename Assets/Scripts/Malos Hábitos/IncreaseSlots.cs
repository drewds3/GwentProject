using UnityEngine;
using UnityEngine.EventSystems;

public class IncreaseSlots : MonoBehaviour, IDropHandler
{
   //Variables para controlar que las cartas se suelten en las casillas correctas
    public string faction;

    //Variables para soltar la carta en las slots
    public GameObject DragParent;

    //Variables para controlar el aumento
    public string type;
    public bool increase = false;
    public GameObject[] slots = new GameObject[6];

    //Variable para mandar al cementerio los aumentos usados
    public Transform graveyard;

    //Método para verificar si sueltas una carta en la casilla correcta y pasar de turno
    public void OnDrop(PointerEventData eventData)
    {   
        Card cardScript = DragParent.GetComponentInChildren<Card>();
        
        //Comprueba si la carta soltada es válida
        if(cardScript && faction == cardScript.Faction && cardScript.Type == "Increase")
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
