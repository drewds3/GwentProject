using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PanelCartas : MonoBehaviour
{
    void Update()
    {
        CalcularPuntosTotal();
        UnDraggeable();
    }

    public int puntosTotal;

    void CalcularPuntosTotal()
    {
        puntosTotal = 0;
        Carta[] cartas = GetComponentsInChildren<Carta>();

        foreach (Carta carta in cartas)
        {
            puntosTotal += carta.puntosPoder;
        }
    }

    public void ResetearPuntos()
    {
        puntosTotal = 0;
    }

    public void UnDraggeable()
    {
        DragHandler[] dragHandlers = GetComponentsInChildren<DragHandler>();

        foreach(DragHandler dragHandler in  dragHandlers)
        {
            dragHandler.enabled = false;
        }
    }
}