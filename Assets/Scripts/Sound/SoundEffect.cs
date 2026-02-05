using UnityEngine;

public class AudioEffect : MonoBehaviour
{
    public AudioClip audioClip;
    public float audioRadius = 10f;
    public float pitchVariance = 0.1f;
    public GameObject playerObject;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.spatialBlend = 1f; // тут аудио
        audioSource.maxDistance = audioRadius;
        audioSource.dopplerLevel = 0f;
    }

    void Update()
    {
        // рандом высота звука и нот
        audioSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);

        // проверка радиуса
        if (playerObject != null && Vector3.Distance(transform.position, playerObject.transform.position) <= audioRadius)
        {
            // играеться звук
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // стоп аудио если игрок не рядом
            audioSource.Stop();
        }
    }
}