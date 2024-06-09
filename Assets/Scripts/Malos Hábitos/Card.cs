using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Card : MonoBehaviour
{
    //Nonbre de la carta
    public string cardName;
    
    //Sus puntos de poder
    public int puntosPoder;

    //Su tipo o tipos de carta
    public string typeCard;
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
