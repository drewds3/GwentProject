using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static SetPlayers;

//Clase principal del "Mini-Compilador"
public class Translator : MonoBehaviour
{
    //Tipos de prefabs
    public GameObject unitCard;
    public GameObject leaderCard;

    //Lista de cartas creadas
    public List<GameObject> cards = new();

    //Booleano que comprueba si ya se le asignó una facción nueva al jugador 1
    private bool isFactionNew = false;

    public void Mostrar()
    {
        //Primero que todo se obtienen los tokens
        Lexer lexer = new(CodeEditor.inputField.text);

        var tokens = lexer.Tokenize();

        //Luego estos se parsean
        Parser parser = new(tokens);

        parser.Parse();

        //Se comprueba de qué tipo es la carta y se instancia
        GameObject cardInstance;

        if(parser.WhichTypeCardIS() == "Leader")
        {
            cardInstance = LeaderInstance();
        }
        else
        {
            cardInstance = Instantiate(unitCard);
        } 

        //Se le añaden las características deseadas a la carta
        Card cardScript = cardInstance.GetComponent<Card>();

        parser.SetProperties(cardScript);

        //Se le establece la facción del jugador 1 como la misma que la de las cartas mientras que esta no sea la "neutral"
        if(!isFactionNew && cardScript.Faction != "Neutral")
        {
            player1.SetFaction(cardScript.Faction);
            isFactionNew = true;
        } 
        //Si ya se estableció la nueva facción se comprueba que las cartas sigan perteneciendo a la misma o sean neutrales
        else if(isFactionNew && cardScript.Faction != "Neutral" && cardScript.Faction != player1.Faction)
        {
            Destroy(cardInstance);
            throw new Exception("Cards must belong to the same faction or be neutral");
        }
        
        //Se agregan al deck si la carta no es líder
        if(cardScript.Type != "Leader")
        {
            cards.Add(cardInstance);

            DrawCards player1Deck = GameObject.FindGameObjectWithTag("Player1Deck").GetComponent<DrawCards>();

            player1Deck.NewDeck();
        }
    }

    //Método para instanciar y cambiar de líder
    private GameObject LeaderInstance()
    {
        //Se instacia
        GameObject cardInstance = Instantiate(leaderCard);

        //Se lleva a la posición en el tablero y se elimina al líder anterior
        cardInstance.transform.position = GameObject.Find("DovahkiinCardNordic").transform.position;
        cardInstance.transform.SetParent(GameObject.Find("Tablero").transform);
        cardInstance.transform.rotation = Quaternion.Euler(0, 0, +90);
        cardInstance.transform.SetSiblingIndex(1);
        Destroy(GameObject.Find("DovahkiinCardNordic"));

        //Se cambia el trigger del bloqueador de la carta líder
        EventTrigger eventTrigger = GameObject.Find("LeaderBlock1").GetComponent<EventTrigger>();

        EventTrigger.Entry pointerEnterEntry = null;
            
        foreach (var entry in eventTrigger.triggers)
        {
            if (entry.eventID == EventTriggerType.PointerEnter)
            {
                pointerEnterEntry = entry;
                break;
            }
        }

        void callback(BaseEventData eventData)
        {
            cardInstance.SendMessage("ViewCard");
        }

        // Se añade el nuevo trigger
        pointerEnterEntry.callback.AddListener(callback);
            
        return cardInstance;
    }
}