using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PanelCartas : MonoBehaviour
{
    //para los puntos
     public int puntosTotal;
     
     //para el sistema de turnos
     public int previousChildCount;
     public GameObject NextTurnPanel;
     public GameObject Block1;
     public GameObject Block2;


    void Start()
    {
        previousChildCount = CountAllChildren(transform) - 6;
    }
    void Update()
    {
        CalcularPuntosTotal();
        UnDraggeable();
        
        int currentChildCount = CountAllChildren(transform) - 6;

        if (currentChildCount != previousChildCount)
        {
            previousChildCount = currentChildCount;
           
            Debug.Log($"En la zona hay actualmente {previousChildCount} cartas");

            NextTurnPanel.SetActive(!NextTurnPanel.activeSelf);
            Block1.SetActive(!Block1.activeSelf);
            Block2.SetActive(!Block2.activeSelf);
        }
    }

    void CalcularPuntosTotal()
    {
        puntosTotal = 0;
        Carta[] cartas = GetComponentsInChildren<Carta>();

        foreach (Carta carta in cartas)
        {
            puntosTotal += carta.puntosPoder;
        }
    }

    public void UnDraggeable()
    {
        DragHandler[] dragHandlers = GetComponentsInChildren<DragHandler>();

        foreach(DragHandler dragHandler in  dragHandlers)
        {
            dragHandler.enabled = false;
        }
    }

    //Aplicando un poco de recursividad xd
    public int CountAllChildren(Transform parent)
    {
        int count = parent.childCount;

        for(int i=0; i<parent.childCount; i++)
        {
            count += CountAllChildren(parent.GetChild(i));
        }

        return count;
    }
}