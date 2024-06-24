using System.Collections.Generic;
using UnityEngine;

//Clase principal del "Mini-Compilador"
public class Translator : MonoBehaviour
{
    //Lista de cartas creadas
    public List<GameObject> cards = new();

    public void Mostrar()
    {
        //Primero que todo se obtienen los tokens
        Lexer lexer = new(CodeEditor.inputField.text);

        var tokens = lexer.Tokenize();

        //Luego estos se parsean
        Parser parser = new(tokens);

        //Se crean las cartas
        parser.Parse();

        //Y se añaden al deck
        DrawCards player1Deck = GameObject.FindGameObjectWithTag("Player1Deck").GetComponent<DrawCards>();

        player1Deck.NewDeck();
    }
}