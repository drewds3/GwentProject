using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewCard : Card
{
    //Efectos de la carta
    public List<Effect> Effects = new();

    public int count;

    private bool leaderEffectActivated = false; // Para que el efecto del líder solo se use una vez

    void Update()
    {
        count = Effects.Count;
    }

    //Método para activar el efecto de la carta
    public void Activate()
    {
        try
        {
            if(Effects.Count > 0 && !leaderEffectActivated)
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

                    leaderEffectActivated = true;
                }
                
                Debug.Log("Efecto de líder activado");

                TMP_Text text = GameObject.Find("Contexto").GetComponent<TMP_Text>();
                text.text = "Active leader effect";
            }
            else if(leaderEffectActivated)
            {
                Debug.Log("Ya se usó una vez");

                TMP_Text text = GameObject.Find("Contexto").GetComponent<TMP_Text>();
                text.text = "Already it used once";
            } 
            else Debug.Log("No tiene efecto");
        }
        /*Al declararse a través de un DSL y activarse en tiempo de juego pueden
        producirse diversos errores comunes al ejecutarse dicho efecto
        como indexar fuera de rango en una lista o un bucle infinito*/
        catch 
        {
            Debug.LogError("Se produjo un error al intentar activar el efecto");
        }
    }
}
