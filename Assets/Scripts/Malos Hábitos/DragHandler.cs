using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    //Variables para arrastrar la carta y almacenar la posición inicial
    public static GameObject itemDragging;
    Vector3 startPosition;
    public static Transform startParent;
    Transform dragParent;

    void Start()
    {
        //Busca un objeto en la escena con la etiqueta "DragParent" y lo asigna a la variable dragParent
        dragParent = GameObject.FindGameObjectWithTag("DragParent").transform;
    }

    //Al comenzar el arrastre se guardan el padre y posición iniciales
    public void OnBeginDrag(PointerEventData eventData)
    {
        itemDragging = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        transform.SetParent(dragParent);

        Debug.Log("OnBeginDrag");
    }

    //Mientras se arrastra se actualiza la posición de la carta a la del puntero
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;

        Debug.Log("OnDrag");
    }

    //Dependiendo de dónde se halla dejado la carta se procede de una forma u otra
    public void OnEndDrag(PointerEventData eventData)
    {
        //Se restablece la variable a null puesto que el usuario ha soltado la carta
        itemDragging = null;

        Debug.Log("OnEndDrag");

        if (transform.parent == dragParent)
        {
            transform.position = startPosition;

            transform.SetParent(startParent);
        }
    }
}