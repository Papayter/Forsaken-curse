using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    public List<Sound> soundList = new List<Sound>();
    private List<AudioSource> activeEffectSources = new List<AudioSource>();
    private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();
    
    public float musicVolume;
    public float effectsVolume;
    
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (Sound sound in soundList)
            {
                if (!soundDict.ContainsKey(sound.key))
                    soundDict.Add(sound.key, sound.clip);
            }
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            PlayMusic("MainMenuTheme");
        }
    }

    public void PlayMusic(string key)
    {
        if (soundDict.TryGetValue(key, out AudioClip clip))
        {
            musicSource.volume = musicVolume;
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayEffects(string key, AudioSource source)
    {
        if (soundDict.TryGetValue(key, out AudioClip clip))
        {
            source.volume = effectsVolume;
            source.PlayOneShot(clip);
        }
    }
    
    public AudioSource PlayLoopingEffect(string key, Vector3? position)
    {
        if (soundDict.TryGetValue(key, out AudioClip clip))
        {
            GameObject obj = new GameObject("LoopingSFX" + key);

            if (position != null) 
                obj.transform.position = position.Value;

            AudioSource source = obj.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.volume = effectsVolume;
            
            source.spatialBlend = 1f;
            source.minDistance = 1f;
            source.maxDistance = 20f;
            source.rolloffMode = AudioRolloffMode.Custom;

            source.Play();
            
            activeEffectSources.Add(source);
            return source;
        }

        return null;
    }
    
    public void UpdateMusicVolume()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void UpdateEffectsVolume()
    {
        for (int i = 0; i < activeEffectSources.Count; i++)
        {
            activeEffectSources[i].volume = effectsVolume;
        }
    }

    public AudioSource GetMusicSource()
    {
        return musicSource;
    }
}

[Serializable]
public struct Sound
{
    public string key;
    public AudioClip clip;
    public bool loop;
}
