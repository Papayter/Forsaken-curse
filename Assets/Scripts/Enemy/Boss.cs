using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;
using SI;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float currentHp;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float attackRange = 3.5f;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float fireBallSpeed = 1f;
    [SerializeField] private int damage = 20;
    [SerializeField] private Image hpBar;
    [SerializeField] private GameObject hpBarBack;
    [SerializeField] private GameObject fireball;
    [SerializeField] private Transform fireballSpawn;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private GameObject fogFinal;
    [SerializeField] private GameObject canvas;

    private SpiderBossState spiderBossState;
    private Animator animator;
    private NavMeshAgent agent;
    private PlayerCombat playerCombat;
    private PlayerStats playerStats;
    private AudioSource audioSource;
    private int countToDash;
    private int countToFireball;
    private int attackOption;
    private int staffLevel;
    private bool isAttacking;
    private bool isShooting;
    private bool isFirstShoot;
    private bool isDashing;
    private bool isDeath;
    private float distance;
    private float nextAttackTime;

    public bool isDealingDamage;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        playerCombat = FindAnyObjectByType<PlayerCombat>();
        playerStats = FindAnyObjectByType<PlayerStats>();

        agent.speed = speed;
        agent.updateRotation = false;

        currentHp = maxHp;

        hpBarBack.SetActive(true);
        SoundManager.Instance.PlayMusic("BossFightTheme");
    }

    private void Update()
    {
        hpBar.fillAmount = Mathf.Clamp01(currentHp / maxHp);

        if (isDeath) return;

        LookAtPlayer();

        if (isAttacking || isShooting || isDashing) return;

        distance = Vector3.Distance(transform.position, playerCombat.transform.position);

        if (countToDash < 5 && countToFireball < 8)
        {
            if (Time.time >= nextAttackTime)
            {
                spiderBossState = distance < attackRange ? SpiderBossState.Attack : SpiderBossState.Chase;
            }
            else
            {
                spiderBossState = distance < attackRange ? SpiderBossState.Idle : SpiderBossState.Chase;
            }
        }
        else if (countToFireball >= 8)
        {
            spiderBossState = SpiderBossState.Shoot;
        }
        else
        {
            spiderBossState = SpiderBossState.Dash;
        }

        switch (spiderBossState)
        {
            case SpiderBossState.Chase:
                animator.SetBool("Move", true);
                agent.SetDestination(playerCombat.transform.position);
                break;
            case SpiderBossState.Attack:
                animator.SetBool("Move", false);
                StartCoroutine(TryAttack());
                break;
            case SpiderBossState.Dash:
                StartCoroutine(Dash());
                break;
            case SpiderBossState.Idle:
                animator.SetBool("Move", false);
                animator.SetBool("Idle", true);
                agent.ResetPath();
                break;
            case SpiderBossState.Shoot:
                StartCoroutine(Shoot());
                break;
        }
    }

    private IEnumerator Shoot()
    {
        animator.SetBool("Move", false);
        animator.SetTrigger("TakeDamage");
        isShooting = true;
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < 3; i++)
        {
            GameObject fb = Instantiate(fireball, fireballSpawn.position, Quaternion.identity);
            
            SoundManager.Instance.PlayEffects("SpiderProjectile", audioSource);

            Vector3 direction = (playerCombat.transform.position - fireballSpawn.position).normalized;

            Vector3 farTargetPos = fireballSpawn.position + direction * 20f;
            farTargetPos.y += 2f;

            fb.transform.DOMove(farTargetPos, fireBallSpeed).SetEase(Ease.Linear).OnComplete(() => { Destroy(fb, 3); });

            isDealingDamage = true;
            yield return new WaitForSeconds(1f);

            if (i == 2)
            {
                isShooting = false;
                isDealingDamage = false;
                countToFireball = 0;
            }
        }
    }

    private IEnumerator Dash()
    {
        animator.SetBool("Move", false);
        animator.SetTrigger("TakeDamage");
        isDashing = true;
        yield return new WaitForSeconds(3f);

        isDealingDamage = true;
        attackOption = 1;
        animator.SetBool("Move", true);
        agent.speed = speed + 7;

        Vector3 direction = (playerCombat.transform.position - transform.position).normalized;
        

        float dashDuration = 2.5f;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            Vector3 dashTarget = playerCombat.transform.position + direction * 5f;
            agent.SetDestination(dashTarget);
            elapsed += Time.deltaTime;
            yield return null;
        }

        agent.speed = speed;
        countToDash = 0;
        isDashing = false;
    }

    private IEnumerator TryAttack()
    {
        isAttacking = true;
        isDealingDamage = true;
        agent.ResetPath();
        animator.SetBool("Move", false);

        int randomAttack = Random.Range(1, 3);
        animator.SetBool("Attack1", randomAttack == 1);
        animator.SetBool("Attack2", randomAttack == 2);
        attackOption = randomAttack;
        
        if (attackOption == 2)
        {
            StartCoroutine(SecondAttack());
        }

        countToDash++;
        
        if (currentHp <= maxHp * 0.5f)
        {
            countToFireball++;
        }


        yield return new WaitForSeconds(2f);

        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
        isAttacking = false;
        isDealingDamage = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    private IEnumerator SecondAttack()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.6f);
            isDealingDamage = true;
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = playerCombat.transform.position - gameObject.transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        targetRotation *= Quaternion.Euler(0f, 180f, 0f);

        gameObject.transform.rotation =
            Quaternion.Slerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            if (playerCombat.IsAttacking())
            {
                AttackEnemy attackEnemy = other.GetComponent<AttackEnemy>();
                int playerDamage = attackEnemy.weapon.damageAmount;
                staffLevel = attackEnemy.weapon.staffLevel;
                
                if (playerStats.GetPlayerClasses() == PlayerClasses.Warrior && attackEnemy.weapon.weaponType == WeaponType.Sword)
                {
                    playerDamage += playerStats.GetBonusDamage();
                }
                else if (playerStats.GetPlayerClasses() == PlayerClasses.Mage && attackEnemy.weapon.weaponType == WeaponType.Staff)
                {
                    playerDamage += playerStats.GetBonusDamage();
                }
                else if (playerStats.GetPlayerClasses() == PlayerClasses.Assassin && attackEnemy.weapon.weaponType == WeaponType.Dagger)
                {
                    playerDamage += playerStats.GetBonusDamage();
                }
                
                TakeDamage(playerDamage);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (playerCombat.isDealingDamage)
        {
            SoundManager.Instance.PlayEffects("SpiderTakeDamage", audioSource);
            currentHp -= damageAmount;
            StartCoroutine(PlayParticle());
            if (currentHp <= maxHp * 0.5f && !isFirstShoot)
            {
                SoundManager.Instance.PlayEffects("SpiderAngry", audioSource);
                countToFireball = 8;
                isFirstShoot = true;
            }

            print("Boss hp: " + currentHp);
            
            if(staffLevel < 1)
                playerCombat.isDealingDamage = false;
        }

        if (currentHp <= 0)
        {
            Death();
        }
    }

    private IEnumerator PlayParticle()
    {
        particle.Play();
        yield return new WaitForSeconds(0.5f);
        particle.Stop();
    }

    private void Death()
    {
        isDeath = true;
        animator.SetBool("Death", true);
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
        animator.SetBool("Move", false);
        isAttacking = false;
        hpBarBack.SetActive(false);

        StartCoroutine(StartCutscene());
        SoundManager.Instance.GetMusicSource().Stop();
    }

    private IEnumerator StartCutscene()
    {
        yield return new WaitForSeconds(1.5f);
        fogFinal.SetActive(true);
        timeline.gameObject.SetActive(true);
        timeline.Play();
        canvas.SetActive(false);
        yield return new WaitForSeconds(30f);
        StartCoroutine(ExitGame());
    }

    private IEnumerator ExitGame()
    {
        yield return new WaitForSeconds(240f);
        Application.Quit();
    }

    public int GetDamage()
    {
        return damage;
    }

    public bool IsDeath()
    {
        return isDeath;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public bool IsShooting()
    {
        return isShooting;
    }

    public int GetAttackOption()
    {
        return attackOption;
    }
}

public enum SpiderBossState
{
    Chase,
    Attack,
    Dash,
    Idle,
    Shoot
}
