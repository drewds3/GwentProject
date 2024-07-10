using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase Variable
public class Variable
{
    public string Name;
    public object Value;
    public string Type;

    public Variable(string name, string type)
    {
        Name = name;
        Type = type;
    }
}