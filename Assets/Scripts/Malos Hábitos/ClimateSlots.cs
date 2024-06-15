using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClimateSlots : MonoBehaviour, IDropHandler
{
    //Variables para controlar que las cartas se suelten en las casillas correctas
    public string faction;
    public string type;

    //Variables para controlar el efecto del clima
    public string type2;
    public bool afectM = false;
    public bool afectR = false;
    public bool afectS = false;

    //Variables para soltar la carta en las slots
    public GameObject DragParent;
    public GameObject item;

    //Variables para enviar el despeje al cementerio
    public Transform graveyard1;
    public Transform graveyard2;

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
        if(!item && type == cardScript.typeCard)
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
            if(cardScript.faction == "Dragon")
            {
                item.tag = "ClimaJugado1";
            }
            else if(cardScript.faction == "Raven")
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
            if(type2 == "M")
            {
                afectM = true;

                Debug.Log("Filas M afectadas");
            }
            else if(type2 == "R")
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
        else if(cardScript.typeCard == "Clearance" && item != null)
        {
            if(item.tag == "CartaJugada1")
            {
                item.transform.SetParent(graveyard1);
                item.transform.position = graveyard1.position;
                item.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if(item.tag == "ClimaJugado2")
            {
                item.transform.SetParent(graveyard2);
                item.transform.position = graveyard2.position;
                item.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            TurnsBasedSystem turnsController = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>();

            //Al ser usada se va al cementerio y pasa de turno
            if(DragHandler.startParent == GameObject.Find("Hand").transform)
            {
                DragHandler.itemDragging.transform.SetParent(graveyard1);
                DragHandler.itemDragging.transform.position = graveyard1.position;
            }
            else
            {
                DragHandler.itemDragging.transform.SetParent(graveyard2);
                DragHandler.itemDragging.transform.position = graveyard2.position;
            }

            //Si nadie se ha pasado cambia el turno
            if(turnsController.passCount%2==0)
            {
                turnsController.NextTurn();
            }
        }
    }
}