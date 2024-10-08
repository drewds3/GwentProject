using UnityEngine;
using UnityEngine.Audio;

public class MenuOpciones : MonoBehaviour
{   
    //Variable para el volumen del juego
    [SerializeField] private AudioMixer audioMixer;

    //Opción de alternar entre pantalla completa y modo ventana
    public void activarPantallaCompleta(bool pantallaCompleta)
    {
        Screen.fullScreen = pantallaCompleta;
    }

    //Opción de cambiar el volumen
    public void CambiarVolumen(float volumen)
    {
        audioMixer.SetFloat("Volumen", volumen);
    }
}
