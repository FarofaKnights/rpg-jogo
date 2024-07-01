using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.IO;
using Cinemachine;


public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    public const float minVolume = -80, maxVolume = 20;
    [Range(minVolume, maxVolume)]
    public float masterVolume, musicVolume, effectVolume;
    public Slider masterSlider, musicSlider, effectSlider, sensiX, sensiY;
    public AudioMixerGroup master, music, effect;
    string settingsPath;
    public GameObject settingsPanel;
    public GameObject cinemachineCamera;
    public bool invertedX = false, invertedY = true;


    [Range(100, 1000)]
    public float senseX;

    [Range(1, 5)]
    public float senseY;

    public GameObject checkBoxX, checkBoxY;
    public void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        settingsPath = Application.dataPath + "Settings.txt";

        masterSlider.onValueChanged.AddListener(delegate { ChangeMasterVolume(); });
        masterSlider.minValue = minVolume;
        masterSlider.maxValue = maxVolume;

        musicSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        musicSlider.minValue = minVolume;
        musicSlider.maxValue = maxVolume;

        effectSlider.onValueChanged.AddListener(delegate { ChangeEffectVolume(); });
        effectSlider.minValue = minVolume;
        effectSlider.maxValue = maxVolume;

        sensiX.onValueChanged.AddListener(delegate { ChangeSenseX(); });
        sensiX.minValue = 100;
        sensiX.maxValue = 1000;
        sensiX.value = 300;

        sensiY.onValueChanged.AddListener(delegate { ChangeSenseY(); });
        sensiY.minValue = 1;
        sensiY.maxValue = 5;
        sensiY.value = 2;

        LoadValues();
    }



    public void ChangeMasterVolume()
    {
        masterVolume = masterSlider.value;
        master.audioMixer.SetFloat("VolumeMaster", masterVolume);
    }
    public void ChangeMusicVolume()
    {
        musicVolume = musicSlider.value;
        music.audioMixer.SetFloat("VolumeMusic", musicVolume);
    }

    public void ChangeEffectVolume()
    {
        effectVolume = effectSlider.value;
        effect.audioMixer.SetFloat("VolumeEffects", effectVolume);
    }

    public void ChangeSenseX()
    {
        senseX = sensiX.value;
        if (cinemachineCamera != null)
        {
            cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_MaxSpeed = senseX;
        }
    }

    public void ChangeSenseY()
    {
        senseY = sensiY.value;
        if (cinemachineCamera != null)
        {
            cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_MaxSpeed = senseY;
        }
    }

    public void WriteValues()
    {
        SettingsValues sv = new SettingsValues();
        sv.masterVolume = masterVolume;
        sv.musicVolume = musicVolume;
        sv.effectVolume = effectVolume;
        sv.cameraInvertX = invertedX;
        sv.cameraInvertY = invertedY;
        sv.senseX = senseX;
        sv.senseY = senseY;
        string saveSettings = JsonUtility.ToJson(sv, true);
        File.WriteAllText(settingsPath, saveSettings);
    }

    public void LoadValues()
    {
        string loadSettings = File.ReadAllText(settingsPath);
        SettingsValues loadValues = JsonUtility.FromJson<SettingsValues>(loadSettings);

        masterVolume = loadValues.masterVolume;
        master.audioMixer.SetFloat("VolumeMaster", masterVolume);
        masterSlider.value = masterVolume;

        musicVolume = loadValues.musicVolume;
        music.audioMixer.SetFloat("VolumeMusic", musicVolume);
        musicSlider.value = musicVolume;

        effectVolume = loadValues.effectVolume;
        effect.audioMixer.SetFloat("VolumeEffects", effectVolume);
        effectSlider.value = effectVolume;

        invertedX = loadValues.cameraInvertX;
        invertedY = loadValues.cameraInvertY;

        senseX = loadValues.senseX;
        sensiX.value = senseX;
        senseY = loadValues.senseY;
        sensiY.value = senseY;
        LoadCam();
    }

    public void ChangeInvertX()
    {
        if(invertedX)
        {
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InvertInput = false;
            }
            checkBoxX.SetActive(false);
            invertedX = false;
        }

        else
        {
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InvertInput = true;
            }

            checkBoxX.SetActive(true);
            invertedX = true;
        }
    }

    public void ChangeInvertY()
    {
        if (invertedY)
        {
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput = false;
            }
            checkBoxY.SetActive(false);
            invertedY = false;
        }

        else
        {
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput = true;
            }

            checkBoxY.SetActive(true);
            invertedY = true;
        }
    }

    public void LoadCam()
    {

        //X
        if(invertedX)
        {
            checkBoxX.SetActive(true);
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InvertInput = true;
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_MaxSpeed = senseX;
            }
        }
        else
        {
            checkBoxX.SetActive(false);
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InvertInput = false;
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_MaxSpeed = senseX;
            }
        }



        //Y
        if (invertedY)
        {
            checkBoxY.SetActive(true);
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput = true;
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_MaxSpeed = senseY;
            }
        }
        else
        {
            checkBoxY.SetActive(false);
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput = false;
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_MaxSpeed = senseY;
            }
        }
    }

}

public class SettingsValues
{
    public float masterVolume;
    public float musicVolume;
    public float effectVolume;
    public float senseX;
    public float senseY;
    public bool cameraInvertX;
    public bool cameraInvertY;
}
