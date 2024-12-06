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

    [HideInInspector] public Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();
    public AudioSource reservadoAcaoFala;
    
    public void Awake()
    {
        instance = this;
    }

    public void PlayOnReservado(string path) {
        if (clips.ContainsKey(path)) {
            PlayOnReservado(clips[path]);
        } else {
            AudioClip clip = Resources.Load<AudioClip>(path);
            if (clip == null) {
                Debug.LogError("AudioClip não encontrado: " + path);
                return;
            }
            clips[path] = clip;
            PlayOnReservado(clip);
        }
    }

    public void PlayOnReservado(AudioClip clip) {
        reservadoAcaoFala.clip = clip;
        reservadoAcaoFala.Play();
    }
}


