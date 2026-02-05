using UnityEngine;
using DG.Tweening;

public class SteadyHorizontalRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 45f;

    [Tooltip("Duration for a full 360-degree rotation")]
    public float rotationDuration = 8f;

    [Tooltip("Rotation axis")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Rotation Behavior")]
    [Tooltip("Should the rotation loop continuously")]
    public bool loopRotation = true;

    [Tooltip("Ease type for rotation")]
    public Ease rotationEase = Ease.Linear;

    private Tweener rotationTween;

    private void Start()
    {
        StartRotation();
    }

    public void StartRotation()
    {
        
        StopRotation();

        
        rotationTween = transform.DORotate(
                new Vector3(0, 360f, 0), 
                rotationDuration, 
                RotateMode.WorldAxisAdd
            )
            .SetEase(rotationEase)
            .SetLoops(loopRotation ? -1 : 0);
    }

    public void StopRotation()
    {
        
        if (rotationTween != null)
        {
            rotationTween.Kill();
        }
    }

   
    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
        // Restart rotation with new speed
        StopRotation();
        StartRotation();
    }

    
    public void ToggleRotation(bool shouldRotate)
    {
        if (shouldRotate)
        {
            StartRotation();
        }
        else
        {
            StopRotation();
        }
    }

    
    private void OnDisable()
    {
        StopRotation();
    }
}