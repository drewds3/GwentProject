using System.Collections.Generic;

//Clase Instrucción
public class Instruction
{
    //Tipo de la acción (si es una declaración de variable o una llamada a un método)
    public bool IsDeclaration = false;

    //Lista de palabras clave
    public List<string> KeyWords = new();

    //Variables a pasar como parámetro (si es que tienen)
    public Variable AssociatedVariable1;
    public Variable AssociatedVariable2;
}