using System;
using System.Collections.Generic;

//Clase Instrucción
public class Instruction : ICloneable
{
    //Lista de palabras clave
    public List<string> KeyWords = new();

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

    public object Clone()
    {
        Instruction instruction = new();

        foreach(string word in KeyWords)
        {
            instruction.Add((string)word.Clone());
        }

        return instruction;
    }
}