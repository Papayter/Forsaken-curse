using UnityEngine;

public class AdjustAudioSource : MonoBehaviour
{
    public AudioSource soundEffect;

    void Start()
    {
        if (soundEffect != null)
        {
            soundEffect.spatialBlend = 1.0f;
            soundEffect.minDistance = 10f;
            soundEffect.maxDistance = 50f;
            soundEffect.rolloffMode = AudioRolloffMode.Linear;
            soundEffect.volume = 1f;
        }
    }
}