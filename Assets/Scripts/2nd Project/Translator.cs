using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using TMPro;
using UnityEngine;

//Clase principal del "Mini-Compilador"
public class Translator : MonoBehaviour
{
    //Lista de cartas creadas
    public List<GameObject> cards = new();

    //Método principal del segundo proyecto
    public void Translate()
    {
        try
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

        //Finalmente se le indica al usuario que se completó con éxito la creación de las cartas y el total de las mismas
        TMP_Text text = GameObject.Find("Excepción").GetComponent<TMP_Text>();

        //De completarse correctamente la creación se le informa al usuario  junto con el total de cartas en el mazo
        int total = GameObject.Find("NewDeck").transform.childCount;

        text.text = "Cards were successfully created!" + "\n\n Total: " + total;

        }
        catch (Exception e) // En caso de cometerse algún error al programar el mazo se muestra cuál fue
        {
            TMP_Text text = GameObject.Find("Excepción").GetComponent<TMP_Text>();
            text.text = "Could not created card because: " + e.Message;
        }
    }
}