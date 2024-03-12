using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards2 : MonoBehaviour
{
    public GameObject[] prefabs;
    public Transform cartasZone;
    public int numCartas = 10;

    public void GenerarCartas()
    {
        if (cartasZone == null)
        {
            Debug.LogError("La zona de cartas no está asignada.");
            return;
        }

        if (numCartas <= 0 || numCartas > prefabs.Length)
        {
            Debug.LogError("El número de cartas no es válido.");
            return;
        }

        List<int> indicesSeleccionados = new List<int>(numCartas);

        for (int i = 0; i < numCartas; i++)
        {
            int indice;
            do{

                indice = Random.Range(0, prefabs.Length);
                
            } while (indicesSeleccionados.Contains(indice));

            indicesSeleccionados.Add(indice);
        }

        foreach (int indice in indicesSeleccionados)
        {
            GameObject carta = Instantiate(prefabs[indice], cartasZone);
        }
    }
}