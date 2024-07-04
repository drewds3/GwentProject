using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase propiedad
public class Property
{
    public readonly string Type;
    public readonly string ValueS;
    public readonly int ValueI;
    
    //Dos constructores, la segunda es para el caso particular del "Power" y la primera para el resto
    public Property(string type, string value)
    {
        Type = type;
        ValueS = value;
    }

    public Property(string type, int value)
    {
        Type = type;
        ValueI = value;
        ValueS = $"{value}";
    }
}
