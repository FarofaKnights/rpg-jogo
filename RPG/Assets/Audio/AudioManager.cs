using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource playerTakeDamage;
    public AudioSource playerattackSwish;
    public AudioSource playerAttackHit;
    public AudioSource playerFootsteps;
    public AudioSource enemyDeath;

    public AudioSource doorOpen;
    public AudioSource doorClose;
    public AudioSource vaultSlam;

    public AudioSource elevatorUp;


    public AudioSource musica;

    public AudioSource reservadoAcaoFala;
    
    public void Awake()
    {
        instance = this;
    }

    public void PlayOnReservado(string path) {
        AudioClip clip = GameManager.instance.loaded_audioClips.Get(path);
        PlayOnReservado(clip);
    }

    public void PlayOnReservado(AudioClip clip) {
        reservadoAcaoFala.clip = clip;
        reservadoAcaoFala.Play();
    }
}


