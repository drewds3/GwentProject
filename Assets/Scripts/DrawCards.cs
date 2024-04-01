using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{   
    //Lista de cartas
    public List<GameObject> prefabs = new List<GameObject>();
    
    //Zona donde se "dibujan"
    public Transform cartasZone;

    //Zona donde se descartan, es decir, el cementerio
    public Transform graveyard;

    //Carta a dibujar
    int indice;

    //Ronda Actual
    int round;

    //Turno actual
    public int turnP1;
    public int turnP2;

    //Sistema de bloqueadores
    public GameObject blockRows1;
    public GameObject blockRows2;
    public GameObject blockPassButton;
    public GameObject condicion1;
    public GameObject condicion2;
    int count = 0;
    private bool condicion3 = false;
    private int roundWinner = 0;

    //Variables para el sistema de robo de una carta cada turno fuera del inicial de la ronda
    int countRound = 1;
    int countTurn1 = 0;
    int countTurn2 = 0;
    public GameObject blockdeck1;
    public GameObject blockdeck2;

    void Update()
    {   
        //Se actualiza la ronda, los turnos, si ya alguien ganó y si el jugador 2 ganó alguna ronda
        round = GameObject.Find("PassButton").GetComponent<PassButton>().round;
        turnP1 = GameObject.Find("PassButton").GetComponent<PassButton>().player1Turn;
        turnP2 = GameObject.Find("PassButton").GetComponent<PassButton>().player2Turn;
        roundWinner = GameObject.Find("PassButton").GetComponent<PassButton>().winner;
        condicion3 = GameObject.Find("PassButton").GetComponent<PassButton>().victory;
        
        //Verifica quién fue el último que ganó
        if(roundWinner < 2)
        {
            //Se bloquean la fila del jugador 2 al inicio de la ronda ganada por el jugador 1 y la inicial
            if(turnP1 == 1 && turnP2 == 0 && condicion1.activeSelf)
            {
                blockRows2.SetActive(true);
            }
        }
        else
        {
            //Se bloquean la fila del jugador 1 al inicio de la ronda ganada por el jugador 2
            if(turnP1 == 0 && turnP2 == 0 && condicion2.activeSelf)
            {
                blockRows1.SetActive(true);
            }
        }

        //Desbloquea el botón si ya ha pasado 1 turno en ambos jugadores y alguno está jugando
        if(turnP1 != 0 && turnP2 != 0 && !condicion2.activeSelf && !condicion1.activeSelf && !condicion3 && roundWinner != 2)
        {
            blockPassButton.SetActive(false);
        }
        else if(turnP1 != 0 && turnP2 != -1 && !condicion2.activeSelf && !condicion1.activeSelf && !condicion3 && roundWinner == 2)
        {
            blockPassButton.SetActive(false);
        }

        //Actualiza variables para controlar que se robe una carta cada turno fuera del inicial
        if(countRound < round)
        {   
            countTurn1 = 0;

            if(roundWinner < 2)
            {
                countTurn2 = 0;
            }
            else
            {
                countTurn2 = -1;
            }

            countRound++;
        }

        //Limita el robo de cartas cada turno a 1 después del primero
        if(countTurn1 == turnP1 - 2)
        {   
            countTurn1++;
        }

        if(countTurn2 == turnP2 - 2)
        {
            countTurn2++;
        }
    }

    //Método que "dibuja" las cartas
    public void OnClick()
    {
        //Si hay cartas dibuja la que está en el indice aleatorio
        if(prefabs.Count==0)
        {
            Debug.LogError("Se acabaron las cartas chamacón");
        }
        else
        {   
            //Comprueba en cuál ronda y turno están
            if(round == 1 && (turnP1 == 0 || turnP2 == 0))
            {
                //Si es el primer turno de la 1ra ronda roban 10 cartas
                if(cartasZone.childCount<10)
                {
                    indice = Random.Range(0, prefabs.Count);

                    Instantiate(prefabs[indice], cartasZone);

                    prefabs.Remove(prefabs[indice]);
                }
                else
                {
                    Debug.Log("No puedes robar más cartas en este turno");
                }

                //Si ya tiene las 10 se le permite jugar
                if(cartasZone.childCount == 10)
                {
                    blockRows1.SetActive(false);
                    blockRows2.SetActive(false);
                    blockPassButton.SetActive(false);

                    Debug.Log("Ya puedes jugar");
                }
            }
            //Si estamos en la 2da o 3ra ronda y es el primer turno roban 2 cartas
            else if(round > 1 && (turnP1 == 0 || (turnP2 == 0 && roundWinner == 1) || (turnP2 == -1 && roundWinner == 2)))
            {
                //Si tiene menos de 10 cartas, roba
                if(cartasZone.childCount<10 && ((count<2 && round == 2) || (count<4 && round == 3)))
                {
                    indice = Random.Range(0, prefabs.Count);

                    Instantiate(prefabs[indice], cartasZone);

                    prefabs.Remove(prefabs[indice]);

                    count++;

                    Debug.Log(count);
                }
                //De no ser así las cartas robadas serán descartadas al cementerio
                else if((count != 2 && round == 2) || (count !=4 && round == 3))
                {
                    indice = Random.Range(0, prefabs.Count);

                    Instantiate(prefabs[indice], graveyard);

                    prefabs.Remove(prefabs[indice]);

                    count++;

                    Debug.Log(count);
                }
                
                //Si ya se robaron 2 cartas se permite jugar
                if((count==2 && round==2) || (count==4 && round==3))
                {
                    blockRows1.SetActive(false);
                    blockRows2.SetActive(false);
                    blockPassButton.SetActive(false);

                    Debug.Log("Ya puedes jugar");
                }

            }
            //Si ya pasó el primer turno de ambos jugadores entonces pueden robar únicamente una carta cada turno
            else if(cartasZone.childCount < 10 && (countTurn1 < turnP1 && blockdeck2.activeSelf) 
                                               || (countTurn2 < turnP2 && blockdeck1.activeSelf))
            {
                indice = Random.Range(0, prefabs.Count);

                Instantiate(prefabs[indice], cartasZone);

                prefabs.Remove(prefabs[indice]);

                if(blockdeck2.activeSelf)
                {
                    countTurn1++;
                }
                else
                {
                    countTurn2++;
                }
            }
            //Si ya se alcanzó el limite de cartas en mano será descartada al cementerio
            else if((countTurn1 < turnP1 && blockdeck2.activeSelf) || (countTurn2 < turnP2 && blockdeck1.activeSelf))
            {
                indice = Random.Range(0, prefabs.Count);

                Instantiate(prefabs[indice], graveyard);

                prefabs.Remove(prefabs[indice]);

                if(blockdeck2.activeSelf)
                {
                    countTurn1++;
                }
                else
                {
                    countTurn2++;
                }
            }
        }
    }
}