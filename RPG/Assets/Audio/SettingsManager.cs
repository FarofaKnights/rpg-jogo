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
    public Slider masterSlider, musicSlider, effectSlider;
    public AudioMixerGroup master, music, effect;
    string settingsPath;
    public GameObject settingsPanel;
    public GameObject cinemachineCamera;
    public bool invertedX = false, invertedY = true;

    public GameObject checkBoxX, checkBoxY;
    public void Awake()
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

        LoadValues();
    }



    public void ChangeMasterVolume()
    {
        masterVolume = masterSlider.value;
        master.audioMixer.SetFloat("VolumeMaster", masterVolume);
        WriteValues();
    }
    public void ChangeMusicVolume()
    {
        musicVolume = musicSlider.value;
        music.audioMixer.SetFloat("VolumeMusic", musicVolume);
        WriteValues();
    }

    public void ChangeEffectVolume()
    {
        effectVolume = effectSlider.value;
        effect.audioMixer.SetFloat("VolumeEffects", effectVolume);
        WriteValues();
    }

    public void WriteValues()
    {
        SettingsValues sv = new SettingsValues();
        sv.masterVolume = masterVolume;
        sv.musicVolume = musicVolume;
        sv.effectVolume = effectVolume;
        sv.cameraInvertX = invertedX;
        sv.cameraInvertY = invertedY;
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
            }
        }
        else
        {
            checkBoxX.SetActive(false);
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InvertInput = false;
            }
        }

        //Y
        if (invertedY)
        {
            checkBoxY.SetActive(true);
            invertedY = true;
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput = true;
            }
        }
        else
        {
            checkBoxY.SetActive(false);
            invertedY = false;
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput = false;
            }
        }
    }

}

public class SettingsValues
{
    public float masterVolume;
    public float musicVolume;
    public float effectVolume;
    public bool cameraInvertX;
    public bool cameraInvertY;
}
