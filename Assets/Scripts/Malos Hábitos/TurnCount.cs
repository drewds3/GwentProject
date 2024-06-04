using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnCount : MonoBehaviour
{
    //Variables para controlar los turnos y rondas
    public int count = 0;
    public int roundCount1 = 1;
    public int roundCount2 = 1;
    public GameObject roundCounter;
    
    void OnEnable()
    {
        //Aumenta el contador si se activa el objeto
        count++;

        //Se reinicia si se pasa de ronda
        roundCount1 = roundCounter.GetComponent<PassButton>().round;

        if(roundCount1 != roundCount2)
        {
            count = 0;
            roundCount2 = roundCount1;
        }
    }
}
