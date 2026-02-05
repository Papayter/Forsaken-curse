using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform topPoint;
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
    private bool isClimbingAnimationPlaying = false;
    public List<GameObject> activeWeaponModels;

    private void Start()
    {
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

            if (Mathf.Abs(verticalInput) > 0.1f)
            {
                if (verticalInput > 0 && !isClimbingAnimationPlaying)
                {
                    playerAnimator.SetTrigger("Climbing");
                    isClimbingAnimationPlaying = true;
                }
                else if (verticalInput < 0 && !isClimbingAnimationPlaying)
                {
                    playerAnimator.SetTrigger("ClimbingMiror");
                    isClimbingAnimationPlaying = true;
                }

                Vector3 climbDirection = verticalInput > 0 ? topPoint.position : bottomPoint.position;

                Vector3 newPosition = Vector3.MoveTowards(
                    playerMovement.transform.position,
                    climbDirection,
                    climbSpeed * Time.deltaTime
                );
                characterController.Move(newPosition - playerMovement.transform.position);

                playerAnimator.SetBool("isClimbing", true);
                playerAnimator.SetFloat("ClimbDirection", Mathf.Sign(verticalInput));
                playerAnimator.speed = 1f;

                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(climbingSound, climbingSoundVolume);
                }
            }
            else
            {
                playerAnimator.speed = 0f;
                playerAnimator.SetBool("isClimbing", true);
                playerAnimator.SetFloat("ClimbDirection", 0);

                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }

                isClimbingAnimationPlaying = false;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndLadderClimb();
            }
        }
    }

    private void StartLadderClimb()
    {
        if (!isOnLadder)
        {
            isOnLadder = true;

            playerMovement.enabled = false;

            Vector3 ladderPosition = bottomPoint.position;
            playerMovement.transform.position = new Vector3(
                ladderPosition.x,
                playerMovement.transform.position.y,
                ladderPosition.z
            );

            playerMovement.transform.rotation = Quaternion.LookRotation(ladderFacingDirection);

            playerAnimator.SetTrigger("Climbing");
            playerAnimator.speed = 1f;

           
            HideWeapons();
        }
    }

    private void EndLadderClimb()
    {
        if (isOnLadder)
        {
            isOnLadder = false;

            playerMovement.enabled = true;

            playerAnimator.SetBool("isClimbing", false);
            playerAnimator.speed = 1f;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            isClimbingAnimationPlaying = false;

            
            ShowWeapons();
        }
    }

    private void HideWeapons()
    {
        foreach (var weapon in activeWeaponModels)
        {
            weapon.SetActive(false);
        }
    }

    private void ShowWeapons()
    {
        foreach (var weapon in activeWeaponModels)
        {
            weapon.SetActive(true); 
        }
    }
}