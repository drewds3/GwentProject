using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobarCartas : MonoBehaviour
{
    public GameObject mazo;

    public GameObject zona;

    public GameObject ExampleCard;

    public List<GameObject> cartasDisponibles;
    
    void Start()
    {
    cartasDisponibles = new List<GameObject>();
    
    cartasDisponibles.Add(ExampleCard);
    //cartasDisponibles.Add(/* Referencia a la carta 2 */);
    }

    GameObject RobarCarta()
{
    if (cartasDisponibles.Count > 0)
    {
        int index = Random.Range(0, cartasDisponibles.Count);
        GameObject carta = cartasDisponibles[index];
        cartasDisponibles.RemoveAt(index);
        return carta;
    }
    else
    {
        Debug.LogWarning("No hay más cartas en el mazo.");
        return null;
    }
}

    void ColocarCartaEnZona(GameObject carta)
    {
        if (carta != null)
       {
            GameObject cartaInstanciada = Instantiate(carta, new Vector3(0, 0, 0), Quaternion.identity);
            cartaInstanciada.transform.SetParent(zona.transform);
        }
    }
    public void OnBotonTocado()
{
    GameObject cartaRobada = RobarCarta();
    ColocarCartaEnZona(cartaRobada);
}
}
