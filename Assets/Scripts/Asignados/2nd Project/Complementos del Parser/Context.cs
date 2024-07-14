using System.Collections.Generic;
using UnityEngine;
using static SetPlayers;
using System.Linq;

public class Context : MonoBehaviour
{
    //Devuelve el jugador que desencadenó el efecto, es decir, al que le toca el turno actual
    public static Player TriggerPlayer
    {get
        {
            if(GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>().currentTurn%2==0)
            {
                return player1;
            }
            else return player2;
        }
    }

    public static Player OtherPlayer
    {get
        {
            if(GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>().currentTurn%2!=0)
            {
                return player1;
            }
            else return player2;
        }
    }

    //Devuelve una lista con todas las cartas del tablero
    public static List<GameObject> Board
    {
        get
        {   
            //Se crea la lista y se le unen las cartas en los mazos
            List<GameObject> board = GameObject.FindObjectsOfType<Card>().Select(card => card.gameObject).ToList();
            board.AddRange(GameObject.Find("Deck").GetComponent<DrawCards>().deck);
            board.AddRange(GameObject.Find("DeckEnemy").GetComponent<DrawCards>().deck);

            return board;
        }
    }

    //Devuelve una lista de las cartas en la mano del jugdor en cuestión
    public static List<GameObject> HandOfPlayer(Player player)
    {
        if(player == player1) return player1.CardsInHand;
        else return player2.CardsInHand;
    }

    //Devuelve una lista de las cartas en el campo del jugador en cuestión
    public static List<GameObject> FieldOfPlayer(Player player)
    {
        if(player == player1)
        {
            List<GameObject> cards = GameObject.FindGameObjectsWithTag("CartaJugada1").ToList();
            cards.AddRange(GameObject.FindGameObjectsWithTag("ClimaJugado1").ToList());
            return cards;
        }
        else
        {
            List<GameObject> cards = GameObject.FindGameObjectsWithTag("CartaJugada2").ToList();
            cards.AddRange(GameObject.FindGameObjectsWithTag("ClimaJugado2").ToList());
            return cards;
        }
    }

    //Devuelve una lista de las cartas en el cementerio del jugador en cuestión
    public static List<GameObject> GraveyardOfPlayer(Player player)
    {
        if(player == player1) return GameObject.FindGameObjectsWithTag("CartaDescartada1").ToList();
        else return GameObject.FindGameObjectsWithTag("CartaDescartada2").ToList();
    }

    //Devuelve una lista de las cartas en el mazo del jugador en cuestión
    public static List<GameObject> DeckOfPlayer(Player player)
    {
        if(player == player1) return player1.Deck.GetComponent<DrawCards>().deck;
        else return player2.Deck.GetComponent<DrawCards>().deck;
    }
}
