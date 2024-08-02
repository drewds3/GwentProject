using System.Collections.Generic;
using UnityEngine;

public class NewCard : Card
{
    //Efectos de la carta
    public List<Effect> Effects = new();

    public int count;

    void Update()
    {
        count = Effects.Count;
    }

    //Método para activar el efecto de la carta
    public void Activate()
    {
        if(Effects.Count > 0)
        {
            //Se activan los efectos que tenga
            for(int i = 0; i < Effects.Count; i++) 
            {
                Effects[i].Activate();
            }

            //Si la carta es líder se pasa de turno
            if(Type == "Leader")
            {
                TurnsBasedSystem turnsController = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>();

                //Pero solo si el otro jugador no ha pasado
                if(turnsController.passCount % 2 == 0)
                {
                    turnsController.NextTurn();
                }
            }
            
            Debug.Log("Efecto de líder activado");
        }
        else Debug.Log("No tiene efecto");
    }
}
