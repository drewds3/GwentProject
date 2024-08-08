using UnityEngine;
using TMPro;
using static TurnsBasedSystem;

public class ScoreText : MonoBehaviour
{
    //Texto a mostrar en pantalla
    private TMP_Text text;

    //Variables para actualizar la puntuación del jugador 1
    public int scorePlayer1 = 0;
    public GameObject RowM1;
    public GameObject RowR1;
    public GameObject RowS1;

    //Variables para actualizar la puntuación del jugador 2
    public int scorePlayer2 = 0;
    public GameObject RowM2;
    public GameObject RowR2;
    public GameObject RowS2;

    //Variables para visualizar la ronda actual y quién ha ganado cada una
    public int winsP1 = 0;
    public int winsP2 = 0;

    void Start()
    {   
        //Inicializa la variable del texto
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {   
        //Se actualizan las puntuaciones a tiempo real
        ActualPlayerScore();
        ActualScore();
    }

    //Método que actualiza el texto mostrado en pantalla
    void ActualScore()
    {
        text.text = $"P1 Score: {scorePlayer1}\n\nP2 Score: {scorePlayer2}\n\nRound: {round}\n\nWins P1: {winsP1}/2\n\nWins P2: {winsP2}/2";
    }

    //Método que actualiza las puntuaciones de cada jugador
    void ActualPlayerScore()
    {
        //Se actualiza al jugador 1
        scorePlayer1 = RowM1.GetComponent<RowScoreController>().rowScore
                     + RowR1.GetComponent<RowScoreController>().rowScore 
                     + RowS1.GetComponent<RowScoreController>().rowScore;

        //Se actualiza al jugador 2
        scorePlayer2 = RowM2.GetComponent<RowScoreController>().rowScore
                     + RowR2.GetComponent<RowScoreController>().rowScore 
                     + RowS2.GetComponent<RowScoreController>().rowScore;
    }
}
