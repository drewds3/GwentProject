using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{   
    //Lista de cartas
    public List<GameObject> prefabs = new List<GameObject>();
    
    //Zona donde se "dibujan"
    public Transform cartasZone;

    //Carta a dibujar
    int indice;

    //Método que "dibuja" las cartas
    public void OnClick()
    {
        //Si hay cartas dibuja la que está en el indice aleatorio
        if(prefabs.Count==0)
        {
            Debug.LogError("Se acabaron las cartas chamacón");
        }
        else
        {
            //El límite de cartas en la mano es 10
            if(cartasZone.childCount<10)
            {
                indice = Random.Range(0, prefabs.Count);

                Instantiate(prefabs[indice], cartasZone);

                prefabs.Remove(prefabs[indice]);
            }
        }
    }
}