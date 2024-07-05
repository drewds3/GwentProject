using System.Collections.Generic;

//Clase Instrucción
public class Instruction
{
    //Lista de palabras clave
    private List<string> KeyWords = new();

    //Variables a pasar como parámetro (si es que tienen)
    public Instruction AssociatedVariable1;
    public Instruction AssociatedVariable2;

    //Método para añadir palabras clave a la instrucción
    public void Add(string KeyWord)
    {
        KeyWords.Add(KeyWord);
    }

    //Cantidad de palabras clave recogidas
    public int Count{get{return KeyWords.Count;}}

    //Método para debuguear en la consola de Unity
    public void Debug()
    {
        string debugList = string.Join(", ", KeyWords.ToArray());
        UnityEngine.Debug.Log("Esta instrucción tiene las siguientes palabras clave: " + debugList);
    }
}