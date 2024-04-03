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

    void Start()
    {
        
    }
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
        rowScore = 0;
        Carta[] cartas = GetComponentsInChildren<Carta>();

        if((typeRow == "M" && M) || (typeRow == "R" && R) || (typeRow == "S" && S))
        {
            foreach (Carta carta in cartas)
        {
            rowScore += carta.puntosPoder/2;
        }
        }
        else
        {
            foreach (Carta carta in cartas)
        {
            rowScore += carta.puntosPoder;
        }
        }
        
        
        // //Si la fila es afectada por el clima se le resta la mitad de su puntuación
        // if(typeRow == "M" && M) rowScore/=2;
        // else if(typeRow == "R" && R) rowScore/=2;
        // else if(typeRow == "S" && S) rowScore/=2;
    }
}