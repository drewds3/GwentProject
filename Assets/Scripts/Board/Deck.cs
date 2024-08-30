using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SetPlayers;
using static TurnsBasedSystem;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour
{   
    //Mazo predeterminado
    public List<GameObject> deck = new();

    //
    delegate void Draw();
    Draw draw;
    
    //Zona donde se instancian
    public Transform hand;

    //Zona donde se descartan, es decir, el cementerio
    public Transform graveyard;

    //Carta a dibujar
    int index;

    //Sistema de bloqueadores
    public GameObject blockRows1;
    public GameObject blockRows2;
    public GameObject blockPassButton;
    public int count = 0;

    //Variables para cambiar de cartas
    public GameObject cardChangeSlot;
    public Transform trash;
    private int count2 = 0;

    //Dueño del mazo
    public string owner;

    void Start()
    {
        draw = DrawCard;
    }

    //Método que "dibuja" las cartas
    public void OnClick()
    {
        //Si hay cartas roba la que está en el index aleatorio
        if(deck.Count == 0)
        {
            Debug.LogError("Se acabaron las cartas");

            TMP_Text text = GameObject.Find("Contexto").GetComponent<TMP_Text>();
            text.text = "You ran out of cards :(";
        }
        else
        {  
            //Comprueba en cuál ronda y turno están
            if(round == 1 && currentTurn < 2)
            {
                //Si es el primer turno de la 1ra ronda roban 10 cartas
                if(hand.childCount < 10)
                {
                    draw();

                    //Si ya tiene las 10 se le permite jugar y se activa la posibilidad de cambiar de carta
                    if(hand.childCount == 10 && count == 0)
                    {
                        blockRows1.SetActive(false);
                        blockRows2.SetActive(false);
                        blockPassButton.SetActive(false);

                        cardChangeSlot.SetActive(true);

                        Debug.Log("Ya puedes jugar");

                        TMP_Text text = GameObject.Find("Contexto").GetComponent<TMP_Text>();
                        text.text = "Now, you can change up to two cards by shuffling them into the deck, play a card, or pass a turn";
                    }
                }
                else
                {
                    Debug.Log("No puedes robar más en este turno");

                    TMP_Text text = GameObject.Find("Contexto").GetComponent<TMP_Text>();
                    text.text = "You can't draw more cards in this turn";
                }
            }
            //Si estamos en la 2da o 3ra ronda y es el primer turno roban 2 cartas
            else if(round > 1 && ((currentTurn < 2 && winner < 2) || (currentTurn < 1 && winner == 2)))
            {
                //Si tiene menos de 10 cartas, roba
                if(hand.childCount<10 && ((count<2 && round == 2) || (count<4 && round == 3)))
                {
                    draw();
                    count++;
                }
                //De no ser así las cartas robadas serán descartadas al cementerio
                else if((count != 2 && round == 2) || (count !=4 && round == 3))
                {
                    index = Random.Range(0, deck.Count);

                    GameObject card = Instantiate(deck[index], graveyard);

                    if(gameObject == player1.Deck) card.tag = "CartaDescartada1";
                    else if(gameObject == player2.Deck) card.tag = "CartaDescartada2";
        
                    deck.Remove(deck[index]);

                    count++;
                }
                
                //Si ya se robaron 2 cartas se permite jugar
                if((count==2 && round==2) || (count==4 && round==3))
                {
                    blockRows1.SetActive(false);
                    blockRows2.SetActive(false);
                    blockPassButton.SetActive(false);

                    Debug.Log("Ya puedes jugar");

                    TMP_Text text = GameObject.Find("Contexto").GetComponent<TMP_Text>();
                    text.text = "You can play now!";
                }

            }
            //Si ya pasó el primer turno de ambos jugadores no pueden robar más cartas
            else
            {
                Debug.Log("No puedes robar cartas en este turno");

                TMP_Text text = GameObject.Find("Contexto").GetComponent<TMP_Text>();
                text.text = "You can't draw more cards in this turn";
            }
        }
    }

    //Método que añade la carta a la lista(mazo) y luego la "descarta"
    public void Swap()
    {
        //Se añade la carta al mazo
        deck.Add(cardChangeSlot.GetComponent<ChangeCard>().item);

        //Se envía a un objeto auxiliar para que desaparezca de la escena
        cardChangeSlot.GetComponent<ChangeCard>().item.transform.SetParent(trash);
        cardChangeSlot.GetComponent<ChangeCard>().item.transform.position = trash.position;

        count2++;

        //Solo se puede usar 2 veces
        if(count2 == 2)
        {
            cardChangeSlot.SetActive(false);
        }
    }

    //Al activarse el respectivo efecto de carta se roba una carta extra
    public void DrawCard()
    {
        //Si hay menos de 10 cartas se roba una
        if(hand.childCount < 10)
        {
            index = Random.Range(0, deck.Count);

            if(deck[index].GetComponent<Card>() is not NewCard)
            {
                GameObject card = Instantiate(deck[index], hand);
                card.GetComponent<Card>().Owner = owner;
            }
            else
            {
                GameObject card = deck[index];

                card.transform.position = hand.position;
                card.transform.SetParent(hand);
                card.GetComponent<Card>().Owner = owner;
            }

            deck.Remove(deck[index]);
        }
        else //De lo contrario se descarta al cementerio
        {
            index = Random.Range(0, deck.Count);

            GameObject card = Instantiate(deck[index], graveyard);

            if(gameObject == player1.Deck) card.tag = "CartaDescartada1";
            else if(gameObject == player2.Deck) card.tag = "CartaDescartada2";

            card.GetComponent<Card>().Owner = owner;
        
            deck.Remove(deck[index]);
        }
    }

    //Método para crear el mazo personalizado
    public void NewDeck()
    {
        deck.Clear();

        Translator translator = GameObject.Find("Botón Confirmar").GetComponent<Translator>();

        for(int i = 0; i < translator.cards.Count; i++)
        {
            deck.Add(translator.cards[i]);
        }

        draw = DrawCard2;
    }

    //Al activarse el respectivo efecto de carta se roba una carta extra
    public void DrawCard2()
    {
        //Si hay menos de 10 cartas se roba una
        if(hand.childCount < 10)
        {
            index = Random.Range(0, deck.Count);

            GameObject card = deck[index];

            card.transform.position = hand.position;
            card.transform.SetParent(hand);
            card.GetComponent<Card>().Owner = owner;

            deck.Remove(deck[index]);
        }
        else //De lo contrario se descarta al cementerio
        {
            index = Random.Range(0, deck.Count);

            GameObject card = deck[index];

            card.transform.position = graveyard.position;
            card.transform.SetParent(graveyard);

            if(gameObject == player1.Deck) card.tag = "CartaDescartada1";
            else if(gameObject == player2.Deck) card.tag = "CartaDescartada2";

            card.GetComponent<Card>().Owner = owner;
        
            deck.Remove(deck[index]);
        }
    }
}