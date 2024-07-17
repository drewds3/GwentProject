using UnityEngine;
using System;
using static SetPlayers;
using System.Collections.Generic;
using System.Linq;

public class EffectCards : MonoBehaviour
{
    //Variables para los efectos
    private Player playerTrigger;
    private Player playerRival;
    private string tagRival;
    private string tagOwn;
    private string deckOwn;
    
    //Metodo para activar el efecto
    public void ActiveEffect(int numEffect)
    {
        if(player1.Faction == gameObject.GetComponent<Card>().Faction)
        {
            playerTrigger = player1;
            playerRival = player2;

            tagOwn = "CartaJugada1";
            tagRival = "CartaJugada2";

            deckOwn = "Deck";
        }
        else if(player2.Faction == gameObject.GetComponent<Card>().Faction)
        {
            playerTrigger = player2;
            playerRival = player1;

            tagOwn = "CartaJugada2";
            tagRival = "CartaJugada1";

            deckOwn = "DeckEnemy";
        }

        if (numEffect == 1) EliminateStrongerCard();
        else if(numEffect == 2) CleanRow();
        else if(numEffect == 3) EliminateWeakerCard();
        else if(numEffect == 4) BandReinforcement();
        else if(numEffect == 5) DrawCard();
        else if(numEffect == 6) Resucite();
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
                cardsRival[i].tag = "CartaDescartada2";
                cardsRival[i].transform.position = playerRival.Graveyard.position;
                cardsRival[i].transform.SetParent(playerRival.Graveyard);

                Debug.Log("Efecto de carta jugada activado");

                //Solo queremos que se elimine una en caso de haber varias con el mismo poder que sean "la más poderosa"
                break;
            }
            else if(cardsPower[i] == strongCard && i - cardsRival.Length < cardsOwn.Length)
            {
                cardsOwn[i - cardsRival.Length].tag = "CartaDescartada1";
                cardsOwn[i - cardsRival.Length].transform.position = playerTrigger.Graveyard.position;
                cardsOwn[i - cardsRival.Length].transform.SetParent(playerTrigger.Graveyard);

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

                        cartas[j].tag = "CartaDescartada2";
                        cartas[j].transform.position = playerTrigger.Graveyard.position;
                        cartas[j].transform.SetParent(playerTrigger.Graveyard);
                    }
                }
                else //Sino las manda al cementerio del adversario
                {
                    for(int j = 0; j < scriptCarta.Length; j++)
                    {
                        cartas[j] = scriptCarta[j].gameObject;

                        cartas[j].tag = "CartaDescartada1";
                        cartas[j].transform.position = playerRival.Graveyard.position;
                        cartas[j].transform.SetParent(playerRival.Graveyard);
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
                if(DragHandler.startParent == GameObject.Find("Hand").transform) cardsRival[i].tag = "CartaDescartada1";
                else cardsRival[i].tag = "CartaDescartada2";
                
                cardsRival[i].transform.position = playerRival.Graveyard.position;
                cardsRival[i].transform.SetParent(playerRival.Graveyard);

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

        Debug.Log("Efecto de carta jugada activado");
    }

    //Roba una carta extra
    public void DrawCard()
    {
        GameObject.Find(deckOwn).GetComponent<Deck>().DrawCard();
        Debug.Log("Efecto de carta jugada activado");
    }

    //Trae de vuelta una carta a la mano del invocante desde cualquier cementerio
    public void Resucite()
    {
        //Se recogen todas las cartas mandadas a ambos cementerios
        GameObject[] descartedCards1 = GameObject.FindGameObjectsWithTag("CartaDescartada1");
        GameObject[] descartedCards2 = GameObject.FindGameObjectsWithTag("CartaDescartada2");

        //Se filtran las cartas que sean del tipo Unidad de Plata
        List<GameObject> cards = new();

        for(int i = 0; i < descartedCards1.Length; i++)
        if(descartedCards1[i].GetComponent<Card>().Type == "Silver") cards.Add(descartedCards1[i]);

        for(int i = 0; i < descartedCards2.Length; i++)
        if(descartedCards2[i].GetComponent<Card>().Type == "Silver") cards.Add(descartedCards2[i]);

        //Se elige el índice de manera aleatoria
        int index = UnityEngine.Random.Range(0, cards.Count);

        GameObject resucitedCard;
 
        try
        {
            //Se encuentra dicha carta
            resucitedCard = cards[index];
        
            //Por último se manda a la mano del jugador que activó el efecto realizándose los cambios necesarios
            resucitedCard.tag = "Carta";

            if(DragHandler.startParent == GameObject.Find("Hand").transform)
            {
                resucitedCard.transform.SetParent(GameObject.Find("Hand").transform);
                resucitedCard.transform.position = GameObject.Find("Hand").transform.position;
                resucitedCard.GetComponent<Card>().Faction = player1.Faction;
                resucitedCard.GetComponent<Card>().Name += " (Resucitado)";
                resucitedCard.GetComponent<DragHandler>().enabled = true;
                Debug.Log("Efecto de carta jugada activado");
            }
            else
            {
                resucitedCard.transform.SetParent(GameObject.Find("HandEnemy").transform);
                resucitedCard.transform.position = GameObject.Find("HandEnemy").transform.position;
                resucitedCard.GetComponent<Card>().Faction = player2.Faction;
                resucitedCard.GetComponent<Card>().Name += " (Resucitado)";
                resucitedCard.GetComponent<DragHandler>().enabled = true;
                Debug.Log("Efecto de carta jugada activado");
            }
        }
        catch // Si no hay cartas en el cementerio pues no hace :(
        {
            Debug.Log("No se pudo activar el efecto");
        }
    }
}