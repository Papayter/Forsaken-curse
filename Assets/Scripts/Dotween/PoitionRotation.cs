using UnityEngine;
using DG.Tweening;

public class BottleAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public float rotationDuration = 2f;
    public float upDownDistance = 0.2f;
    public float upDownDuration = 1f;

    private void Start()
    {
        // выставить углы
        transform.rotation = Quaternion.Euler(15, 0, 0);

        // начать анимку 
        AnimateBottle();
    }

    private void AnimateBottle()
    {
        
        
             

        // 360 градусов крутит
        transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.WorldAxisAdd)
            .SetLoops(-1, LoopType.Restart);

        // вверх вниз
        transform.DOLocalMoveY(transform.position.y + upDownDistance, upDownDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnimation()
    {
        transform.DOKill();
    }
}