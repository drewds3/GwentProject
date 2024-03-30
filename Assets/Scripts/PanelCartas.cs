using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PanelCartas : MonoBehaviour
{
    //Variables para el sistema de puntuación
     public int puntosTotal;
     
     //Variables para el sistema de turnos
     public int previousChildCount;
     public GameObject NextTurnPanel;
     public GameObject Block1;
     public GameObject Block2;
     public GameObject passButtonBlock;

     //Variable para el sistema de rondas
     private int count = 0;
     public GameObject passButton;

    void Start()
    {
        //Se inicializa la cantidad de objetos en 0
        previousChildCount = CountAllChildren(transform) - 6;
    }
    void Update()
    {
        //Se llaman a los métodos
        CalcularPuntosTotal();
        UnDraggeable();
        
        //Una variable que es igual a la cantidad de cartas en la fila en cuestión en todo momento
        int currentChildCount = CountAllChildren(transform) - 6;

        //Se actualiza el contador de pase
        count = passButton.GetComponent<PassButton>().passCount;

        //Se igualan las variables si son diferentes y se pasa de turno
        if(currentChildCount != previousChildCount && count%2==0)
        {
            previousChildCount = currentChildCount;
           
            Debug.Log($"En la zona hay actualmente {previousChildCount} cartas");

            NextTurnPanel.SetActive(!NextTurnPanel.activeSelf);
            Block1.SetActive(!Block1.activeSelf);
            Block2.SetActive(!Block2.activeSelf);
            passButtonBlock.SetActive(passButton.activeSelf);
        }
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

    //Contador de los hijos(cartas) del objeto(fila) aplicando un poco de recursividad
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