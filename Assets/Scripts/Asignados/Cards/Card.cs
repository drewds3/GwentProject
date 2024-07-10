using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //Nombre de la carta
    public string Name;
    
    //Tipo (Oro, Plata, Clima...)
    public string Type;

    //Su facción
    public string Faction;

    //Poder de la carta (si es que tiene)
    public int Power;

    //Rango
    public string Range1;
    public string Range2;
    public string Range3;

    //Aumenta el poder por 2 si se activa un aumento o en n si tiene cierto efecto
    public void IncreasePower(int n)
    {
        Power *= n;
    }

    //Número del efecto a activar
    public int NumEffect;
}