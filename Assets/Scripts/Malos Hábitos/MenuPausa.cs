using UnityEngine;

public class MenuPausa : MonoBehaviour
{   
    //Variables para mostrar el menú de pausa mientras se juega
    public GameObject pauseMenu;
    public GameObject playZone;
    public KeyCode esc;

    void Update()
    {   
        //Simplemente al pulsar el botón escape se activa o desactiva
        if(Input.GetKeyDown(esc))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            playZone.SetActive(!playZone.activeSelf);
        }
    }
}
