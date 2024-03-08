using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenúInicial : MonoBehaviour
{
    //Este método se usa para cambiar de escena.
    public void Jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Este otro para salir de la aplicación.
    public void Salir()
    {
        Debug.Log("Salir...");

        //Aplication.Quit();
    }
}
