using UnityEngine;
using UnityEngine.EventSystems;

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

        if(GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>().currentTurn == turnLimit) gameObject.SetActive(false);
    }

    //Método para cambiar la carta por otra al soltarla en el slot
    public void OnDrop(PointerEventData eventData)
    {  
        DrawCards cardScript = deck.GetComponent<DrawCards>();
        
        //Comprueba si hay una carta
        if(!item)
        {       
            //De no ser así, cambia la carta por otra
            item = DragHandler.itemDragging;
            item.transform.SetParent(transform);
            item.transform.position = transform.position;

            cardScript.OnClick();
                
            cardScript.Swap();
        }
    }
}