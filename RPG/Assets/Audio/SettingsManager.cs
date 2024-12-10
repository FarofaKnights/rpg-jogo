using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.IO;
using Cinemachine;
using Defective.JSON;

public class SettingsManager : MonoBehaviour, Saveable
{
    public static SettingsManager instance;
    public const float minVolume = -60, maxVolume = 5;
    [Range(minVolume, maxVolume)]
    public float masterVolume, musicVolume, effectVolume;
    public Slider masterSlider, musicSlider, effectSlider, sensi;
    public AudioMixerGroup master, music, effect;

    public System.Action<SettingsValues> OnSettingsChanged;
    public SettingsValues settingsValues;
    
    GameObject cinemachineCamera;
    CinemachineFreeLook playerCamera;

    public bool invertedX = false, invertedY = true;

    public Toggle invertXBtn, invertYBtn, dontInvertXBtn, dontInvertYBtn;
    public bool hasChanged = false;
    bool isLoading = false;


    [Range(0, 1)]
    public float sense;

    public float senseX { get { return sense * 1000 + 100; } }
    public float senseY { get { return sense * 5 + 1; } }

    public float SenseXInverse(float value) {
        return (value - 100) / 1000;
    }

    public CinemachineFreeLook PlayerCamera {
        get {
            if (Player.instance != null && playerCamera == null) {
                playerCamera = Player.instance.thirdPersonCam.GetComponent<CinemachineFreeLook>();
            }
            return playerCamera;
        }
    }

    void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void Start()
    {
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

        LoadValues();
    }

    public void RefreshUI() {
        masterSlider.value = masterVolume;
        musicSlider.value = musicVolume;
        effectSlider.value = effectVolume;
        sensi.value = sense;
        
        invertXBtn.isOn = invertedX;
        invertYBtn.isOn = invertedY;
    }


    public void ChangeMasterVolume()
    {
        masterVolume = masterSlider.value;
        master.audioMixer.SetFloat("VolumeMaster", masterVolume);

        SetDirty();
    }
    public void ChangeMusicVolume()
    {
        musicVolume = musicSlider.value;
        music.audioMixer.SetFloat("VolumeMusic", musicVolume);

        SetDirty();
    }
    public void ChangeEffectVolume()
    {
        effectVolume = effectSlider.value;
        effect.audioMixer.SetFloat("VolumeEffects", effectVolume);

        SetDirty();
    }

    public void ChangeSense()
    {
        sense = sensi.value;
        if (PlayerCamera != null) {
            PlayerCamera.m_XAxis.m_MaxSpeed = senseX;
            PlayerCamera.m_YAxis.m_MaxSpeed = senseY;
        }

        SetDirty();
    }

    public float NormalizeValues(float value)
    {
        return (value - minVolume) / (maxVolume - minVolume);
    }

    public float DenormalizeValues(float value)
    {
        return value * (maxVolume - minVolume) + minVolume;
    }

    public void SaveValues()
    {
        if (!hasChanged || isLoading) return;
        ClearDirty();
        SaveSystem.instance.SaveSettings();
    }

    public void LoadValues()
    {
        isLoading = true;
        SaveSystem.instance.LoadSettings();
        isLoading = false;
    }

    public JSONObject Save()
    {
        SettingsValues sv = new SettingsValues();
        sv.masterVolume = NormalizeValues(masterVolume);
        sv.musicVolume = NormalizeValues(musicVolume);
        sv.effectVolume = NormalizeValues(effectVolume);
        sv.cameraInvertX = invertedX;
        sv.cameraInvertY = invertedY;
        sv.sense = sense;

        settingsValues = sv;
        OnSettingsChanged?.Invoke(sv);

        return sv.ToJson();
    }

    public void Load(JSONObject json)
    {
        SettingsValues loadValues = new SettingsValues(json);
        settingsValues = loadValues;

        masterVolume = DenormalizeValues(loadValues.masterVolume);
        master.audioMixer.SetFloat("VolumeMaster", masterVolume);
        masterSlider.value = masterVolume;

        musicVolume = DenormalizeValues(loadValues.musicVolume);
        music.audioMixer.SetFloat("VolumeMusic", musicVolume);
        musicSlider.value = musicVolume;

        effectVolume = DenormalizeValues(loadValues.effectVolume);
        effect.audioMixer.SetFloat("VolumeEffects", effectVolume);
        effectSlider.value = effectVolume;

        invertedX = loadValues.cameraInvertX;
        invertedY = loadValues.cameraInvertY;

        sense = loadValues.sense;
        sensi.value = sense;
        LoadCam();
        
        OnSettingsChanged?.Invoke(loadValues);
    }

    public void LoadDefault()
    {
        masterVolume = masterSlider.value;
        master.audioMixer.SetFloat("VolumeMaster", masterVolume);

        musicVolume = musicSlider.value;
        music.audioMixer.SetFloat("VolumeMusic", musicVolume);

        effectVolume = effectSlider.value;
        effect.audioMixer.SetFloat("VolumeEffects", effectVolume);

        invertedX = false;
        invertedY = true;
        sense = 0.33f;

        if (PlayerCamera != null)
        {
            invertedX = PlayerCamera.m_XAxis.m_InvertInput;
            invertedY = PlayerCamera.m_YAxis.m_InvertInput;
            sense = SenseXInverse(PlayerCamera.m_XAxis.m_MaxSpeed);
        }

        RefreshUI();
        LoadCam();
    }

    public void ChangeInvertX()
    {
        bool invertValue = invertXBtn.isOn;

        if (PlayerCamera != null)
        {
            PlayerCamera.m_XAxis.m_InvertInput = invertValue;
        }


        if (invertValue) invertXBtn.isOn = true;
        else dontInvertXBtn.isOn = true;

        if (invertValue != invertedX) SetDirty();

        invertedX = invertValue;

        SaveValues();
    }

    public void ChangeInvertY()
    {
        bool invertValue = invertYBtn.isOn;

        if (PlayerCamera != null)
        {
            PlayerCamera.m_YAxis.m_InvertInput = invertValue;
        }

        if (invertValue) invertYBtn.isOn = true;
        else dontInvertYBtn.isOn = true;

        if (invertValue != invertedY) SetDirty();

        invertedY = invertValue;

        SaveValues();
    }

    public void LoadCam()
    {

        // X
        if (invertedX) invertXBtn.isOn = true;
        else dontInvertXBtn.isOn = true;

        if (PlayerCamera != null)
        {
            PlayerCamera.m_XAxis.m_InvertInput = invertedX;
            PlayerCamera.m_XAxis.m_MaxSpeed = senseX;
        }

        //Y
        if (invertedY) invertYBtn.isOn = true;
        else dontInvertYBtn.isOn = true;

        if (PlayerCamera != null)
        {
            PlayerCamera.m_YAxis.m_InvertInput = invertedY;
            PlayerCamera.m_YAxis.m_MaxSpeed = senseY;
        }
    }

    protected void SetDirty()
    {
        if (!isLoading) hasChanged = true;
    }

    protected void ClearDirty()
    {
        hasChanged = false;
    }

    void OnApplicationQuit()
    {
        SaveValues();
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

    public float senseX { get { return sense * 1000 + 100; } }
    public float senseY { get { return sense * 5 + 1; } }

    public SettingsValues()
    {
        masterVolume = 0;
        musicVolume = 0;
        effectVolume = 0;
        sense = 0;
        cameraInvertX = false;
        cameraInvertY = false;
    }

    public SettingsValues(float masterVolume, float musicVolume, float effectVolume, float sense, bool cameraInvertX, bool cameraInvertY)
    {
        this.masterVolume = masterVolume;
        this.musicVolume = musicVolume;
        this.effectVolume = effectVolume;
        this.sense = sense;
        this.cameraInvertX = cameraInvertX;
        this.cameraInvertY = cameraInvertY;
    }

    public SettingsValues(JSONObject json)
    {
        masterVolume = json.GetField("masterVolume").floatValue;
        musicVolume = json.GetField("musicVolume").floatValue;
        effectVolume = json.GetField("effectVolume").floatValue;
        sense = json.GetField("sense").floatValue;
        cameraInvertX = json.GetField("invertedX").boolValue;
        cameraInvertY = json.GetField("invertedY").boolValue;
    }

    public JSONObject ToJson()
    {
        JSONObject obj = new JSONObject();
        obj.AddField("masterVolume", masterVolume);
        obj.AddField("musicVolume", musicVolume);
        obj.AddField("effectVolume", effectVolume);
        obj.AddField("sense", sense);
        obj.AddField("invertedX", cameraInvertX);
        obj.AddField("invertedY", cameraInvertY);
        return obj;
    }
}
