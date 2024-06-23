using UnityEngine;
using UnityEngine.EventSystems;
using static SetPlayers;

public class ClimateSlots : MonoBehaviour, IDropHandler
{
    //Variables para controlar el efecto del clima
    public string type;
    public bool afectM = false;
    public bool afectR = false;
    public bool afectS = false;

    //Variables para soltar la carta en las slots
    public GameObject DragParent;
    public GameObject item;

    void Update()
    {
        //Si se remueve la carta del slot, esta queda habilitada para poner otra
        if(item != null && item.transform.parent != transform)
        {
            item = null;

            //Deja de hacer efecto el clima
            afectM = false;
            afectR = false;
            afectS = false;

            Debug.Log("Clima removido");
        }
    }

    //Método para verificar si sueltas una carta en la casilla correcta y pasar de turno
    public void OnDrop(PointerEventData eventData)
    {   
        Card cardScript = DragParent.GetComponentInChildren<Card>();

        //Comprueba si la carta soltada es válida y no hay otra carta
        if(!item && cardScript.Type == "Climate")
        {
            /* Si no es así, establece el objeto que se está arrastrando
            como el objeto de la slot y lo posiciona en la slot
            */
            item = DragHandler.itemDragging;
            item.transform.SetParent(transform);
            item.transform.position = transform.position;

            item.transform.rotation = Quaternion.Euler(0, 0, -90);

            /*Se le otorga un tag dependiendo de su facción para luego
            ser enviada a sus respectivos cementerios desde otro script*/
            if(DragHandler.startParent == GameObject.Find("Hand").transform)
            {
                item.tag = "ClimaJugado1";
            }
            else if(DragHandler.startParent == GameObject.Find("HandEnemy").transform)
            {
                item.tag = "ClimaJugado2";
            }
               
            Debug.Log("Carta colocada correctamente");

            //Se le quita la movilidad a la carta
            item.GetComponent<DragHandler>().enabled = false;

            TurnsBasedSystem turnsController = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>();

            //Además pasa de turno si el otro jugador no ha pasado
            if(turnsController.passCount%2==0)
            {
                turnsController.NextTurn();  
            }

            //Zona a la que afecta
            if(type == "M")
            {
                afectM = true;

                Debug.Log("Filas M afectadas");
            }
            else if(type == "R")
            {
                afectR = true;

                Debug.Log("Filas R afectadas");
            }
            else
            {
                afectS = true;

                Debug.Log("Filas S afectadas");
            } 
                
        }
        //Si se usa el despeje se elimina el clima
        else if(cardScript.Type == "Clearance" && item != null)
        {
            if(item.tag == "ClimaJugado1")
            {
                item.transform.SetParent(player1.Graveyard);
                item.transform.position = player1.Graveyard.position;
                item.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if(item.tag == "ClimaJugado2")
            {
                item.transform.SetParent(player2.Graveyard);
                item.transform.position = player2.Graveyard.position;
                item.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            TurnsBasedSystem turnsController = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>();

            //Al ser usada se va al cementerio y pasa de turno
            if(DragHandler.startParent == GameObject.Find("Hand").transform)
            {
                DragHandler.itemDragging.transform.SetParent(player1.Graveyard);
                DragHandler.itemDragging.transform.position = player1.Graveyard.position;
            }
            else
            {
                DragHandler.itemDragging.transform.SetParent(player2.Graveyard);
                DragHandler.itemDragging.transform.position = player2.Graveyard.position;
            }

            //Si nadie se ha pasado cambia el turno
            if(turnsController.passCount%2==0)
            {
                turnsController.NextTurn();
            }
        }
    }
}