using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    //Variables para el audio
    AudioSource audio;
    public AudioClip clip1;
    public AudioClip clip2;

    //Al iniciarse la partida empieza a reproducirse el audio
    void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();
        audio.clip = clip1;
        audio.Play();
    }

    //Si es la última ronda cambia el audio a uno más de tensión
    public void LastRoundAudio()
    {
        audio.clip = clip2;
        audio.Play();
    }
}
