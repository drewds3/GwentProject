using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RowScoreController : MonoBehaviour
{
    //Variables para el sistema de puntuación
    public int rowScore;

    void Start()
    {

    }
    void Update()
    {
        //Se calcula la puntuación total de la fila a tiempo real
        TotalScoreRow();
    }

    //Método para calcular la puntuación en la fila
    void TotalScoreRow()
    {
        rowScore = 0;
        Carta[] cartas = GetComponentsInChildren<Carta>();

        foreach (Carta carta in cartas)
        {
            rowScore += carta.puntosPoder;
        }
    }
}