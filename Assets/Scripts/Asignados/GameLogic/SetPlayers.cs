using UnityEngine;

public class SetPlayers : MonoBehaviour
{
    //Jugadores
    public static Player player1;
    public static Player player2;

    //Propiedades de los jugadores
    public GameObject hand1;
    public GameObject dekc1;
    public Transform graveyard1;
    public GameObject handBlock1;
    public GameObject deckBlock1;
    public GameObject nextTurnCartel1;
    public GameObject blockLeader1;

    public GameObject hand2;
    public GameObject dekc2;
    public Transform graveyard2;
    public GameObject handBlock2;
    public GameObject deckBlock2;
    public GameObject nextTurnCartel2;
    public GameObject blockLeader2;

    void Awake()
    {
        //Se le otorgan las propiedades a los jugadores
        Player playerX = new(hand1, dekc1, graveyard1, "Dragon", handBlock1, deckBlock1, nextTurnCartel1, blockLeader1);
        player1 = playerX;

        Player playerY = new(hand2, dekc2, graveyard2, "Raven", handBlock2, deckBlock2, nextTurnCartel2, blockLeader2);
        player2 = playerY;
    }
}
