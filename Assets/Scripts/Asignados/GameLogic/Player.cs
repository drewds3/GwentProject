using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Clase Jugador
public class Player
{
    //Propiedades de cada jugador
    public GameObject Hand {get;set;}
    public GameObject Deck {get;set;}
    public Transform Graveyard {get;set;}
    public string Faction {get;set;}

    //Lista de las cartas en la mano del jugador
    public List<GameObject> CardsInHand
    {
        get
        {
            List<Card> cardsTransforms = Hand.GetComponentsInChildren<Card>().ToList();
            List<GameObject> cards = new();

            for(int i = 0; i < cardsTransforms.Count; i++)
            {
                cards.Add(cardsTransforms[i].gameObject);
            }

            return cards;
        }
    }

    //Variables para simular el pase del jugador
    public GameObject blockHand;
    public GameObject blockDeck;
    public GameObject nextTurnCartel;
    public GameObject blockLeader;

    //Constructor de la clase
    public Player(GameObject hand, GameObject deck, Transform graveyard, string faction, GameObject blockHand, 
                  GameObject blockDeck, GameObject nextTurnCartel, GameObject blockLeader)
    {
        Hand = hand;
        Deck = deck;
        Graveyard = graveyard;
        Faction = faction;
        this.blockHand = blockHand;
        this.blockDeck = blockDeck;
        this.nextTurnCartel = nextTurnCartel;
        this.blockLeader = blockLeader;
    }

    //Método para evitar que un jugador juegue en el turno del adversario al finalizar su turno
    public void EndTurn()
    {
        blockHand.SetActive(!blockHand.activeSelf);
        blockDeck.SetActive(!blockDeck.activeSelf);
        nextTurnCartel.SetActive(!nextTurnCartel.activeSelf);
        blockLeader.SetActive(true);
    }

    //Método para cambiar la facción
    public void SetFaction(string faction)
    {
        Faction = faction;
    }
}