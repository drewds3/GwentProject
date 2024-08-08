using UnityEngine;
using UnityEngine.EventSystems;
using static TurnsBasedSystem;

public class ChangeCard : MonoBehaviour, IDropHandler
{
    //Variables para soltar la carta en las slots
    public GameObject item;
    public GameObject deck;

    //Turno en el que se desactiva la posibilidad de cambio
    public int turnLimit;

     void Update()
    {
        //Luego del cambio, se habilita la slot otra vez
        if (item != null && item.transform.parent != transform)
        {
            item = null;

            Debug.Log("La carta ha sido removida");
        }

        if(currentTurn == turnLimit || round > 1) gameObject.SetActive(false);
    }

    //Método para cambiar la carta por otra al soltarla en el slot
    public void OnDrop(PointerEventData eventData)
    {  
        Deck playerDeck = deck.GetComponent<Deck>();
        
        //Comprueba si hay una carta
        if(!item)
        {       
            //De no ser así, cambia la carta por otra
            item = DragHandler.itemDragging;
            item.transform.SetParent(transform);
            item.transform.position = transform.position;

            playerDeck.OnClick();
                
            playerDeck.Swap();
        }
    }
}