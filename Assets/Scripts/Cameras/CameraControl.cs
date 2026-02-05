/*using UnityEngine;


public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float distance = 5.0f;
    [SerializeField] private float sensitivity = 3.0f;
    [SerializeField] private float minYAngle = -20f;
    [SerializeField] private float maxYAngle = 50f;
    [SerializeField] private float lockFinderOffset = 100f;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float distanceToEnemy = 20f;
    [SerializeField] private Vector3 lockOffset;
    [SerializeField] private float minLockDistance = 3f;
    public CameraState camState;
    public Transform currentLockedEnemy;
    public bool IsBlendedWalk;
    private float currentX = 0f;
    private float currentY = 0f;
    public PlayerMovement playermovement;
   
    private void Update()
    {
       
       
        currentX += Input.GetAxis("Mouse X") * sensitivity;
        currentY -= Input.GetAxis("Mouse Y") * sensitivity;
        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

        var height = Screen.height / 2 + lockFinderOffset;
        var width = Screen.width / 2;

        if (camState == CameraState.Death)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentLockedEnemy != null)
            {
                Unlock();
            }
            else
            {
                var ray = Camera.main.ScreenPointToRay(new Vector2(width, height));

                if (Physics.Raycast(ray, out RaycastHit hit, distanceToEnemy, enemyLayerMask))
                {
                    Lock(hit.transform);
                }
            }
        }
    }

    

    private void LateUpdate()
    {
        switch (camState)
        {
            case CameraState.Lock:
                EnemyManager enemyManager = currentLockedEnemy.GetComponent<EnemyManager>();
                if (enemyManager.isDied())
                {
                    camState = CameraState.Default;
                }
                transform.position = player.position + lockOffset;
                transform.LookAt(currentLockedEnemy.position);
                break;
            case CameraState.Death:
                break;
            default:
                Vector3 direction = new Vector3(0, 0, -distance);
                Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
                transform.position = player.position + rotation * direction;
                transform.LookAt(player.position);
                break;
        }
    }

    

    public void Lock(Transform enemy)
    {
        print(enemy);
        camState = CameraState.Lock;
        currentLockedEnemy = enemy;
        IsBlendedWalk = true;
        playermovement.SetLockState();
    }

    private void Unlock()
    {
        camState = CameraState.Default;
        currentLockedEnemy = null;
        IsBlendedWalk = false;
       playermovement.DisableLockState();
    }
}

public enum CameraState
{
    Default,
    Lock,
    Death
}*/