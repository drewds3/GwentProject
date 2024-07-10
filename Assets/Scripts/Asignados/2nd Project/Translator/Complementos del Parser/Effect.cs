using System.Collections.Generic;

//Efecto de las cartas
public class Effect
{
    //Nombre del efecto
    public string Name {get;}

    //Parámetros de la carta (si es que tiene)
    public LinkedList<Variable> Params {get;}

    //"Targets"
    public string Targets {get;set;}

    //Single
    public bool Single {get;set;}

    //Instrucciones a ejecutar por el efecto
    public LinkedList<Instruction> Instructions;

    //Efecto anidado (si es que tiene)
    public Effect PostEffect {get;set;}

    //Constructores
    public Effect(string name, LinkedList<Instruction> instructions)
    {
        Name = name;
        Instructions = instructions;
    } 

    public Effect(string name, LinkedList<Instruction> instructions, LinkedList<Variable> Params)
    {
        Name = name;
        Instructions = instructions;
        this.Params = Params;
    }

    //Método para clonar el efecto y trabajar con él sin que sea de referencia
    public Effect Clone()
    {
        return new Effect(Name, Instructions, Params);
    }
}
