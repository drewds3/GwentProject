using System.Collections.Generic;
using UnityEngine;
using static SetPlayers;

public class DrawCards : MonoBehaviour
{   
    //Mazo predeterminado
    public List<GameObject> deck = new();
    
    //Zona donde se "dibujan"
    public Transform cartasZone;

    //Zona donde se descartan, es decir, el cementerio
    public Transform graveyard;

    //Carta a dibujar
    int indice;

    //Ronda Actual
    int round;

    //Turno actual
    public int currentTurn;

    //Sistema de bloqueadores
    public GameObject blockRows1;
    public GameObject blockRows2;
    public GameObject blockPassButton;
    public GameObject condicion1;
    public GameObject condicion2;
    public GameObject leaderBlock;
    public int count = 0;
    private bool condicion3 = false;
    private int roundWinner = 0;

    //Variables para cambiar de cartas
    public GameObject cardChangeSlot;
    public Transform trash;
    private int count2 = 0;

    void Update()
    {   
        //Se actualiza la ronda, los turnos, si ya alguien ganó y si el jugador 2 ganó alguna ronda
        round = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>().round;
        currentTurn = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>().currentTurn;
        roundWinner = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>().winner;
        condicion3 = GameObject.Find("Tablero").GetComponent<TurnsBasedSystem>().victory;
        
        //Verifica quién fue el último que ganó
        if(roundWinner < 2)
        {
            //Se bloquean la fila del jugador 2 al inicio de la ronda ganada por el jugador 1 y la inicial
            if(currentTurn == 1 && condicion1.activeSelf)
            {
                blockRows2.SetActive(true);
            }
        }
        else
        {
            //Se bloquean la fila del jugador 1 al inicio de la ronda ganada por el jugador 2
            if(currentTurn == 0 && condicion2.activeSelf)
            {
                blockRows1.SetActive(true);
            }
        }

        //Desbloquea el botón si ya ha pasado 1 turno en ambos jugadores y alguno está jugando
        if(currentTurn > 1 && !condicion2.activeSelf && !condicion1.activeSelf && !condicion3 && roundWinner != 2)
        {
            blockPassButton.SetActive(false);
        }
        else if(currentTurn > 0 && !condicion2.activeSelf && !condicion1.activeSelf && !condicion3 && roundWinner == 2)
        {
            blockPassButton.SetActive(false);
        }

        //Hasta el segundo turno no se pueden activar las habilidades de líder
        if((currentTurn < 2) && roundWinner < 2)
        {
            leaderBlock.SetActive(true);
        }
        else if((currentTurn < 1) && roundWinner == 2)
        {
            leaderBlock.SetActive(true);
        }
        
    }

    //Método que "dibuja" las cartas
    public void OnClick()
    {
        //Si hay cartas roba la que está en el indice aleatorio
        if(deck.Count==0)
        {
            Debug.LogError("Se acabaron las cartas");
        }
        else
        {  
            //Comprueba en cuál ronda y turno están
            if(round == 1 && currentTurn < 2)
            {
                //Si es el primer turno de la 1ra ronda roban 10 cartas
                if(cartasZone.childCount<10)
                {
                    indice = Random.Range(0, deck.Count); 

                    Instantiate(deck[indice], cartasZone);

                    deck.Remove(deck[indice]);
                }
                else
                {
                    Debug.Log("No puedes robar más en este turno");
                }

                //Si ya tiene las 10 se le permite jugar y se activa la posibilidad de cambiar de carta
                if(cartasZone.childCount == 10 && count == 0)
                {
                    blockRows1.SetActive(false);
                    blockRows2.SetActive(false);
                    blockPassButton.SetActive(false);

                    cardChangeSlot.SetActive(true);

                    Debug.Log("Ya puedes jugar");
                }
            }
            //Si estamos en la 2da o 3ra ronda y es el primer turno roban 2 cartas
            else if(round > 1 && ((currentTurn < 2 && roundWinner < 2) || (currentTurn < 1 && roundWinner == 2)))
            {
                //Si tiene menos de 10 cartas, roba
                if(cartasZone.childCount<10 && ((count<2 && round == 2) || (count<4 && round == 3)))
                {
                    indice = Random.Range(0, deck.Count);

                    Instantiate(deck[indice], cartasZone);

                    deck.Remove(deck[indice]);

                    count++;

                    Debug.Log(count);
                }
                //De no ser así las cartas robadas serán descartadas al cementerio
                else if((count != 2 && round == 2) || (count !=4 && round == 3))
                {
                    indice = Random.Range(0, deck.Count);

                    GameObject card = Instantiate(deck[indice], graveyard);

                    if(gameObject == player1.Deck) card.tag = "CartaDescartada1";
                    else if(gameObject == player2.Deck) card.tag = "CartaDescartada2";
        
                    deck.Remove(deck[indice]);

                    count++;

                    Debug.Log(count);
                }
                
                //Si ya se robaron 2 cartas se permite jugar
                if((count==2 && round==2) || (count==4 && round==3))
                {
                    blockRows1.SetActive(false);
                    blockRows2.SetActive(false);
                    blockPassButton.SetActive(false);

                    Debug.Log("Ya puedes jugar");
                }

            }
            //Si ya pasó el primer turno de ambos jugadores no pueden robar más cartas
            else
            {
                Debug.Log("No puedes robar cartas en este turno");
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
        //Si en la mano hay menos de 10 cartas se roba
        if(cartasZone.childCount<10)
        {
            indice = Random.Range(0, deck.Count);

            Instantiate(deck[indice], cartasZone);

            deck.Remove(deck[indice]);
        }
        else //Sino se descarta la carta robada
        {
            indice = Random.Range(0, deck.Count);

            GameObject card = Instantiate(deck[indice], graveyard);

            if(gameObject == player1.Deck) card.tag = "CartaDescartada1";
            else if(gameObject == player2.Deck) card.tag = "CartaDescartada2";

            deck.Remove(deck[indice]);
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
    }
}