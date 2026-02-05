/*using UnityEngine;
using DG.Tweening;

public class PickupObjectAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("How high the object will rise")]
    public float riseHeight = 0.5f;
    
    [Tooltip("Duration of rise animation")]
    public float riseDuration = 0.5f;
    
    [Tooltip("Duration of descent animation")]
    public float descendDuration = 0.5f;
    
    [Tooltip("Ease type for animations")]
    public Ease riseEase = Ease.OutQuad;
    public Ease descendEase = Ease.InQuad;

    [Header("Loop Settings")]
    [Tooltip("Should the animation loop continuously")]
    public bool loopAnimation = true;

    private Vector3 originalPosition;
    private Tweener currentTween;

    private void Start()
    {
        
        originalPosition = transform.position;

       
        StartObjectAnimation();
    }

    private void StartObjectAnimation()
    {
        
        Sequence animationSequence = DOTween.Sequence();

       
        animationSequence.Append(transform.DOMove(originalPosition + Vector3.up * riseHeight, riseDuration)
            .SetEase(riseEase));

       
        animationSequence.Append(transform.DOMove(originalPosition, descendDuration)
            .SetEase(descendEase));

        
        if (loopAnimation)
        {
            animationSequence.SetLoops(-1, LoopType.Restart);
        }

        
        animationSequence.OnComplete(() => {
            CheckOriginalPosition();
        });
    }

    private void CheckOriginalPosition()
    {
        
        if (Vector3.Distance(transform.position, originalPosition) > 0.001f)
        {
            Debug.LogWarning("Object not perfectly at original position!");
            
            transform.position = originalPosition;
        }
    }

    
    public void StopAnimation()
    {
        if (currentTween != null)
        {
            currentTween.Kill();
        }
    }

    
    public void SetRiseHeight(float newHeight)
    {
        riseHeight = newHeight;
        
        StopAnimation();
        StartObjectAnimation();
    }
}*/