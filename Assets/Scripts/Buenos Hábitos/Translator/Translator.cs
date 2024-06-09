using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Clase principal del "Mini-Compilador"
public class Translator : MonoBehaviour
{
    public GameObject card;
    private readonly List<Card> cards;
    private int currentCardIndex = 0;

    public void Mostrar()
    {
        Lexer lexer = new(CodeEditor.inputField.text);

        var tokens = lexer.Tokenize();

        Parser parser = new(tokens);

        parser.Parse();

        Card cardScript = card.GetComponent<Card>();

        cards.Add(cardScript);

        // foreach(Token token in tokens)
        // {
        //     Debug.Log($"{token._type}, {token._value}, {token._position}");
        // } 
    }
}