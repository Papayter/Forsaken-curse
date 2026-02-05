using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public float Tension; 
    public float MaxTension = 1f; 
    public Transform RopeTransform;
    public Vector3 RopeNearLocalPosition;
    public Vector3 RopeFarLocalPosition;
    public Animator animator; 
    public Transform bow; 
    public Vector3 aimingPosition; 
    public Vector3 aimingRotation; 
    public Vector3 defaultPosition; 
    public Vector3 defaultRotation; 

    private void Start()
    {
        RopeNearLocalPosition = RopeTransform.localPosition;
    }

    private void Update()
    {
        if (animator == null || bow == null)
            return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

       
        if (stateInfo.IsName("isAiming"))
        {
           
            if (Tension < MaxTension)
                Tension += Time.deltaTime;  

            RopeTransform.localPosition = Vector3.Lerp(RopeNearLocalPosition, RopeFarLocalPosition, Tension);
            bow.localPosition = aimingPosition;
            bow.localEulerAngles = aimingRotation;
        }
        else
        {
           
            Tension = 0;
            RopeTransform.localPosition = RopeNearLocalPosition;
            bow.localPosition = defaultPosition;
            bow.localEulerAngles = defaultRotation;
        }

       
        if (stateInfo.IsName("isShooting")) 
        {
            
            ShootArrow();
        }
    }

   
    private void ShootArrow()
    {
        Tension = 0;
    }
}
