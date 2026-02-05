using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixanim : MonoBehaviour
{
 
    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (isAttacking && stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
        {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
            animator.CrossFade("Idle", 0.2f); 
        }

        
        HandleMovement();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        if (moveDir.magnitude > 0.1f && !isAttacking)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            animator.SetBool("Walk", !isRunning);
            animator.SetBool("Run", isRunning);
        }
        else if (!isAttacking)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
    }

   
    public void Attack()
    {
        if (!isAttacking)
        {
            animator.SetBool("isAttacking", true);
            isAttacking = true;
        }
    }
}
