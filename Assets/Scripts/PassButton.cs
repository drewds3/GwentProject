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

    //Variables para el contador de turnos del jugador 2
    public int player2Turn = 0;
    public GameObject turnCount2;
    private int comparador2;

    //Variables para simular el pase del jugador 2
    public GameObject blockHand2;
    public GameObject blockDeck2;
    public GameObject nextTurnCartel2;

    //Variable para bloquear el botón momentáneamente al tocarlo
    public GameObject passButtonBlock;

    //Variable para pasar de ronda
    public int passCount;
    public GameObject nextRoundCartel;
    public int round = 1;

    //Variable para mandar las cartas al cementerio al finalizar la ronda
    public Transform graveyard;

    void Start()
    {
        
    }

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

        if(turnCount2 != null && comparador2 != player2Turn)
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
                passButtonBlock.SetActive(!passButtonBlock.activeSelf);

                //Cuenta que usaron el pase
                passCount++;    
            }

            //En caso de que el turno sea del  jugador 2, este deja de jugar por la ronda.
            if(player1Turn != player2Turn)
            {
                blockHand2.SetActive(!blockHand2.activeSelf);
                blockDeck2.SetActive(!blockDeck2.activeSelf);
                nextTurnCartel2.SetActive(!nextTurnCartel2.activeSelf);
                passButtonBlock.SetActive(!passButtonBlock.activeSelf);

                //Cuenta que usaron el pase
                passCount++;    
            }
        }
        else //Si se vuelve a pulsar el botón pasa a la siguiente ronda
        {
            //Manda las cartas al cementerio
           GameObject[] cardsToGraveyard = GameObject.FindGameObjectsWithTag("CartaJugada");

           foreach(GameObject objeto in cardsToGraveyard)
           {
            objeto.transform.position = graveyard.position;
            objeto.transform.SetParent(graveyard);
           }

           //Algunos cambios en el escenario
            passButtonBlock.SetActive(!passButtonBlock.activeSelf);
            blockHand1.SetActive(true);
            blockDeck1.SetActive(true);
            blockHand2.SetActive(true);
            blockDeck2.SetActive(true);
            nextRoundCartel.SetActive(true);

            passCount++;
            round++;

            nextTurnCartel1.SetActive(!nextTurnCartel1.activeSelf);
            nextTurnCartel2.SetActive(!nextTurnCartel2.activeSelf);
        }
    }
}
