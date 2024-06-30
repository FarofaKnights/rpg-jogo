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
    public AudioSource enemyAlert;
    public AudioSource enemyAttack;
    public AudioSource enemyDeath;

    public AudioSource musica;
    
    public void Awake()
    {
        instance = this;
    }
}


