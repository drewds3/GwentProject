using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public List<GameObject> prefabs = new List<GameObject>();
    public Transform cartasZone;
    int indice;

    public void OnClick()
    {
        if(prefabs.Count==0)
        {
            Debug.LogError("Se acabaron las cartas chamacón");
        }
        else
        {
            if(cartasZone.childCount<10)
            {
                indice = Random.Range(0, prefabs.Count);

                Instantiate(prefabs[indice], cartasZone);

                prefabs.Remove(prefabs[indice]);
            }
        }
    }
}