using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Clase principal del "Mini-Compilador"
public class Translator : MonoBehaviour
{
    public GameObject card;
    public List<GameObject> cards = new();

    public void Mostrar()
    {
        Lexer lexer = new(CodeEditor.inputField.text);

        var tokens = lexer.Tokenize();

        Parser parser = new(tokens);

        parser.Parse();

        GameObject cardInstance = Instantiate(card);

        Card cardScript = cardInstance.GetComponent<Card>();

        parser.SetProperties(cardScript);

        cards.Add(cardInstance);

        DrawCards player1Deck = GameObject.FindGameObjectWithTag("Player1Deck").GetComponent<DrawCards>();

        player1Deck.NewDeck();

        // foreach(Token token in tokens)
        // {
        //     Debug.Log($"{token._type}, {token._value}, {token._position}");
        // } 
    }
}