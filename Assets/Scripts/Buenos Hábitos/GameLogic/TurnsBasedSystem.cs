using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnsBasedSystem : MonoBehaviour
{
    //Turno actual, par le toca al jugador 1, y impar al jugador 2
    public int currentTurn = 0;

    //Veces que han pasado los jugadores
    public int passCount = 0;

    //Jugadores
    public Player player1;
    public Player player2;

    //Propiedades de los jugadores
    public GameObject hand1;
    public GameObject dekc1;
    public Transform graveyard1;
    public GameObject handBlock1;
    public GameObject deckBlock1;
    public GameObject nextTurnCartel1;
    public GameObject blockLeader1;

    public GameObject hand2;
    public GameObject dekc2;
    public Transform graveyard2;
    public GameObject handBlock2;
    public GameObject deckBlock2;
    public GameObject nextTurnCartel2;
    public GameObject blockLeader2;

    public GameObject passButtonBlock;

    //Variables para cambiar de ronda
    public GameObject nextRoundCartel1;
    public GameObject nextRoundCartel2;
    public GameObject lastRoundCartel1;
    public GameObject lastRoundCartel2;
    public GameObject tiedGameCartel;
    public GameObject victoryCartel1;
    public GameObject victoryCartel2;
    public int round = 1;
    public int winner = 0;
    public bool victory;
 
    void Awake()
    {
        //Se le otorgan las propiedades a los jugadores
        Player playerX = new(hand1, dekc1, graveyard1, "Dragon", handBlock1, deckBlock1, nextTurnCartel1, blockLeader1, passButtonBlock);
        player1 = playerX;

        Player playerY = new(hand2, dekc2, graveyard2, "Raven", handBlock2, deckBlock2, nextTurnCartel2, blockLeader2, passButtonBlock);
        player2 = playerY;
    }
    
    //Método para cambiar de turno
    public void NextTurn()
    {   
        //Verifica de quién es el actual turno y pasa la siguiente
        if (currentTurn % 2 == 0)
        {
            player1.EndTurn();
        }
        else
        {
            player2.EndTurn();
        }
        currentTurn++;
    }

    public void EndRound()
    {
        //Prepara el escenario para la siguiente ronda
        passButtonBlock.SetActive(true);

        handBlock1.SetActive(true);
        deckBlock1.SetActive(true);
        blockLeader1.SetActive(true);

        handBlock2.SetActive(true);
        deckBlock2.SetActive(true);
        blockLeader2.SetActive(true);
    }

    //Método para controlar el pase
    public void Pass()
    {
        //Si nadie ha pasado, el jugador actual deja de jugar por la ronda
        if(passCount%2==0)
        {
            NextTurn();

            passCount++;
        }
        else //Si se vuelve a pulsar el botón verifica quién gano y pasa a la siguiente ronda
        {
            ScoreText counterScore = GameObject.Find("ScoreText").GetComponent<ScoreText>();

            if(counterScore.GetComponent<ScoreText>().scorePlayer1 > counterScore.GetComponent<ScoreText>().scorePlayer2)
            {
                counterScore.winsP1++;
                winner = 1;
            }
            else if(counterScore.GetComponent<ScoreText>().scorePlayer1 < counterScore.GetComponent<ScoreText>().scorePlayer2)
            {
                counterScore.winsP2++;
                winner = 2;
            }
            else
            {
                //Si está activa la habilidad del lider del jugador 1 entonces gana en caso de empate
                if(GameObject.Find("DovahkiinCardNordic").GetComponent<LeaderEffect>().actived)
                {
                    counterScore.winsP1++;

                }
                else
                {
                    counterScore.winsP1++;
                    counterScore.winsP2++;
                }
               
                winner = 0;
            }

            //Manda las cartas a sus respectivos cementerios y se les cambia el tag para que no sean afectadas por efectos
            GameObject[] cardsToGraveyard1 = GameObject.FindGameObjectsWithTag("CartaJugada1");
            GameObject[] cardsToGraveyard2 = GameObject.FindGameObjectsWithTag("CartaJugada2");
            GameObject[] cardsToGraveyard3 = GameObject.FindGameObjectsWithTag("ClimaJugado1");
            GameObject[] cardsToGraveyard4 = GameObject.FindGameObjectsWithTag("ClimaJugado2");

            foreach(GameObject objeto in cardsToGraveyard1)
            {
                objeto.tag = "CartaDescartada";
                objeto.transform.position = player1.Graveyard.position;
                objeto.transform.SetParent(player1.Graveyard);
                objeto.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            foreach(GameObject objeto in cardsToGraveyard2)
            {
                objeto.tag = "CartaDescartada";
                objeto.transform.position = player2.Graveyard.position;
                objeto.transform.SetParent(player2.Graveyard);
                objeto.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            foreach(GameObject objeto in cardsToGraveyard3)
            {
                objeto.tag = "CartaDescartada";
                objeto.transform.position = player1.Graveyard.position;
                objeto.transform.SetParent(player1.Graveyard);
                objeto.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            foreach(GameObject objeto in cardsToGraveyard4)
            {
                objeto.tag = "CartaDescartada";
                objeto.transform.position = player2.Graveyard.position;
                objeto.transform.SetParent(player2.Graveyard);
                objeto.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            //Evalua la situación de la partida y procede en consecuencia
            if(counterScore.winsP1 == 1 && counterScore.winsP2 == 1)
            {
                EndRound();

                //Evalúa si hubo empate o alguien ganó la última ronda y procede en consecuencia
                if(winner < 2)
                {
                    lastRoundCartel1.SetActive(true);
                    
                    passCount++;
                    round++;

                    currentTurn = 0;
                }
                else
                {
                    lastRoundCartel2.SetActive(true);
                    
                    passCount++;
                    round++;

                    currentTurn = -1;
                }
            }
            else if(counterScore.winsP1 == 1 && counterScore.winsP2 == 0)
            {
                EndRound();
                nextRoundCartel1.SetActive(true);
                
                passCount++;
                round++;

                currentTurn = 0;
            }
            else if(counterScore.winsP1 == 0 && counterScore.winsP2 == 1)
            {
                EndRound();
                nextRoundCartel2.SetActive(true);
                
                passCount++;
                round++;

                currentTurn = -1;
            }
            else if(counterScore.winsP1 == 2 && counterScore.winsP2 == 2)
            {
                //Queda en empate y termina la partida
                victory = true;

                EndRound();
                tiedGameCartel.SetActive(true);
            }
            else if(counterScore.winsP1 == 2)
            {
                //Gana el jugador 1 y termina la partida
                victory = true;

                EndRound();
                victoryCartel1.SetActive(true);
            }
            else
            {
                //Gana el jugador 2 y termina la partida
                victory = true;

                EndRound();
                victoryCartel2.SetActive(true);
            }
        }
    }   
}