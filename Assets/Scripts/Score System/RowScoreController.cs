using UnityEngine;

public class RowScoreController : MonoBehaviour
{
    //Variables para el sistema de puntuación
    public string typeRow;
    public int rowScore;
    private bool M = false;
    private bool R = false;
    private bool S = false;
    public GameObject climate;

    //Variable para el efecto de limpiar fila
    public int numberCards = 0;

    void Update()
    {
        //Se actualizan las zonas afectadas por el clima
        M = climate.GetComponent<ClimateSlots>().afectM;
        R = climate.GetComponent<ClimateSlots>().afectR;
        S = climate.GetComponent<ClimateSlots>().afectS;

        //Se calcula la puntuación total de la fila a tiempo real
        TotalScoreRow();
    }

    //Método para calcular la puntuación en la fila
    void TotalScoreRow()
    {   
        //El contador se inicia en 0
        rowScore = 0;

        Card[] cards = GetComponentsInChildren<Card>();

        //Se cuentan las cartas que hay para el efecto de limpiar fila
        numberCards = cards.Length;

        //Verifica si algún clima afecta a dicha fila
        if((typeRow == "M" && M) || (typeRow == "R" && R) || (typeRow == "S" && S))
        {
            foreach (Card card in cards)
            {
                //Si la carta es de oro se mantiene su poder
                if(card.Type == "Gold")
                {
                    rowScore += card.Power;
                }
                else //De lo contrario se divide su poder a la mitad
                {
                    rowScore += card.Power/2;
                }
            }
        }
        else //De lo contrario se procede con normalidad
        {
            foreach (Card card in cards)
            {
                rowScore += card.Power;
            }
        }
    }
}