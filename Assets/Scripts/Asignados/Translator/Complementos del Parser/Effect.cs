using System.Collections.Generic;

//Efecto de las cartas
public class Effect
{
    //Nombre del efecto
    public string Name;

    //Instrucciones a ejecutar por el efecto
    private LinkedList<Instruction> Instructions;

    //Método para añadir el Action del efecto
    public Effect(string name, LinkedList<Instruction> instructions)
    {
        Name = name;
        Instructions = instructions;
    } 
}
