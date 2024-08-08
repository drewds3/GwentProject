using UnityEngine;
using static TurnsBasedSystem;
using static SetPlayers;

//Clase para controlar algunos aspectos del tablero
public class BoardControl : MonoBehaviour
{
    //Variables para controlar el tablero
    public GameObject condicion1;
    public GameObject condicion2;
    public GameObject rowsBlock1;
    public GameObject rowsBlock2;
    public GameObject passButtonBlock;
    public GameObject leaderBlock1;
    public GameObject leaderBlock2;

    void Update()
    {
        //Verifica quién fue el último que ganó
        if(winner < 2)
        {
            //Se bloquean la fila del jugador 2 al inicio de la ronda ganada por el jugador 1 y la inicial
            if(currentTurn == 1 && condicion1.activeSelf)
            {
                rowsBlock2.SetActive(true);
            }
        }
        else
        {
            //Se bloquean la fila del jugador 1 al inicio de la ronda ganada por el jugador 2
            if(currentTurn == 0 && condicion2.activeSelf)
            {
                rowsBlock1.SetActive(true);
            }
        }

        //Desbloquea el botón si ya ha pasado 1 turno en ambos jugadores y alguno está jugando
        if(currentTurn > 1 && !condicion2.activeSelf && !condicion1.activeSelf && !victory && winner != 2)
        {
            passButtonBlock.SetActive(false);
        }
        else if(currentTurn > 0 && !condicion2.activeSelf && !condicion1.activeSelf && !victory && winner == 2)
        {
            passButtonBlock.SetActive(false);
        }

        //Hasta el segundo turno no se pueden activar las habilidades de líder
        if((currentTurn < 2) && winner < 2)
        {
            leaderBlock1.SetActive(true);
            leaderBlock2.SetActive(true);
        }
        else if((currentTurn < 1) && winner == 2)
        {
            leaderBlock1.SetActive(true);
            leaderBlock2.SetActive(true);
        }
    }
}
