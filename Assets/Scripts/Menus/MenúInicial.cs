using UnityEngine;
using UnityEngine.SceneManagement;

public class MenúInicial : MonoBehaviour
{
    //Al pulsar el botón se carga la escena siguiente, es decir, se inicia la partida
    public void Jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Al tocar el botón se cierra la aplicación
    public void Salir()
    {
        Debug.Log("Salir...");

        Application.Quit();
    }

    //Al pulsar el botón regresa al menú inicial
     public void BackToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
