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

    void Update()
    {   
        //Se actualiza que ronda es
        round1 = GameObject.Find("PassButton").GetComponent<PassButton>().round;

        if(round1 != round2)
        {
            actived = false;
        }
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
            }
            //Si no hay cartas de unidad no se activa
            catch(IndexOutOfRangeException ex)
            {
                Debug.Log("No hay cartas de unidades en el campo");
            }
        }
        else
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
        }
        else
        {
            Debug.Log("Ya se usó una vez");
        }
    }
}
