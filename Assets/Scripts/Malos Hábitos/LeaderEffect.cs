using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeaderEffect : MonoBehaviour
{   
    //Variables para evitar que se usen efectos más de una vez
    int count1 = 0;
    int count2 = 0;
    int round1;
    int round2;

    //Variable para el efecto de la carta lider del jugador 1
    public bool actived = false;

     //Variables para el sistema de turnos
    public GameObject nextTurnPanel;
    public GameObject blockHand;
    public GameObject blockDeck;
    public GameObject passButtonBlock;
    public GameObject leaderBlock;

    //Variables para el sistema de rondas
    private int count = 0;

    void Update()
    {   
        //Se actualiza que ronda es
        round1 = GameObject.Find("PassButton").GetComponent<PassButton>().round;

        if(round1 != round2)
        {
            actived = false;
        }

        //Se actualiza el contador de pase
        count = GameObject.Find("PassButton").GetComponent<PassButton>().passCount;
    }

    //Mantiene un carta aleatoria en el campo entre cada ronda tras activar su efecto
    public void AnastarianKingEffect()
    {
        if(count2 < 1)
        {
            try
            {
                GameObject[] playedCards = GameObject.FindGameObjectsWithTag("CartaJugada2");

                int indice = UnityEngine.Random.Range(0, playedCards.Length);

                playedCards[indice].tag = "CartaSalvada";

                count2++;

                Debug.Log("Efecto de líder elfo activado");

                //Además pasa de turno si el otro jugador no ha pasado
                if(count%2==0)
                {
                    nextTurnPanel.SetActive(!nextTurnPanel.activeSelf);
                    blockHand.SetActive(!blockHand.activeSelf);
                    blockDeck.SetActive(!blockDeck.activeSelf);
                    passButtonBlock.SetActive(!passButtonBlock.activeSelf);
                    leaderBlock.SetActive(true);
                }
            }
            //Si no hay cartas de unidad no se activa
            catch (IndexOutOfRangeException)
            {
                Debug.Log("No hay cartas de unidades en el campo");
            }
        }
        else //Solo se puede activar una vez
        {
            Debug.Log("Ya se usó una vez");
        }
        
    }

    //En caso de empate, gana
    public void DovahkiinEffect()
    {
        if(count1 < 1)
        {
            round2 = round1;

            actived = true;

            count1++;
            
            Debug.Log("Efecto de líder nórdico activado");

            //Además pasa de turno si el otro jugador no ha pasado
            if(count%2==0)
            {
                nextTurnPanel.SetActive(!nextTurnPanel.activeSelf);
                blockHand.SetActive(!blockHand.activeSelf);
                blockDeck.SetActive(!blockDeck.activeSelf);
                passButtonBlock.SetActive(!passButtonBlock.activeSelf);
                leaderBlock.SetActive(true);
            }
        }
        else //Solo se puede activar una vez
        {
            Debug.Log("Ya se usó una vez");
        }
    }
}
