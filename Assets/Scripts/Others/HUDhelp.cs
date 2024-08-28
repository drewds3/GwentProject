using TMPro;
using UnityEngine;
using static TurnsBasedSystem;

public class HUDhelp : MonoBehaviour
{
    TMP_Text text;
    GameObject BLeader1;
    GameObject BLeader2;

    void Start()
    {
        text = GameObject.Find("Contexto").GetComponent<TMP_Text>();
        BLeader1 = GameObject.Find("LeaderBlock1");
        BLeader2 = GameObject.Find("LeaderBlock2");
        text.text = "Hello, try draw 10 cards of your deck";
    }

    // Pequeña ayuda al usuario ;)
    public void FirstTurnPlayer()
    {
        if(currentTurn < 2 && round == 1)
        {
            text.text = "Try draw 10 cards of your deck";
        }
        else if((winner < 2 && currentTurn < 2 && round > 1) || (winner == 2 && currentTurn < 1 && round > 1))
        {
            text.text = "Try draw 2 cards of your deck";
        }
    }

    public void BlockEffectLeader()
    {
        if((BLeader1.activeSelf && !BLeader2.activeSelf && currentTurn%2 != 0) 
        || (!BLeader1.activeSelf && BLeader2.activeSelf && currentTurn%2 == 0))
        {
            text.text = "It's not your leader";
        }
        else if(BLeader1.activeSelf && BLeader2.activeSelf)
        {
            text.text = "You cannot activate the leader on the first turn of a round";
        }
    }
}
