using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PanelCartas : MonoBehaviour
{
    //Variables para el sistema de puntuación
    public int puntosTotal;

    void Start()
    {

    }
    void Update()
    {
        //Se llaman a los métodos
        CalcularPuntosTotal();
        UnDraggeable();
        
    }

    //Método para calcular la puntuación en la fila
    void CalcularPuntosTotal()
    {
        puntosTotal = 0;
        Carta[] cartas = GetComponentsInChildren<Carta>();

        foreach (Carta carta in cartas)
        {
            puntosTotal += carta.puntosPoder;
        }
    }

    //Método para que las cartas dejen de ser movibles
    public void UnDraggeable()
    {
        DragHandler[] dragHandlers = GetComponentsInChildren<DragHandler>();

        foreach(DragHandler dragHandler in  dragHandlers)
        {
            dragHandler.enabled = false;
        }
    }
}