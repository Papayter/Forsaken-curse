using UnityEngine;
using DG.Tweening;

public class ElevatorPlatform : MonoBehaviour
{
    public float upDistance = 5f;
    public float movementDuration = 2f;
    public float waitInterval = 20f;

    private Vector3 startPosition;
    private bool isMovingUp = true;

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 100);
        startPosition = transform.position;
        StartElevatorMovement();
    }

    private void StartElevatorMovement()
    {
        Sequence elevatorSequence = DOTween.Sequence()
            .SetLoops(-1);

        // вверх
        elevatorSequence.Append(transform.DOMove(startPosition + Vector3.up * upDistance, movementDuration)
            .SetEase(Ease.InOutSine)
            .SetDelay(waitInterval));

        // вниз
        elevatorSequence.Append(transform.DOMove(startPosition, movementDuration)
            .SetEase(Ease.InOutSine)
            .SetDelay(waitInterval));
    }

    public void StopElevator()
    {
        transform.DOKill();
    }
}