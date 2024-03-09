using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*Se define una clase DragHandler que implementa las interfaces IDragHandler, IEndDragHandler y IBeginDragHandler.
 Estas interfaces se utilizan para controlar el comportamiento del objeto durante el arrastre.
 */
public class DragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    /*La variable Vector3 almacena la posición inicial, en itemDragging se almacena el objeto mientras está siendo arratrado.
    La variable Transform almacena el padre del objeto antes de ser arrastrado.
    La variable Transform almacena el padre especial para el objeto durante el arrastre.
    */
    public static GameObject itemDragging;
    Vector3 startPosition;

    Transform startParent;

    Transform dragParent;
    void Start()
    {
        //Busca un objeto en la escena con la etiqueta "DragParent" y lo asigna a la variable dragParent
        dragParent = GameObject.FindGameObjectWithTag("DragParent").transform;
    }

    /*
    Implementación del método OnBeginDrag de la interfaz IDragHandler.
    Este método se ejecuta cuando el usuario comienza a arrastrar un objeto con el que ha interactuado.
    */
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");

        /*Almacena el objeto que está siendo arrastrado en la variable itemDragging y
         guarda su posición inicial y su padre actual en las variables startPosition y startParent, respectivamente.
        */
        itemDragging = gameObject;

        startPosition = transform.position;

        startParent = transform.parent;

        /*Establece el padre del objeto en dragParent, el cual fue definido en el método Start().
         Esto permite que el objeto siga al cursor durante el arrastre.
        */
        transform.SetParent(dragParent);
    }

    /*
    Implementación del método OnDrag de la interfaz IDragHandler.
    Este método se ejecuta mientras el usuario arrastra el objeto con el que ha interactuado.
    */
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");

        /*Actualiza la posición del objeto al valor de la posición del mouse.*/
        transform.position = Input.mousePosition;
    }

    /*
    Implementación del método OnEndDrag de la interfaz IEndDragHandler.
    Este método se ejecuta cuando el usuario suelta el objeto que ha estado arrastrando.
    */
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");

        /*Restablece la variable itemDragging a null, ya que el usuario ha soltado el objeto.*/
        itemDragging = null;

        /*Si el padre actual del objeto es igual a dragParent, es decir, el objeto aún está siendo arrastrado,
         entonces se restaura su posición y su padre originales.
        */
        if (transform.parent == dragParent)
        {
            transform.position = startPosition;

            transform.SetParent(startParent);
        }
    }

    void Update()
    {
        
    }
}

/*
En resumen, este código permite arrastrar un objeto en la escena de Unity con el mouse.
Cuando el usuario comienza a arrastrar el objeto, el código guarda su posición inicial y su padre original.
Luego, el objeto sigue al cursor mientras el usuario lo arrastra. Cuando el usuario suelta el objeto,
el código restaura su posición y su padre originales.
*/