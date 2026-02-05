using UnityEngine;
using System.Collections;

public class TreeWindMovement : MonoBehaviour
{
    [Header("Wind Movement Parameters")]
    [Tooltip("Maximum horizontal shake angle")]
    public float maxHorizontalAngle = 15f;

    [Tooltip("Maximum vertical tilt angle")]
    public float maxVerticalAngle = 10f;

    [Tooltip("Movement speed")]
    public float movementSpeed = 1f;

    [Tooltip("Movement smoothness")]
    public float smoothness = 2f;

    [Header("Randomization Settings")]
    [Tooltip("Overall wind intensity")]
    [Range(0f, 1f)]
    public float windIntensity = 0.5f;

    [Tooltip("Randomness of movement pattern")]
    [Range(0f, 1f)]
    public float randomnessLevel = 0.5f;

    [Header("Branch Specific")]
    [Tooltip("If true, movement will be more subtle")]
    public bool isSmallBranch = false;

    
    private Vector3 movementPattern;
    private float movementFrequency;
    private float movementAmplitude;
    private Vector3 initialRotation;

    private void Start()
    {
        
        initialRotation = transform.localRotation.eulerAngles;

        
        GenerateMovementPattern();

        
        StartCoroutine(RandomizedWindMovement());
    }

    private void GenerateMovementPattern()
    {
        
        Random.State previousState = Random.state;
        Random.InitState(transform.GetInstanceID());

        
        float intensityMultiplier = isSmallBranch ? 0.5f : 1f;

        
        movementPattern = new Vector3(
            Random.Range(-1f, 1f),   // Horizontal movement direction
            Random.Range(-1f, 1f),   // Vertical movement direction
            Random.Range(-1f, 1f)    // Optional subtle rotation
        ).normalized;

        
        movementFrequency = Random.Range(0.5f, 2f) * (1f + randomnessLevel);
        movementAmplitude = Random.Range(0.7f, 1.3f) * intensityMultiplier * windIntensity;

        // возврат рандома
        Random.state = previousState;
    }

    private IEnumerator RandomizedWindMovement()
    {
        while (true)
        {
            // время на движение для раз синхрона
            float time = Time.time * movementFrequency;

            
            Vector3 windOffset = new Vector3(
                // хоризонтально
                Mathf.Sin(time * movementPattern.x) * maxHorizontalAngle * movementAmplitude,
                
                // вертикально
                Mathf.Cos(time * movementPattern.y) * maxVerticalAngle * movementAmplitude,
                
                0 
            );

            // для норм поворота
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation, 
                Quaternion.Euler(windOffset + initialRotation), 
                Time.deltaTime * smoothness
            );

            yield return null;
        }
    }

    
    public void ResetMovementPattern()
    {
        GenerateMovementPattern();
    }

    // метод для резкости
    public void SetWindIntensity(float intensity)
    {
        windIntensity = Mathf.Clamp01(intensity);
        GenerateMovementPattern(); 
    }

    
    public void StopWindMovement()
    {
        StopAllCoroutines();
        transform.localRotation = Quaternion.Euler(initialRotation);
    }

    
    public void ResumeWindMovement()
    {
        StartCoroutine(RandomizedWindMovement());
    }

    //возврат
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)movementPattern);
        }
    }
}