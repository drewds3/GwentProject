using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassButton : MonoBehaviour
{
    //Variables para el contador de turnos del jugador 1
    public int player1Turn = 0;
    public GameObject turnCount1;
    private int comparador1;

    //Variables para simular el pase del jugador 1
    public GameObject blockHand1;
    public GameObject blockDeck1;
    public GameObject nextTurnCartel1;
    public GameObject blockLeader1;

    //Variables para el contador de turnos del jugador 2
    public int player2Turn = 0;
    public GameObject turnCount2;
    private int comparador2;

    //Variables para simular el pase del jugador 2
    public GameObject blockHand2;
    public GameObject blockDeck2;
    public GameObject nextTurnCartel2;
    public GameObject blockLeader2;

    //Variable para bloquear el botón momentáneamente al tocarlo
    public GameObject passButtonBlock;

    //Variable para pasar de ronda
    public int passCount;
    public GameObject nextRoundCartel1;
    public GameObject nextRoundCartel2;
    public GameObject lastRoundCartel1;
    public GameObject lastRoundCartel2;
    public GameObject tiedGameCartel;
    public GameObject victoryCartel1;
    public GameObject victoryCartel2;
    public int round = 1;
    public int winner = 0;

    //Variable para mandar las cartas al cementerio al finalizar la ronda
    public Transform graveyard1;
    public Transform graveyard2;

    //Variables para saber quién gana cada ronda
    public GameObject counterScore;
    public int winP1 = 0;
    public int winP2 = 0;
    public bool victory = false;

    void Update()
    {   
        //Contador del jugador1
        if(turnCount1 != null)
        {
            comparador1 = turnCount1.GetComponent<TurnCount>().count;
        }

        if(turnCount1 != null && comparador1 != player1Turn)
        {
            player1Turn = comparador1;
        }

        //Contador del jugador2
        if(turnCount2 != null)
        {
            comparador2 = turnCount2.GetComponent<TurnCount>().count;
        }

        if(turnCount2 != null && comparador2 != player2Turn && winner == 2)
        {
            player2Turn = comparador2 - 1;
        }
        else
        {
            player2Turn = comparador2;
        }
    }

    public void Pass()
    {   
        //Verifica si nadie ha tocado el botón de pasar
        if(passCount%2==0)
        {
            //En caso de que el turno sea del  jugador 1, este deja de jugar por la ronda.
            if(player1Turn == player2Turn)
            {
                blockHand1.SetActive(!blockHand1.activeSelf);
                blockDeck1.SetActive(!blockDeck1.activeSelf);
                nextTurnCartel1.SetActive(!nextTurnCartel1.activeSelf);
                passButtonBlock.SetActive(true);
                blockLeader1.SetActive(true);

                //Cuenta que usaron el pase
                passCount++;    
            }

            //En caso de que el turno sea del  jugador 2, este deja de jugar por la ronda.
            if(player1Turn != player2Turn)
            {
                blockHand2.SetActive(!blockHand2.activeSelf);
                blockDeck2.SetActive(!blockDeck2.activeSelf);
                nextTurnCartel2.SetActive(!nextTurnCartel2.activeSelf);
                passButtonBlock.SetActive(true);
                blockLeader2.SetActive(true);

                //Cuenta que usaron el pase
                passCount++;    
            }
        }
        else //Si se vuelve a pulsar el botón verifica quién gano y pasa a la siguiente ronda
        {
            if(counterScore.GetComponent<ScoreText>().scorePlayer1 > counterScore.GetComponent<ScoreText>().scorePlayer2)
            {
                winP1++;
                winner = 1;
            }
            else if(counterScore.GetComponent<ScoreText>().scorePlayer1 < counterScore.GetComponent<ScoreText>().scorePlayer2)
            {
                winP2++;
                winner = 2;
            }
            else
            {
                //Si está activa la habilidad del lider del jugador 1 entonces gana en caso de empate
                bool effectLeaderNordic = GameObject.Find("DovahkiinCardNordic").GetComponent<LeaderEffect>().actived;

                if(effectLeaderNordic)
                {
                    winP1++;

                }
                else
                {
                    winP1++;
                    winP2++;
                }
               
                winner = 0;
            }

            //Manda las cartas a sus respectivos cementerios y se les cambia el tag para que no sean afectadas por efectos
            GameObject[] cardsToGraveyard1 = GameObject.FindGameObjectsWithTag("CartaJugada1");
            GameObject[] cardsToGraveyard2 = GameObject.FindGameObjectsWithTag("CartaJugada2");
            GameObject[] cardsToGraveyard3 = GameObject.FindGameObjectsWithTag("ClimaJugado2");

            foreach(GameObject objeto in cardsToGraveyard1)
            {
                objeto.tag = "CartaDescartada";
                objeto.transform.position = graveyard1.position;
                objeto.transform.SetParent(graveyard1);
                objeto.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            foreach(GameObject objeto in cardsToGraveyard2)
            {
                objeto.tag = "CartaDescartada";
                objeto.transform.position = graveyard2.position;
                objeto.transform.SetParent(graveyard2);
                objeto.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            foreach(GameObject objeto in cardsToGraveyard3)
            {
                objeto.tag = "CartaDescartada";
                objeto.transform.position = graveyard2.position;
                objeto.transform.SetParent(graveyard2);
                objeto.transform.rotation = Quaternion.Euler(0, 0, 0);
            }


            //Evalua la situación de la partida y procede en consecuencia
            if(winP1 == 1 && winP2 == 1)
            {
                //Prepara el escenario para la siguiente ronda
                passButtonBlock.SetActive(!passButtonBlock.activeSelf);
                blockHand1.SetActive(true);
                blockDeck1.SetActive(true);
                blockHand2.SetActive(true);
                blockDeck2.SetActive(true);
                blockLeader1.SetActive(true);
                blockLeader2.SetActive(true);

                //Evalúa si hubo empate o alguien ganó la última ronda y procede en consecuencia
                if(winner < 2)
                {
                    lastRoundCartel1.SetActive(true);
                    
                    passCount++;
                    round++;

                    nextTurnCartel1.SetActive(!nextTurnCartel1.activeSelf);
                    nextTurnCartel2.SetActive(!nextTurnCartel2.activeSelf);
                }
                else
                {
                    lastRoundCartel2.SetActive(true);
                    
                    passCount++;
                    round++;

                    nextTurnCartel1.SetActive(!nextTurnCartel1.activeSelf);
                    nextTurnCartel2.SetActive(!nextTurnCartel2.activeSelf);
                }
            }
            else if(winP1 == 1 && winP2 == 0)
            {
                //Prepara el escenario para la siguiente ronda
                passButtonBlock.SetActive(!passButtonBlock.activeSelf);
                blockHand1.SetActive(true);
                blockDeck1.SetActive(true);
                blockHand2.SetActive(true);
                blockDeck2.SetActive(true);
                nextRoundCartel1.SetActive(true);
                blockLeader1.SetActive(true);
                blockLeader2.SetActive(true);
                
                passCount++;
                round++;

                nextTurnCartel1.SetActive(!nextTurnCartel1.activeSelf);
                nextTurnCartel2.SetActive(!nextTurnCartel2.activeSelf);
            }
            else if(winP1 == 0 && winP2 == 1)
            {
                //Prepara el escenario para la siguiente ronda
                passButtonBlock.SetActive(!passButtonBlock.activeSelf);
                blockHand1.SetActive(true);
                blockDeck1.SetActive(true);
                blockHand2.SetActive(true);
                blockDeck2.SetActive(true);
                nextRoundCartel2.SetActive(true);
                blockLeader1.SetActive(true);
                blockLeader2.SetActive(true);
                
                passCount++;
                round++;

                nextTurnCartel1.SetActive(!nextTurnCartel1.activeSelf);
                nextTurnCartel2.SetActive(!nextTurnCartel2.activeSelf);
            }
            else if(winP1 == 2 && winP2 == 2)
            {
                //Queda en empate y termina la partida
                victory = true;

                passButtonBlock.SetActive(true);
                blockHand1.SetActive(true);
                blockDeck1.SetActive(true);
                blockHand2.SetActive(true);
                blockDeck2.SetActive(true);
                tiedGameCartel.SetActive(true);
                blockLeader1.SetActive(true);
                blockLeader2.SetActive(true);
            }
            else if(winP1 == 2)
            {
                //Gana el jugador 1 y termina la partida
                victory = true;

                passButtonBlock.SetActive(true);
                blockHand1.SetActive(true);
                blockDeck1.SetActive(true);
                blockHand2.SetActive(true);
                blockDeck2.SetActive(true);
                victoryCartel1.SetActive(true);
                blockLeader1.SetActive(true);
                blockLeader2.SetActive(true);
            }
            else
            {
                //Gana el jugador 2 y termina la partida
                victory = true;

                passButtonBlock.SetActive(true);
                blockHand1.SetActive(true);
                blockDeck1.SetActive(true);
                blockHand2.SetActive(true);
                blockDeck2.SetActive(true);
                victoryCartel2.SetActive(true);
                blockLeader1.SetActive(true);
                blockLeader2.SetActive(true);
            }

        }
    }
}
