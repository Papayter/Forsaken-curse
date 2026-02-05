using System.Collections;
using System.Linq.Expressions;
using NaughtyAttributes;
using SE;
using SI;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour, IDamageable
{
    private NavMeshAgent agent;
    private EnemyStates state;
    private Transform playerT;
    private float attackRange;
    private Animator anim;
    private bool died;

    public Bow Bow;

    private bool isShoting;
    private bool isAttacking;
    public bool isDealingDamage;

    private bool isPatrolStay;
    [SerializeField] private EnemyData enemy;
    [SerializeField] private float attackTime;
    [SerializeField] private GameObject[] possibleEnemyDrops = new GameObject[5];
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hp;
    [SerializeField] private Transform dropPosition;

    private float maxHp;
    public float currentHp;
    public int damage;
    private int staffLevel;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float stayTime;
    [SerializeField] private float visionRange;
    [SerializeField] private float soundRange;
    [SerializeField] private float minShootingDistance = 5f;
    [SerializeField] private float attackCooldown = 2f;
    private bool canShoot = true;

    [SerializeField] private ParticleSystem particle;

    [SerializeField] private GameObject arrowPrefab; 
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float arrowSpeed = 10f; 

    [SerializeField] private int attacksToDodge = 3; 
    [SerializeField] private float dodgeDistance = 5f; 
    [SerializeField] private float dodgeSpeed = 5f;
    
    [SerializeField] private float stunTime = 1f;

    private int countAttacksToDodge;
    
    private int currentPatrolPointIndex;
    private PlayerCombat player;
    private PlayerMovement playerMovement;
    private PlayerStats playerStats;
    
    private Vector3 dodgePosition;
    private bool isDodging;
    private AudioSource audioSource;

    private bool isStunning;
    
    private Coroutine attackingMeleeCoroutine;
    private Coroutine attackingRangeCoroutine;
    
    [SerializeField] private float moveAwayDistance = 5f;
    [SerializeField] private float moveAwaySpeed = 3f;
    private bool isMoveAway;
    
    private float distanceToPlayer;
    [SerializeField] private float timeBeforeLost = 5f; 
    private float lostTimer;
    private bool playerInAttackRange;
    private bool isEnemyRespawn;
   
    private void Start()
    {
        playerT = FindAnyObjectByType<PlayerMovement>().transform;
        playerMovement = playerT.GetComponent<PlayerMovement>();
        player = FindObjectOfType<PlayerCombat>();
        playerStats = playerT.GetComponent<PlayerStats>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        maxHp = enemy.health;
        currentHp = maxHp;
        damage = enemy.damage;
        attackRange = enemy.attackRange;

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
        SetRagdollState(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            staffLevel = 0;
            
            AttackEnemy attackEnemy = other.GetComponent<AttackEnemy>();

            if (other.GetComponent<ProjectileMagic>() != null)
            {
                staffLevel = other.GetComponent<ProjectileMagic>().GetStaff().staffLevel;
            }
           

            int damage = 0;

            if (player.IsStrongAttacking())
            {
                damage = attackEnemy.weapon.damageAmount * 2;
                print(currentHp);

                if (currentHp <= 0)
                {
                    Bow.Tension = 0;
                    Bow.MaxTension = 0;
                    Die();
                    hpBar.gameObject.SetActive(false);
                    hp.gameObject.SetActive(false);
                }
            }
            else if (player.IsAttacking())
            {
                damage = attackEnemy.weapon.damageAmount;
                
                if (playerStats.GetPlayerClasses() == PlayerClasses.Warrior && attackEnemy.weapon.weaponType == WeaponType.Sword)
                {
                    damage += playerStats.GetBonusDamage();
                }
                else if (playerStats.GetPlayerClasses() == PlayerClasses.Mage && attackEnemy.weapon.weaponType == WeaponType.Staff)
                {
                    damage += playerStats.GetBonusDamage();
                }
                else if (playerStats.GetPlayerClasses() == PlayerClasses.Assassin && attackEnemy.weapon.weaponType == WeaponType.Dagger)
                {
                    damage += playerStats.GetBonusDamage();
                }
                
                if (currentHp <= 0)
                {
                    Die();
                    hpBar.gameObject.SetActive(false);
                    hp.gameObject.SetActive(false);
                }
            }
            else
            {
                return;
            }

            if (player.isDealingDamage)
            {
                TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isStunning) return;
        
        switch (staffLevel)
        {
            case <= 1:
                currentHp -= damageAmount;
                break;
            case 2:
                currentHp -= damageAmount * 2;
                break;
            case 3:
                currentHp -= damageAmount * 3;
                break;
        }
        
        isStunning = true;
        
        
        player.isDealingDamage = false;
        
        Debug.Log(staffLevel);
        
        StartCoroutine(TakeDamageEffect());
        TakeDamageSound();
        print("Enemy hp: " + currentHp);
        
        agent.velocity = Vector3.zero; 
        agent.ResetPath();             
        agent.isStopped = true;        
        StartCoroutine(StunCoroutine());
        
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private IEnumerator TakeDamageEffect()
    {
       particle.Play();
       yield return new WaitForSeconds(0.5f);
       particle.Stop();
    }

    private void TakeDamageSound()
    {
        switch (enemy.enemyName)
        {
            case "Skeleton":
                SoundManager.Instance.PlayEffects("TakeDamage", audioSource);
                break;
            case "DarkSkeleton":
                SoundManager.Instance.PlayEffects("DarkSkeletonTakeDamage", audioSource);
                break;
        }
    }

    private IEnumerator StunCoroutine()
    {
        if (attackingMeleeCoroutine != null) 
        {
            StopCoroutine(attackingMeleeCoroutine);
        }
        if (attackingRangeCoroutine != null) 
        {
            StopCoroutine(attackingRangeCoroutine);
        }
        
        isAttacking = false;
        isDealingDamage = false;
        player.isDealingDamage = false;
        
        state = EnemyStates.Stun;
        agent.isStopped = true;
        
        agent.velocity = Vector3.zero; 
        agent.ResetPath();     
        anim.SetBool("Stun", true);
        print("Stun activate");
        yield return new WaitForSeconds(stunTime);
        print("Stun deactivate");
        anim.SetBool("Stun", false);
        agent.isStopped = false;
        isStunning = false;

        state = enemy.enemyType == EnemyType.Melee ? EnemyStates.Chasing : EnemyStates.MoveAway;
    }
    
    private void RespawnEnemy()
    {
        isStunning = false;
        anim.SetBool("Stun", false);
        
        if(spawnPoint != null)
            gameObject.transform.position = spawnPoint.position;
        
        switch (enemy.enemyType)
        {
            case EnemyType.Melee when attackingMeleeCoroutine != null:
                StopCoroutine(attackingMeleeCoroutine);
                break;
            case EnemyType.Range when attackingRangeCoroutine != null: 
                StopCoroutine(attackingRangeCoroutine);
                break;
        }
        
        currentHp = maxHp;
        died = false;
        state = EnemyStates.Patrol;
        
        particle.Stop();
        
        DisableRagdoll();
    }

    private void Die()
    {
        died = true;
        
        if (possibleEnemyDrops.Length > 0)
        {
            var randomNumber = Random.Range(0, possibleEnemyDrops.Length);
            Instantiate(possibleEnemyDrops[randomNumber], dropPosition.position, Quaternion.identity);
        }

        EnableRagdoll();
    }

    public bool isDied()
    {
        return died;
    }
    
    private void EnableRagdoll()
    {
        print("EnableRagdoll");

        if (anim != null) anim.enabled = false;

        SetRagdollState(true);

        if (agent != null)
        {
            StopAllCoroutines();
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = false;
        }

        Vector3 position = transform.position;
        position.y = Mathf.Max(position.y, 0.1f);

        transform.position = position;
    }
    
    private void DisableRagdoll()
    {
        if (anim != null) anim.enabled = true;

        SetRagdollState(false);

        if (agent != null)
        {
            agent.enabled = true;
        }

        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = true;
        }
    }
    
    private void SetRagdollState(bool state)
    {
        Rigidbody[] ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !state;
            rb.useGravity = state;

            if (state)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    

    private void Update()
    {     
        hpBar.fillAmount = Mathf.Clamp01(currentHp / maxHp);
        distanceToPlayer = Vector3.Distance(transform.position, playerT.position);
        
        if (playerMovement.isBonfire && !isEnemyRespawn)
        {
            RespawnEnemy();
            isEnemyRespawn = true;
        }
        else if(!playerMovement.isBonfire && isEnemyRespawn)
        {
            isEnemyRespawn = false;
        }
        
        if (died)
            return;
        
        if(isStunning)
            return;
        
        if(state == EnemyStates.Dodge || state == EnemyStates.Attacking || state == EnemyStates.MoveAway)
            LookAtPlayer();
        
        if (state == EnemyStates.Stun)
        {
            return;
        }
            
        if (isDodging)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.1f)
            {
                isDodging = false;
                anim.SetBool("BackWalking", false);
                state = EnemyStates.Chasing;

            }
            return;
        }

        if (enemy.enemyType == EnemyType.Range)
        {
            if (distanceToPlayer < moveAwayDistance && !isMoveAway)
            {
                isMoveAway = true;
                state = EnemyStates.MoveAway;
            }
            else if(distanceToPlayer > moveAwayDistance && playerInAttackRange)
            {
                isMoveAway = false;
                StartAttacking();
            }
            else if(distanceToPlayer > moveAwayDistance && !playerInAttackRange)
            {
                if(attackingRangeCoroutine != null)
                    StopCoroutine(attackingRangeCoroutine);
                
                CheckForPlayer();
            }
        }
        
        switch (state)
        {
            case EnemyStates.Chasing:
                Chasing();
                break;
            case EnemyStates.Attacking when !isAttacking:
                Attack();
                break;
             case EnemyStates.Dodge:
                Dodge();
                break;
            case EnemyStates.Patrol:
                Patrol();
                break;
            case EnemyStates.MoveAway:
                MoveAway();
                break;
        }
    }
    
    private void LookAtPlayer()
    {
        Vector3 direction = (playerT.position - transform.position).normalized;
        direction.y = 0; 
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); 
    }


    private void MoveAway()
    {
        anim.SetBool("BackWalking", true);
        agent.speed = moveAwaySpeed;
        Vector3 direction = (transform.position - player.transform.position).normalized;
        agent.SetDestination(transform.position + direction * 5f);
        
    }

    private void Chasing()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerT.position);
        
        /*if (enemy.enemyType == EnemyType.Range && distanceToPlayer > attackRange)
        {
            agent.SetDestination(playerT.position);
            agent.speed = 3.5f;
            anim.SetBool("Walking", true);
        }
        else if (enemy.enemyType == EnemyType.Range && distanceToPlayer <= attackRange && distanceToPlayer >= minShootingDistance)
        {
            agent.ResetPath();
            anim.SetBool("Walking", false);
            StartAttacking();
        }*/

        PlayerMovement player = playerT.GetComponent<PlayerMovement>();
        
        playerInAttackRange = distanceToPlayer <= attackRange;
        
        if (!IsPlayerVisible() && !IsPlayerSound())
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= timeBeforeLost)
            {
                state = EnemyStates.Patrol;
                return;
            }
        }
        else
        {
            lostTimer = 0f;
        }

        if (distanceToPlayer > attackRange)
        {
            agent.SetDestination(playerT.position);
            agent.speed = 3.5f;
            anim.SetBool("Walking", true);
        }
        else if (player.IsDied())
        {
            StopAttack();
        }
        else
        {
            StartAttacking();
        }
    }
    private void Patrol()
    {
        CheckForPlayer();
        
        if(patrolPoints.Length == 0)
            return;
        
        agent.speed = 1f;
            
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPointIndex].position) < 0.5f)
        {
            if(!isPatrolStay)
                StartCoroutine(PatrolStay());
        }
        else if (!isPatrolStay)
        {
            agent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
            anim.SetBool("Walking", true);
        }
    }

    private void CheckForPlayer()
    {
        if (!isAttacking)
        {
            if (IsPlayerVisible())
            {
                state = EnemyStates.Chasing;
            }
            else if (IsPlayerSound())
            {
                state = EnemyStates.Chasing;
            }
            else
            {
                state = EnemyStates.Patrol;
            }
        }
    }

    private bool IsPlayerVisible()
    {
        float eyeHeightOffset = 1.5f; 
        Vector3 rayOrigin = transform.position + Vector3.up * eyeHeightOffset;
        
        Vector3 playerTargetPosition = playerT.position + Vector3.up * eyeHeightOffset;

        Vector3 dirToPlayer = playerTargetPosition - rayOrigin;
        
        Debug.DrawRay(rayOrigin, dirToPlayer.normalized * visionRange, Color.yellow, 1f); 

        if (dirToPlayer.magnitude > visionRange)
            return false;

        RaycastHit hit;
        
        if (Physics.Raycast(rayOrigin, dirToPlayer.normalized, out hit, visionRange)) 
        {
            if (hit.transform.CompareTag("Player") || hit.transform.GetComponentInParent<PlayerMovement>() != null)
            {
                return true;
            }
        }
        
        return false;
    }

    private bool IsPlayerSound()
    {
        return false;
    }

    private IEnumerator PatrolStay()
    {
        isPatrolStay = true;
        anim.SetBool("Walking", false);
        agent.ResetPath();
        yield return new WaitForSeconds(stayTime);
        currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
        isPatrolStay = false;
    }

    private IEnumerator AttackingRangeTime()
    {
        if (!isShoting && canShoot)
        {
            isShoting = true;
            isDealingDamage = true;
            isAttacking = true;
            canShoot = false;
            agent.ResetPath();
            anim.SetBool("Walking", false);
            anim.SetBool("BackWalking", false);
            anim.SetBool("isAiming", true);
            yield return new WaitForSeconds(1);

            anim.SetBool("isShooting", true);
            yield return new WaitForSeconds(0.5f);

           

            anim.SetBool("isAiming", false);
            anim.SetBool("isShooting", false);
            isShoting = false;
            isAttacking = false;

            yield return new WaitForSeconds(attackCooldown); 
            canShoot = true;
        }
    }
    
    public void ShootArrow()
    {
        if (playerT == null || isMoveAway || !playerInAttackRange) return;
        
        SoundManager.Instance.PlayEffects("Arrow", audioSource);
        
        playerInAttackRange = distanceToPlayer <= attackRange;

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        arrow.GetComponent<Arrow>().SetEnemyManager(gameObject.GetComponent<EnemyManager>());
        
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        Vector3 targetPosition = playerT.position + Vector3.up * 2f; 
        Vector3 direction = (targetPosition - arrowSpawnPoint.position).normalized;

       
        arrow.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

       
        rb.velocity = direction * arrowSpeed;
        Destroy(arrow, 3f);
    }
    
    private IEnumerator AttackingMeleeTime()
    {
        isAttacking = true;
        isDealingDamage = true;
        countAttacksToDodge++;

       
        while (Vector3.Distance(transform.position, playerT.position) > attackRange)
        {
            anim.SetBool("Walking", true);
            agent.SetDestination(playerT.position);
            yield return null;
        }

       
        anim.SetBool("Walking", false);
        agent.ResetPath();

      
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(attackTime);

        isAttacking = false;

        if (!isDealingDamage && countAttacksToDodge >= attacksToDodge)
        {
            StartDodge();
            countAttacksToDodge = 0;
        }
        else
        {
            StartAttack();
        }
    }

    private void Attack()
    {
        switch (enemy.enemyType)
        {
            case EnemyType.Melee:
                StartCoroutine(AttackingMeleeTime());
                break;
            case EnemyType.Range:
                StartCoroutine(AttackingRangeTime());
                break;
        }
    }

    private void StartAttacking()
    {
        switch (enemy.enemyType)
        {
            case EnemyType.Melee:
                attackingMeleeCoroutine = StartCoroutine(AttackingMeleeTime());
                break;
            case EnemyType.Range:
                attackingRangeCoroutine = StartCoroutine(AttackingRangeTime());
                break;
        }
        state = EnemyStates.Attacking;
    }
    
     private void StartDodge()
     {
        state = EnemyStates.Dodge;
     }

    public void StartAttack()
    {
        state = EnemyStates.Chasing;
    }
    
    

     private void Dodge()
     {
        Vector3 dodgeDirection = transform.position - playerT.position;
        dodgeDirection.Normalize();
        dodgePosition = transform.position + dodgeDirection * dodgeDistance;
        
        agent.SetDestination(dodgePosition);
        agent.speed = dodgeSpeed;
        anim.SetBool("BackWalking", true);
        anim.SetBool("Walking", false);
        isDodging = true;
     }

    public void StopAttack()
    {
        state = EnemyStates.Patrol;
        anim.SetBool("Walking", false);
    }
    
    

    private enum EnemyStates
    {
        Patrol,
        Chasing,
        Attacking,
        Dodge,
        Stun,
        MoveAway
    }
}