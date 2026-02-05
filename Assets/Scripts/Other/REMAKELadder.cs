using UnityEngine;

public class MultiPointLadderController : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform[] midPoints;
    [SerializeField] private Transform finishPoint;
    [SerializeField] private float climbSpeed = 2f;
    [SerializeField] private Vector3 ladderFacingDirection = Vector3.back;
    [SerializeField] private AudioClip climbingSound;
    [SerializeField] private float climbingSoundVolume = 0.5f;

    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private CharacterController characterController;
    private AudioSource audioSource;
    private bool isNearLadder = false;
    private bool isOnLadder = false;
    private Transform[] allPoints;
    private int currentPointIndex = -1;

    private void Start()
    {
        // Create array of all points
        allPoints = new Transform[midPoints.Length + 2];
        allPoints[0] = startPoint;
        for (int i = 0; i < midPoints.Length; i++)
        {
            allPoints[i + 1] = midPoints[i];
        }
        allPoints[allPoints.Length - 1] = finishPoint;

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearLadder = true;
            playerMovement = other.GetComponent<PlayerMovement>();
            playerAnimator = other.GetComponent<Animator>();
            characterController = other.GetComponent<CharacterController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearLadder = false;
            EndLadderClimb();
        }
    }

    private void Update()
    {
        if (isNearLadder && Input.GetKeyDown(KeyCode.E))
        {
            StartLadderClimb();
        }

        if (isOnLadder)
        {
            float verticalInput = Input.GetAxis("Vertical");

            // Climbing logic
            if (Mathf.Abs(verticalInput) > 0.1f)
            {
                // Determine target point based on input direction
                Transform targetPoint = DetermineTargetPoint(verticalInput);

                if (targetPoint != null)
                {
                    playerAnimator.SetBool("isClimbing", true);
                    playerAnimator.SetFloat("ClimbDirection", Mathf.Sign(verticalInput));
                    playerAnimator.speed = 1f;

                    Vector3 newPosition = Vector3.MoveTowards(
                        playerMovement.transform.position,
                        targetPoint.position,
                        climbSpeed * Time.deltaTime
                    );
                    characterController.Move(newPosition - playerMovement.transform.position);

                    // Play sound only while moving
                    if (!audioSource.isPlaying)
                    {
                        audioSource.PlayOneShot(climbingSound, climbingSoundVolume);
                    }

                    // Check if target point is reached
                    if (Vector3.Distance(playerMovement.transform.position, targetPoint.position) < 0.1f)
                    {
                        UpdateCurrentPointIndex(verticalInput);
                    }
                }
            }
            else
            {
                // Idle state
                playerAnimator.speed = 0f;
                playerAnimator.SetBool("isClimbing", true);
                playerAnimator.SetFloat("ClimbDirection", 0);

                // Stop sound when not moving
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }

            // Exit ladder
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndLadderClimb();
            }
        }
    }

    private Transform DetermineTargetPoint(float verticalInput)
    {
        if (verticalInput > 0 && currentPointIndex < allPoints.Length - 1)
        {
            return allPoints[currentPointIndex + 1];
        }
        else if (verticalInput < 0 && currentPointIndex > 0)
        {
            return allPoints[currentPointIndex - 1];
        }
        return null;
    }

    private void UpdateCurrentPointIndex(float verticalInput)
    {
        if (verticalInput > 0 && currentPointIndex < allPoints.Length - 1)
        {
            currentPointIndex++;
        }
        else if (verticalInput < 0 && currentPointIndex > 0)
        {
            currentPointIndex--;
        }
    }

    private void StartLadderClimb()
    {
        if (!isOnLadder)
        {
            isOnLadder = true;
            currentPointIndex = 0;

            playerMovement.enabled = false;

            // Align player with start point
            Vector3 ladderPosition = startPoint.position;
            playerMovement.transform.position = new Vector3(
                ladderPosition.x,
                playerMovement.transform.position.y,
                ladderPosition.z
            );

            // Rotate to face ladder
            playerMovement.transform.rotation = Quaternion.LookRotation(ladderFacingDirection);

            playerAnimator.SetTrigger("Climbing");
            playerAnimator.speed = 1f;
        }
    }

    private void EndLadderClimb()
    {
        if (isOnLadder)
        {
            isOnLadder = false;

            // Enable player movement
            playerMovement.enabled = true;

            // Reset animation and sound
            playerAnimator.SetBool("isClimbing", false);
            playerAnimator.speed = 1f;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            currentPointIndex = -1;
        }
    }

    // Optional: Visualization in scene view
    private void OnDrawGizmosSelected()
    {
        if (startPoint == null || finishPoint == null) return;

        // Draw points
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPoint.position, 0.2f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(finishPoint.position, 0.2f);

        Gizmos.color = Color.yellow;
        if (midPoints != null)
        {
            foreach (Transform midPoint in midPoints)
            {
                if (midPoint != null)
                    Gizmos.DrawSphere(midPoint.position, 0.1f);
            }

            // Draw path
            Gizmos.color = Color.blue;
            Transform[] allPathPoints = new Transform[midPoints.Length + 2];
            allPathPoints[0] = startPoint;
            for (int i = 0; i < midPoints.Length; i++)
            {
                allPathPoints[i + 1] = midPoints[i];
            }
            allPathPoints[allPathPoints.Length - 1] = finishPoint;

            for (int i = 0; i < allPathPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(allPathPoints[i].position, allPathPoints[i + 1].position);
            }
        }
    }
}