using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Scrollbar scrollbarSfx;
    [SerializeField] private Scrollbar scrollbarMusic;
    
    private float volumeMusic;
    private float volumeSfx;
    
    void Start()
    {
        resolutionDropdown.ClearOptions();
        
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        LoadSettings();
        
        scrollbarSfx.onValueChanged.AddListener(OnScrollValueChangedSfx);
        scrollbarMusic.onValueChanged.AddListener(OnScrollValueChangedMusic);
    }

    private void OnScrollValueChangedSfx(float value)
    {
        SoundManager.Instance.effectsVolume = value;
        SoundManager.Instance.UpdateEffectsVolume();
    }

    private void OnScrollValueChangedMusic(float value)
    {
        SoundManager.Instance.musicVolume = value;
        SoundManager.Instance.UpdateMusicVolume();
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution[] resolutions = Screen.resolutions;
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

   
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("resolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("qualityIndex", qualityDropdown.value);
        PlayerPrefs.SetInt("fullScreen", Screen.fullScreen ? 1 : 0);
        PlayerPrefs.SetFloat("SFX", SoundManager.Instance.effectsVolume);
        PlayerPrefs.SetFloat("Music", SoundManager.Instance.musicVolume);
        PlayerPrefs.Save();

        LoadSettings();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex");
            resolutionDropdown.RefreshShownValue();
            SetResolution(resolutionDropdown.value);
        }

        if (PlayerPrefs.HasKey("qualityIndex"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("qualityIndex");
            qualityDropdown.RefreshShownValue();
            SetQuality(qualityDropdown.value);
        }

        if (PlayerPrefs.HasKey("fullScreen"))
        {
            bool isFullScreen = PlayerPrefs.GetInt("fullScreen") == 1;
            Screen.fullScreen = isFullScreen;
        }

        if (PlayerPrefs.HasKey("SFX"))
        {
            scrollbarSfx.value = PlayerPrefs.GetFloat("SFX");
            SoundManager.Instance.effectsVolume = PlayerPrefs.GetFloat("SFX");
        }

        if (PlayerPrefs.HasKey("Music"))
        {
            scrollbarMusic.value = PlayerPrefs.GetFloat("Music");
            SoundManager.Instance.musicVolume = PlayerPrefs.GetFloat("Music");
        }
    }
    
    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
        if(SaveSystem.instance != null)
            SaveSystem.instance.DeleteData();
    }
}

