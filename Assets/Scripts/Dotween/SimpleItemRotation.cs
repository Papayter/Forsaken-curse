using UnityEngine;
using DG.Tweening;

public class FloatingObjectAnimation : MonoBehaviour
{
    public float upDownDistance = 0.2f;
    public float upDownDuration = 1f;
    public float rotationDuration = 2f;

    private void Start()
    {
        AnimateObject();
    }

    private void AnimateObject()
    {
        Sequence objectSequence = DOTween.Sequence()
            .SetLoops(-1);

        // крутит
        objectSequence.Append(transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.WorldAxisAdd)
            .SetLoops(-1, LoopType.Restart));

        // вверх вниз
        objectSequence.Join(transform.DOLocalMoveY(transform.position.y + upDownDistance, upDownDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo));
    }

    public void StopAnimation()
    {
        transform.DOKill();
    }
}