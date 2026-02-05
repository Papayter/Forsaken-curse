using UnityEngine;
using DG.Tweening;

public class TreeWindAnimation : MonoBehaviour
{
    public float windRotationAngle = 15f;
    public float windRotationDuration = 1f;
    public float sizeVariation = 0.1f;
    public float windInterval = 3f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
        StartWindEffect();
    }

    private void StartWindEffect()
    {
        InvokeRepeating(nameof(SimulateWindEffect), 0f, windInterval);
    }

    private void SimulateWindEffect()
    {
        Sequence windSequence = DOTween.Sequence();

        // выбор стороны поворота
        float randomRotation = Random.Range(-windRotationAngle, windRotationAngle);

        // разворот
        windSequence.Append(transform.DORotate(new Vector3(0, randomRotation, 0), windRotationDuration)
            .SetEase(Ease.InOutSine));

        // размер
        windSequence.Join(transform.DOScale(originalScale * (1 + sizeVariation), windRotationDuration / 2));

        // обратно в исход состояние
        windSequence.Append(transform.DORotate(Vector3.zero, windRotationDuration)
            .SetEase(Ease.InOutSine));
        windSequence.Join(transform.DOScale(originalScale, windRotationDuration / 2));
    }

    public void StopWindEffect()
    {
        CancelInvoke(nameof(SimulateWindEffect));
        transform.DOKill();
    }
}