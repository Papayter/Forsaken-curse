using System.Collections;
using UnityEngine;
using DG.Tweening;

public class WallSpikeTrap : MonoBehaviour
{
    public float extendDistance = 1f;
    public float animationDuration = 1f;
    public float waitInterval = 7f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
        StartSpikeTrap();
    }

    private void StartSpikeTrap()
    {
        Sequence spikeSequence = DOTween.Sequence()
            .SetLoops(-1);

        // ловушка растет
        spikeSequence.Append(transform.DOScaleZ(originalScale.z * extendDistance, animationDuration)
            .SetEase(Ease.InOutSine));

        // ждет
        spikeSequence.AppendInterval(waitInterval);

        // внутрь
        spikeSequence.Append(transform.DOScaleZ(originalScale.z, animationDuration)
            .SetEase(Ease.InOutSine));

        // ждем
        spikeSequence.AppendInterval(waitInterval);
    }

    public void StopSpikeTrap()
    {
        transform.DOKill();
    }
}