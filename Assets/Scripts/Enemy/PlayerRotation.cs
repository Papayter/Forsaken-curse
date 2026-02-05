using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        float turnX = 0f;

        if (Input.GetKey(KeyCode.A)) 
        {
            turnX = -1f;
        }
        else if (Input.GetKey(KeyCode.D)) 
        {
            turnX = 1f;
        }

        animator.SetFloat("TurnX", turnX);
    }
}
