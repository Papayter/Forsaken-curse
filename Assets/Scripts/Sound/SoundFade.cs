using UnityEngine;

public class SpatialSoundProximity : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Audio source to play sound")]
    public AudioSource audioSource;

    [Tooltip("Sound clip to play")]
    public AudioClip soundClip;

    [Header("Volume Transition Settings")]
    [Tooltip("Maximum volume when player is inside the trigger")]
    [Range(0f, 1f)]
    public float maxVolume = 0.7f;

    [Tooltip("Volume fade in speed")]
    public float fadeInSpeed = 1f;

    [Tooltip("Volume fade out speed")]
    public float fadeOutSpeed = 1f;

    [Header("Debug")]
    [Tooltip("Enable debug logs")]
    public bool debugMode = false;

    private bool isPlayerInside = false;
    private float currentVolume = 0f;

    private void Start()
    {
       
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        
        audioSource.clip = soundClip;
        audioSource.loop = true;
        audioSource.spatialBlend = 1f; // Full 3D sound
        audioSource.volume = 0f;
    }

    private void Update()
    {
        
        if (isPlayerInside)
        {
           
            currentVolume = Mathf.Lerp(currentVolume, maxVolume, Time.deltaTime * fadeInSpeed);
        }
        else
        {
            
            currentVolume = Mathf.Lerp(currentVolume, 0f, Time.deltaTime * fadeOutSpeed);
        }

      
        audioSource.volume = currentVolume;

        
        if (currentVolume > 0.01f && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (currentVolume <= 0.01f && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            
            if (debugMode)
                Debug.Log("Player entered sound trigger zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            
            if (debugMode)
                Debug.Log("Player exited sound trigger zone");
        }
    }

   
    public void SetVolumeParameters(float max, float fadeIn, float fadeOut)
    {
        maxVolume = max;
        fadeInSpeed = fadeIn;
        fadeOutSpeed = fadeOut;
    }
}