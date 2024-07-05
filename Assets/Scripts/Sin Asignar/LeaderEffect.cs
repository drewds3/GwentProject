using UnityEngine;
using System;

public class LeaderEffect : MonoBehaviour
{   
    //Variables para evitar que se usen efectos más de una vez
    int count = 0;
    int round1;
    int round2;

    //Variable para el efecto de la carta lider del jugador 1
    public bool actived = false;

    void Update()
    {   
        //Se actualiza que ronda es
        round1 = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>().round;

        if(round1 != round2)
        {
            actived = false;
        }
    }

    //Mantiene un carta aleatoria en el campo entre cada ronda tras activar su efecto
    public void AnastarianKingEffect()
    {
        if(count < 1)
        {
            try
            {
                GameObject[] playedCards = GameObject.FindGameObjectsWithTag("CartaJugada2");

                int indice = UnityEngine.Random.Range(0, playedCards.Length);

                playedCards[indice].tag = "CartaSalvada";

                count++;

                Debug.Log("Efecto de líder elfo activado");

                TurnsBasedSystem turnsController = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>();

                //Además pasa de turno si el otro jugador no ha pasado
                if(turnsController.passCount % 2 == 0)
                {
                    turnsController.NextTurn();
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
        if(count < 1)
        {
            round2 = round1;

            actived = true;

            count++;
            
            Debug.Log("Efecto de líder nórdico activado");

            TurnsBasedSystem turnsController = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>();

            //Además pasa de turno si el otro jugador no ha pasado
            if(turnsController.passCount % 2 == 0)
            {
                turnsController.NextTurn();
            }
        }
        else //Solo se puede activar una vez
        {
            Debug.Log("Ya se usó una vez");
        }
    }
}
