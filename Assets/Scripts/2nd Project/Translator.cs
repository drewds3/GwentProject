using System.Collections.Generic;
using UnityEngine;

//Clase principal del "Mini-Compilador"
public class Translator : MonoBehaviour
{
    //Lista de cartas creadas
    public List<GameObject> cards = new();

    //Método principal del segundo proyecto
    public void Translate()
    {
        //Primero que todo se obtienen los tokens
        Lexer lexer = new(CodeEditor.inputField.text);
        var tokens = lexer.Tokenize();

        //Luego estos se parsean creándose así las cartas
        Parser parser = new(tokens);
        parser.Parse();

        //Y se añaden al deck si fueron bien declaradas
        Deck player1Deck = GameObject.FindGameObjectWithTag("Player1Deck").GetComponent<Deck>();
        player1Deck.NewDeck();
    }
}