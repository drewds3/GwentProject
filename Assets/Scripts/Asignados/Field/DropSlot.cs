using UnityEngine;
using UnityEngine.EventSystems;
using static SetPlayers;

public class DropSlot : MonoBehaviour, IDropHandler
{
    //Variables para controlar que las cartas se suelten en las casillas correctas
    public bool isPlayer1Slot;
    private string faction;
    public string type;
    private bool isP1Neutral;
    private bool isP2Neutral;

    //Variables para soltar la carta en las slots
    public GameObject DragParent;
    public GameObject item;

    //Variables para el sistema de señuelos
    public Transform hand;

    void Update()
    {
        //Si se remueve la carta del slot, esta queda habilitada para poner otra
        if (item != null && item.transform.parent != transform)
        {
            item = null;

            Debug.Log("La carta ha sido removida");
        }
    }

    //Método para verificar si sueltas una carta en la casilla correcta y pasar de turno
    public void OnDrop(PointerEventData eventData)
    {   
        //Comprueba de quién es el slot
        if(isPlayer1Slot) faction = player1.Faction;
        else faction = player2.Faction;

        Card cardScript = DragParent.GetComponentInChildren<Card>();

        //Comprueba si es una carta de unidad de facción "Neutral" y es la casilla correcta
        if(DragHandler.startParent == GameObject.Find("Hand").transform && cardScript.Faction == "Neutral" && isPlayer1Slot) isP1Neutral = true;
        else if(DragHandler.startParent == GameObject.Find("HandEnemy").transform && cardScript.Faction == "Neutral" && !isPlayer1Slot) isP2Neutral = true;

        TurnsBasedSystem turnsController = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>();
        
        //Comprueba si la carta soltada es un señuelo válido y si hay una carta tipo Plata en el slot
        if(cardScript.Type == "Lure" && item != null && (cardScript && faction == cardScript.Faction || isP1Neutral || isP2Neutral) && item.GetComponent<Card>().Type == "Silver")
        {       
                //Le activa el arrastre a la carta en la slot y la manda a la mano
                item.GetComponent<DragHandler>().enabled = true;

                item.transform.SetParent(hand);
                item.transform.position = hand.position;
                item.transform.rotation = Quaternion.Euler(0, 0, 0);
                
                //Se le otorga otro tag para que no se elimine al cementerio al finalizar la ronda
                item.tag = "Carta";

                //Establece como el objeto de la slot al señuelo
                item = DragHandler.itemDragging;
                item.transform.SetParent(transform);
                item.transform.position = transform.position;

                //Se le quita la movilidad al señuelo
                item.GetComponent<DragHandler>().enabled = false;

                /*Se le otorga un tag dependiendo de su facción para luego
                ser enviada a sus respectivos cementerios desde otro script*/
                if(cardScript.Faction == player1.Faction || isP1Neutral)
                {
                    item.tag = "CartaJugada1";
                }
                else if(cardScript.Faction == player2.Faction || isP2Neutral)
                {
                    item.tag = "CartaJugada2";
                }
               
                Debug.Log("Señuelo colocada correctamente");


                //Además pasa de turno si el otro jugador no ha pasado
                if(turnsController.passCount % 2 == 0)
                {
                   turnsController.NextTurn();
                }
        }
        //De lo contrario comprueba si la carta soltada es válida y no hay otra
        else if((cardScript && faction == cardScript.Faction || isP1Neutral || isP2Neutral) && !item && (type == cardScript.Range1 || type == cardScript.Range2 || type == cardScript.Range3))
        {
                //De ser así, verifica si la carta tiene efecto que se deba activar en este momento
                if(cardScript.NumEffect < 4 && cardScript.NumEffect != 0)
                {
                    EffectCards effects = DragHandler.itemDragging.GetComponent<EffectCards>();

                    //De ser así, activa el efecto en cuestión
                    effects.ActiveEffect(cardScript.NumEffect);
                }

                /* Luego, establece el objeto que se está arrastrando
                como el objeto de la slot y lo posiciona en la slot
                */
                item = DragHandler.itemDragging;
                item.transform.SetParent(transform);
                item.transform.position = transform.position;

                /*Se le otorga un tag dependiendo de su facción para luego
                ser enviada a sus respectivos cementerios desde otro script*/
                if(cardScript.Faction == player1.Faction || isP1Neutral)
                {
                    item.tag = "CartaJugada1";
                }
                else if(cardScript.Faction == player2.Faction || isP2Neutral)
                {
                    item.tag = "CartaJugada2";
                }
               
                //Se le quita la movilidad a la carta
                item.GetComponent<DragHandler>().enabled = false;
                
                //Verifica si hay algún efecto que se deba activar en este momento
                if(cardScript.NumEffect > 3)
                {
                    EffectCards effects = DragHandler.itemDragging.GetComponent<EffectCards>();

                    //De ser así, activa el efecto en cuestión
                    effects.ActiveEffect(cardScript.NumEffect);
                }

                Debug.Log("Carta colocada correctamente");

                //Además pasa de turno si el otro jugador no ha pasado
                if(turnsController.passCount % 2 == 0)
                {
                   turnsController.NextTurn();
                }
        }
    }

    //Método que se llama al activarse un aumento en la fila
    public void Increase()
    {
        //Se duplica el poder de la carta en este slot si y solo si tiene una carta y es de tipo Plata
        if(item != null)
        {
            Card cardScript = item.GetComponent<Card>();

            if(cardScript.Type == "Silver")
            {
                cardScript.IncreasePower(2);
            }
        }
    }
}