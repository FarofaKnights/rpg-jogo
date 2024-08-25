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
    public Slider masterSlider, musicSlider, effectSlider, sensi;
    public AudioMixerGroup master, music, effect;
    string settingsPath;
    GameObject cinemachineCamera;
    public bool invertedX = false, invertedY = true;

    public Toggle invertXBtn, invertYBtn, dontInvertXBtn, dontInvertYBtn;
    public bool hasChanged = false;


    [Range(0, 1)]
    public float sense;

    public float senseX { get { return sense * 1000 + 100; } }
    public float senseY { get { return sense * 5 + 1; } }

    public void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        settingsPath = Application.dataPath + "Settings.txt";

        invertXBtn.onValueChanged.AddListener(delegate { ChangeInvertX(); });
        invertYBtn.onValueChanged.AddListener(delegate { ChangeInvertY(); });

        masterSlider.onValueChanged.AddListener(delegate { ChangeMasterVolume(); });
        masterSlider.minValue = minVolume;
        masterSlider.maxValue = maxVolume;

        musicSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        musicSlider.minValue = minVolume;
        musicSlider.maxValue = maxVolume;

        effectSlider.onValueChanged.AddListener(delegate { ChangeEffectVolume(); });
        effectSlider.minValue = minVolume;
        effectSlider.maxValue = maxVolume;

        sensi.onValueChanged.AddListener(delegate { ChangeSense(); });
        sense = 0.33f;

        if (Player.instance != null)
            cinemachineCamera = Player.instance.thirdPersonCam.gameObject;

        LoadValues();
    }



    public void ChangeMasterVolume()
    {
        masterVolume = masterSlider.value;
        master.audioMixer.SetFloat("VolumeMaster", masterVolume);

        hasChanged = true;
    }
    public void ChangeMusicVolume()
    {
        musicVolume = musicSlider.value;
        music.audioMixer.SetFloat("VolumeMusic", musicVolume);

        hasChanged = true;
    }

    public void ChangeEffectVolume()
    {
        effectVolume = effectSlider.value;
        effect.audioMixer.SetFloat("VolumeEffects", effectVolume);

        hasChanged = true;
    }

    public void ChangeSense()
    {
        sense = sensi.value;
        if (cinemachineCamera != null) {
            cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_MaxSpeed = senseX;
            cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_MaxSpeed = senseY;
        }

        hasChanged = true;
    }

    public void WriteValues()
    {
        SettingsValues sv = new SettingsValues();
        sv.masterVolume = masterVolume;
        sv.musicVolume = musicVolume;
        sv.effectVolume = effectVolume;
        sv.cameraInvertX = invertedX;
        sv.cameraInvertY = invertedY;
        sv.sense = sense;
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

        sense = loadValues.sense;
        sensi.value = sense;
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

            dontInvertXBtn.isOn = true;
            invertedX = false;
        }

        else
        {
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InvertInput = true;
            }

            invertXBtn.isOn = true;
            invertedX = true;
        }

        WriteValues();
    }

    public void ChangeInvertY()
    {
        if (invertedY)
        {
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput = false;
            }

            dontInvertYBtn.isOn = true;
            invertedY = false;
        }

        else
        {
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput = true;
            }

            invertYBtn.isOn = true;
            invertedY = true;
        }

        WriteValues();
    }

    public void LoadCam()
    {

        //X
        if(invertedX)
        {
            invertXBtn.isOn = true;
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InvertInput = true;
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_MaxSpeed = senseX;
            }
        }
        else
        {
            dontInvertXBtn.isOn = true;
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InvertInput = false;
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_MaxSpeed = senseX;
            }
        }



        //Y
        if (invertedY)
        {
            invertYBtn.isOn = true;
            if (cinemachineCamera != null)
            {
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InvertInput = true;
                cinemachineCamera.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_MaxSpeed = senseY;
            }
        }
        else
        {
            dontInvertYBtn.isOn = true;
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
    public float sense;
    public bool cameraInvertX;
    public bool cameraInvertY;
}
