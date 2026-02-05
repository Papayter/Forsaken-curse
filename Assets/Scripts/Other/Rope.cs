using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
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
                if (verticalInput > 0)
                {
                    playerAnimator.SetBool("Rope", true);
                    playerAnimator.SetBool("RopeDown", false);
                    playerAnimator.SetBool("Walk", false);
                    playerAnimator.SetBool("Run", false);
                    playerAnimator.SetBool("IsFalling", false);
                }
                else if (verticalInput < 0)
                {
                    playerAnimator.SetBool("Rope", false);
                    playerAnimator.SetBool("RopeDown", true);
                    playerAnimator.SetBool("Walk", false);
                    playerAnimator.SetBool("Run", false);
                    playerAnimator.SetBool("IsFalling", false);
                }

                    Vector3 climbDirection = verticalInput > 0 ? topPoint.position : bottomPoint.position;
                    Vector3 newPosition = Vector3.MoveTowards(
                    playerMovement.transform.position,
                    climbDirection,
                    climbSpeed * Time.deltaTime
                );
                characterController.Move(newPosition - playerMovement.transform.position);
                
                playerAnimator.speed = 1f;

                
            }
            else
            {
                playerAnimator.speed = 0f;

                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
       
                playerAnimator.SetBool("Rope", false);
                playerAnimator.SetBool("RopeDown", false);
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
            playerMovement.isRope = true;
            playerMovement.enabled = false;

                Vector3 ladderPosition = bottomPoint.position;
                playerMovement.transform.position = new Vector3(
                ladderPosition.x,
                playerMovement.transform.position.y,
                ladderPosition.z
            );

            playerMovement.transform.rotation = Quaternion.LookRotation(ladderFacingDirection);

            playerAnimator.SetBool("Rope", false);
            playerAnimator.SetBool("RopeDown", false);
            playerAnimator.speed = 1f;

            HideWeapons();
        }
    }

    private void EndLadderClimb()
    {
        if (isOnLadder)
        {
            isOnLadder = false;

            playerAnimator.SetBool("RopeDown", false);
            playerAnimator.SetBool("Rope", false);
            playerAnimator.SetFloat("ClimbDirection", 0);
            playerAnimator.speed = 1f;

            playerMovement.enabled = true;

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
