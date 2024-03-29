using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject playZone;
    public KeyCode esc;

    void Update()
    {
        if(Input.GetKeyDown(esc))
        {
            Debug.Log("sa");
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            playZone.SetActive(!playZone.activeSelf);
        }
    }
}
