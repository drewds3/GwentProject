using System;

//Clase Variable
public class Variable : ICloneable
{
    public string Name;
    public object Value;
    public string Type;

    public Variable(string name, string type)
    {
        Name = name;
        Type = type;
    }

    public object Clone()
    {
        return new Variable(Name, Type){Value = Value};
    }
}