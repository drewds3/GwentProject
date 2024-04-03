using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCard : MonoBehaviour, IDropHandler
{
    //Variables para soltar la carta en las slots
    public GameObject DragParent;
    public GameObject item;
    public GameObject deck;

    //Método para cambiar la carta por otra al soltarla en el slot
    public void OnDrop(PointerEventData eventData)
    {  
        DrawCards cardScript = deck.GetComponent<DrawCards>();
        
        //Comprueba si hay una carta
        if(!item)
        {
                item = DragHandler.itemDragging;
                item.transform.SetParent(transform);
                item.transform.position = transform.position;

                cardScript.OnClick();
                
                cardScript.Swap();
        }
    }

    void Update()
    {
        //Si se remueve la carta del slot, esta queda habilitada para poner otra
        if (item != null && item.transform.parent != transform)
        {
            item = null;

            Debug.Log("La carta ha sido removida");
        }
    }
}

