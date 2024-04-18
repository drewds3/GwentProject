using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RowScoreController : MonoBehaviour
{
    //Variables para el sistema de puntuación
    public string typeRow;
    public int rowScore;
    private bool M = false;
    private bool R = false;
    private bool S = false;
    public GameObject climate;

    //Variable para el efecto de limpiar fila
    public int numberCards = 0;

    void Update()
    {
        //Se actualizan las zonas afectadas por el clima
        M = climate.GetComponent<ClimateSlots>().afectM;
        R = climate.GetComponent<ClimateSlots>().afectR;
        S = climate.GetComponent<ClimateSlots>().afectS;

        //Se calcula la puntuación total de la fila a tiempo real
        TotalScoreRow();
    }

    //Método para calcular la puntuación en la fila
    void TotalScoreRow()
    {   
        //El contador se inicia en 0
        rowScore = 0;

        Carta[] cartas = GetComponentsInChildren<Carta>();

        //Se cuentan las cartas que hay para el efecto de limpiar fila
        numberCards = cartas.Length;

        //Verifica si algún clima afecta a dicha fila
        if((typeRow == "M" && M) || (typeRow == "R" && R) || (typeRow == "S" && S))
        {
            foreach (Carta carta in cartas)
            {
                //Si la carta es de oro se mantiene su poder
                if(carta.typeCard4 == "Gold")
                {
                    rowScore += carta.puntosPoder;
                }
                else //De lo contrario se divide su poder a la mitad
                {
                    rowScore += carta.puntosPoder/2;
                }
            }
        }
        else //De lo contrario se procede con normalidad
        {
            foreach (Carta carta in cartas)
            {
                rowScore += carta.puntosPoder;
            }
        }
    }
}