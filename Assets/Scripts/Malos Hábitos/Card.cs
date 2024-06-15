using UnityEngine;
using UnityEngine.UI;
using System;

public class Card : MonoBehaviour
{
    //Nombre de la carta
    public string cardName;
    
    //Poder de la carta (si es que tiene)
    public int puntosPoder;

    //Tipo (Oro, Plata, Clima...)
    public string typeCard;

    //Rango
    public string typeCard2;
    public string typeCard3;
    public string typeCard4;

    //Su facción
    public string faction;

    //Booleano de si tiene efecto o no
    public bool effect;

    //Aumenta el poder por 2 si se activa un aumento o en n si tiene cierto efecto
    public void IncreasePower(int n)
    {
        puntosPoder *= n;
    }
}
