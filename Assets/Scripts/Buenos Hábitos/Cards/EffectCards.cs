using UnityEngine;
using System;

public class EffectCards : MonoBehaviour
{
    //Variables para los efectos
    private Transform graveyardOwn;
    private Transform graveyardRival;
    private string tagRival;
    private string tagOwn;
    private string deckOwn;
    
    //Metodo para activar el efecto
    public void ActiveEffect(int numEffect)
    {
        Player player1 = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>().player1;
        Player player2 = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>().player2;

        if(player1.Faction == gameObject.GetComponent<Card>().Faction)
        {
            graveyardOwn = player1.Graveyard;
            graveyardRival = player2.Graveyard;

            tagOwn = "CartaJugada1";
            tagRival = "CartaJugada2";

            deckOwn = "Deck";
        }
        else if(player2.Faction == gameObject.GetComponent<Card>().Faction)
        {
            graveyardOwn = player2.Graveyard;
            graveyardRival = player1.Graveyard;

            tagOwn = "CartaJugada2";
            tagRival = "CartaJugada1";

            deckOwn = "DeckEnemy";
        }

        if (numEffect == 1) EliminateStrongerCard();
        else if(numEffect == 2) CleanRow();
        else if(numEffect == 3) EliminateWeakerCard();
        else if(numEffect == 4) BandReinforcement();
        else if(numEffect == 5) DrawCard();
    }

    //Efectos posibles: ---------------------------------------------------------------------------------------------------------------

    //Elimina la carta más poderosa en el campo
    public void EliminateStrongerCard()
    {
        //Primero se crean conjuntos (arrays) de las cartas jugadas por ambos jugadores
        GameObject[] cardsRival = GameObject.FindGameObjectsWithTag(tagRival);
        GameObject[] cardsOwn = GameObject.FindGameObjectsWithTag(tagOwn);

        int[] cardsPower = new int[cardsRival.Length + cardsOwn.Length];

        //Luego se busca en ambos conjuntos el valor más alto de poder
        for(int i = 0; i < cardsRival.Length; i++)
        {
            cardsPower[i] = cardsRival[i].GetComponent<Card>().Power;
        }

        for(int i = 0; i < cardsOwn.Length; i++)
        {
            cardsPower[cardsRival.Length + i] = cardsOwn[i].GetComponent<Card>().Power;
        }

        int strongCard = 0;

        for(int i = 0; i < cardsPower.Length; i++)
        {
            strongCard = Math.Max(strongCard, cardsPower[i]);
        }
        
        for(int i = 0; i < cardsOwn.Length; i++)
        {
            strongCard = Math.Max(strongCard, cardsPower[cardsRival.Length + i]);
        }

        /* Al encontrar dicho valor, se compara con el de las cartas jugadas
           y al encontrar la carta más poderosa, esta se ilimina */
        for(int i = 0; i < cardsPower.Length; i++)
        {
            if(cardsPower[i] == strongCard && i < cardsRival.Length)
            {
                cardsRival[i].tag = "CartaDescartada";
                cardsRival[i].transform.position = graveyardRival.position;
                cardsRival[i].transform.SetParent(graveyardRival);

                Debug.Log("Efecto de carta jugada activado");

                //Solo queremos que se elimine una en caso de haber varias con el mismo poder que sean "la más poderosa"
                break;
            }
            else if(cardsPower[i] == strongCard && i - cardsRival.Length < cardsOwn.Length)
            {
                cardsOwn[i - cardsRival.Length].tag = "CartaDescartada";
                cardsOwn[i - cardsRival.Length].transform.position = graveyardOwn.position;
                cardsOwn[i - cardsRival.Length].transform.SetParent(graveyardOwn);

                Debug.Log("Efecto de carta jugada activado");

                //Solo queremos que se elimine una en caso de haber varias con el mismo poder que sean "la más poderosa"
                break;
            }
        }
    }

    //Limpia la fila con menos unidades
    public void CleanRow()
    {
        //Primero busca en el conjunto (array) de las filas 
        GameObject[] rows = GameObject.FindGameObjectsWithTag("Fila");

        int min = 7;

        //Encuantra cúal es el menor números de cartas en una fila distinto de cero
        for(int i = 0; i < rows.Length; i++)
        {   
            if(rows[i].GetComponent<RowScoreController>().numberCards != 0)
            {
                min = Math.Min(rows[i].GetComponent<RowScoreController>().numberCards, min);
            }
        }

        //Luego encuentra la fila que tiene ese menor número y la limpia
        for(int i = 0; i < rows.Length; i++)
        {
            if(min == rows[i].GetComponent<RowScoreController>().numberCards)
            {   
                //Obtiene las cartas de la fila
                Card[] scriptCarta = rows[i].GetComponentsInChildren<Card>();
                GameObject[] cartas = new GameObject[scriptCarta.Length];

                //Si la fila es suya manda las cartas a su cementerio
                if(scriptCarta[0].Faction == "Raven")
                {
                    for(int j = 0; j < scriptCarta.Length; j++)
                    {
                        cartas[j] = scriptCarta[j].gameObject;

                        cartas[j].tag = "CartaDescartada";
                        cartas[j].transform.position = graveyardOwn.position;
                        cartas[j].transform.SetParent(graveyardOwn);
                    }
                }
                else //Sino las manda al cementerio del adversario
                {
                    for(int j = 0; j < scriptCarta.Length; j++)
                    {
                        cartas[j] = scriptCarta[j].gameObject;

                        cartas[j].tag = "CartaDescartada";
                        cartas[j].transform.position = graveyardRival.position;
                        cartas[j].transform.SetParent(graveyardRival);
                    }
                }

                Debug.Log("Efecto de carta jugada activado");

                /*Solo queremos que se limpie una en caso de haber varias
                  con la menor cantidad de cartas */
                break;
            }
        }
    }

    //Elimina la carta más débil del rival
    public void EliminateWeakerCard()
    {
        //Primero se crea un conjuntos (array) de las cartas jugadas por el jugador rival
        GameObject[] cardsRival = GameObject.FindGameObjectsWithTag(tagRival);

        int[] cardsPower = new int[cardsRival.Length];

        //Luego se busca el valor más bajo de poder
        for(int i = 0; i < cardsRival.Length; i++)
        {
            cardsPower[i] = cardsRival[i].GetComponent<Card>().Power;
        }

        int weakerCard = 99;

        for(int i = 0; i < cardsPower.Length; i++)
        {
            weakerCard = Math.Min(weakerCard, cardsPower[i]);
        }

        /* Al encontrar dicho valor, se compara con el de las cartas jugadas
           y al encontrar la carta más débil, esta se elimina */
        for(int i = 0; i < cardsPower.Length; i++)
        {
            if(cardsPower[i] == weakerCard)
            {
                cardsRival[i].tag = "CartaDescartada";
                cardsRival[i].transform.position = graveyardRival.position;
                cardsRival[i].transform.SetParent(graveyardRival);

                Debug.Log("Efecto de carta jugada activado");

                //Solo queremos que se elimine una en caso de haber varias con el mismo poder que sean "la más poderosa"
                break;
            }
        }
    }

    /*Si hay otras cartas del mismo tipo, a esta se le aumenta su poder
      n veces la cantidad de cartas iguales a esta en la fila*/
    public void BandReinforcement()
    {
        GameObject slot = transform.parent.gameObject;
        GameObject row = slot.transform.parent.gameObject;

        Card[] scriptCarta = row.GetComponentsInChildren<Card>();

        int n = 0;

        //Se cuentan las cartas que sean iguales a esta
        for(int i = 0; i < scriptCarta.Length; i++)
        {
            if(scriptCarta[i].Name == gameObject.GetComponent<Card>().Name)
            {
                n++;
            }
        }

        //Luego se aumenta su poder la cantidad n veces
        gameObject.GetComponent<Card>().IncreasePower(n);
    }

    //Roba una carta extra
    public void DrawCard()
    {
        GameObject.Find(deckOwn).GetComponent<DrawCards>().DrawCard();
    }
}