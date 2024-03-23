using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* Definimos una clase pública llamada DropSlot que hereda de MonoBehaviour y
  también implementa la interfaz IDropHandler
*/
public class DropSlot : MonoBehaviour, IDropHandler
{
    // Declaramos una variable pública item de tipo GameObject
    public GameObject item;

    // Este método se llama cuando se suelta un objeto sobre la slot
    public void OnDrop(PointerEventData eventData)
    {   
        // Comprueba si la slot ya contiene un objeto
        if(!item)
        {
             /* Si no es así, establece el objeto que se está arrastrando
              como el objeto de la slot y lo posiciona en la slot
             */
            item = DragHandler.itemDragging;

            item.transform.SetParent(transform);

            item.transform.position = transform.position;
        }
    }

    // Este método se ejecuta en cada fotograma y comprueba si el objeto de la slot ya no es nuestro hijo directo
    void Update()
    {
        if (item != null && item.transform.parent != transform)
        {
            // Si ya no lo es, establece el objeto como null para permitir que se suelte un nuevo objeto en la slot
            item = null;
        }
    }
}

/* En resumen, este script gestiona una "slot" donde se pueden soltar objetos arrastrados. 
Cuando se suelta un objeto sobre la slot, se asigna como el objeto de la slot y se posiciona en la misma.
Si el objeto de la slot ya no es su hijo directo (por ejemplo, si se ha soltado en otra slot), 
entonces el objeto se elimina de la slot actual y queda disponible para ser recogido y
soltado en otra parte del juego. La slot se representa con un objeto GameObject en Unity y el script
se asegura de que solo haya un objeto en la slot en cualquier momento.
*/
