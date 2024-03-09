using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenúInicial : MonoBehaviour
{
    // Funcion para iniciar una partida y cargar la escena siguiente a la actual.
    public void Jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Funcion para salir del juego
    public void Salir()
    {
        Debug.Log("Salir...");

        //Aplication.Quit();
    }
}
