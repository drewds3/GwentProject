using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* Definimos una clase pública llamada DropSlot que hereda de MonoBehaviour y
  también implementa la interfaz IDropHandler
*/
public class DropSlot : MonoBehaviour, IDropHandler
{
    public string faction = "Neutral";
    private string cardFaction;

    public GameObject DragParent;

    // Declaramos una variable pública item de tipo GameObject
    public GameObject item;

    // Este método se llama cuando se suelta un objeto sobre la slot
    public void OnDrop(PointerEventData eventData)
    {   
        // Obtenemos el componente Carta del gameobject en el que se está arrastrando
        Carta cardScript = DragParent.GetComponentInChildren<Carta>();
        
        // Comprueba si la slot ya contiene un objeto
        if(cardScript && faction == cardScript.deck)
        {
            if(!item)
            {
                /* Si no es así, establece el objeto que se está arrastrando
                como el objeto de la slot y lo posiciona en la slot
                */
                item = DragHandler.itemDragging;

                item.transform.SetParent(transform);

                item.transform.position = transform.position;

                cardFaction = cardScript.deck;
            }
        }
    }

    // Este método se ejecuta en cada fotograma y comprueba si el objeto de la slot ya no es nuestro hijo directo
    void Update()
    {
        // Usamos la variable cardFaction para evitar errores al usar el GetComponentInChildren, ya que es una variable privada
        if (item && cardFaction != null && cardFaction != faction && item.transform.parent != transform)
        {
            // Si ya no es hijo, establece el objeto como null para permitir que se suelte un nuevo objeto en la slot
            item = null;
        }
        else if (item && cardFaction == null)
        {
            // Si el cardScript no existe en el gameobject, se iguala la cardFaction a faction
            cardFaction = faction;
        }
        
        // Actualizamos la variable cardFaction si hay un gameobject con el script Carta en el objeto DragParent
        if (DragParent && DragParent.transform.childCount > 0) cardFaction = DragParent.GetComponentInChildren<Carta>()?.deck;
    }
}

