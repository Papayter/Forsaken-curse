using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using SI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static PotionPickup;

public class PlayerCombat : MonoBehaviour, IDamageable
{
    [SerializeField] private float attackTime;
    [SerializeField] private float strongAttackTime;
    [SerializeField] private float secondaryAttackTime;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private float stunTime = 0.5f;
    [SerializeField] private ParticleSystem takeDamageEffect;
    [SerializeField] private ParticleSystem shieldEffect;
    [SerializeField] private GameObject weaponHolderLeft;
    [SerializeField] private GameObject weaponHolderRight;
    [SerializeField] private float particleSpawnTime = 2f;
    [SerializeField] private Boss boss;
    
    
    private PlayerStats playerStats;
    private AudioSource audioSource;
    private List<Weapon> weapons;
    public List<GameObject> activeWeaponModels;
    private List<GameObject> hiddenWeapons = new List<GameObject>();
    private bool hasItemInHand = false;
    private float strongAttackHoldTime = 0.5f;
    private float currentHoldTime = 0f;
    private bool isHolding = false;
    private bool isBlocking = false;
    private bool isLeftHandNext = true;
    private bool isMainHandWeaponEquipped; 
    private bool isOffHandWeaponEquipped; 
    private bool isMainHandAttack = true;
    private bool isRightHandAttack = true;
    private int handIndex;
    private Animator anim;
    private ShopSystem shop;
    private bool isStrongAttacking = false;
    private bool isAttacking = false;
    private bool isAttackingM = false;
    private bool isMagic;
    private EnemyLockOn enemyLockOn;
    private Camera cam;
    
    public bool isDrinking = false;
    public bool isStunning;
    public bool isRightHandEquipped; 
    public bool isLeftHandEquipped;
    public bool isProjectileDestroyed;
    public Image hpBar;
    public Image manaBar;
    public EnemyManager enemyManager;
    public bool isDealingDamage;
    internal Vector3 position;
    public GameObject redpotion;
    public GameObject bluepotion;
    public GameObject greenpotion;
    
    private void Start()
    {
        shop = FindAnyObjectByType<ShopSystem>();
        cam = Camera.main;
        anim = GetComponent<Animator>();
        inventory = GetComponent<PlayerInventory>();
        audioSource = GetComponent<AudioSource>();
        playerStats = GetComponent<PlayerStats>();
        enemyLockOn = FindObjectOfType<EnemyLockOn>();
    }

    private void Update()
    {
        manaBar.fillAmount = Mathf.Clamp01(playerStats.CurrentMana / playerStats.MaxMana);
        
        if(playerMovement.isDead || playerMovement.isBonfire || boss.IsDeath()) return;
        
        if(isStunning) return;
        
        if (isProjectileDestroyed)
        {
            isAttacking = false;
            isDealingDamage = false;
            isProjectileDestroyed = false;
        }
        
        if (Input.GetMouseButtonDown(1) && !isStunning && !isAttacking && !isStrongAttacking && !isAttackingM && !playerMovement.isInventory && !playerMovement.IsMenu() && !shop.IsShopOpen())
        {
            weapons = inventory.GetWeapons();
            if (weapons[1] != null && weapons[1].staminaCost < playerStats.CurrentStamina && weapons[1].manaCost < playerStats.CurrentMana)
            {
                if (weapons[1].weaponType == WeaponType.Shield)
                {
                    print("block left");
                    ActivateBlock(weapons[1].animationLeft);
                    handIndex = 0;
                }
                else if (weapons[1].weaponType == WeaponType.Staff)
                {
                    print("Magic left");
                    anim.SetTrigger("MagiclL");
                    if (!isMagic && playerStats.CurrentMana >= weapons[1].manaCost && playerStats.CurrentStamina >= weapons[1].staminaCost)
                        StartCoroutine(MagicAttack(weapons[1].animationLeft, weapons[1], 1));
                }
                else if (weapons[1].weaponType == WeaponType.Dagger)
                {
                    print("danger right");       
                    StartCoroutine(Dagger(weapons[1].animationLeft, weapons[1]));
                }
                else
                {
                    print("attack left");
                    StartCoroutine(Attack(weapons[1].animationLeft, weapons[1]));
                }
           
            }
        }
        if (Input.GetMouseButtonDown(0) && !isStunning && !isAttacking && !isStrongAttacking && !isAttackingM && !playerMovement.isInventory && !playerMovement.IsMenu() && !shop.IsShopOpen())
        {
            weapons = inventory.GetWeapons();
            if (weapons[0] != null && weapons[0].staminaCost < playerStats.CurrentStamina && weapons[0].manaCost < playerStats.CurrentMana)
            {
                if (weapons[0].weaponType == WeaponType.Shield)
                {
                    print("block right");
                    ActivateBlock(weapons[0].animationRight);
                    handIndex = 1;
                }
                else if (weapons[0].weaponType == WeaponType.Staff)
                {
                    if (!isDrinking) 
                    { 
                        anim.SetTrigger("MagickR");
                        
                        if (!isMagic && playerStats.CurrentMana >= weapons[0].manaCost && playerStats.CurrentStamina >= weapons[0].staminaCost)
                            StartCoroutine(MagicAttack(weapons[0].animationRight, weapons[0], 0));
                    }
                }
                else if (weapons[0].weaponType == WeaponType.Dagger)
                {
                    StartCoroutine(Dagger(weapons[0].animationRight, weapons[0]));
                }
                else
                {               
                    print(weapons[0].animationRight);
                    StartCoroutine(Attack(weapons[0].animationRight, weapons[0]));
                }
            }
        }


        if (Input.GetMouseButtonUp(1) && !playerMovement.isInventory && !playerMovement.IsMenu() && !shop.IsShopOpen())
        {
            if (isBlocking && weapons[1] != null)
            {
                DeactivateBlock(weapons[1].animationLeft);
            }
        }
        if (Input.GetMouseButtonUp(0) && !playerMovement.isInventory && !playerMovement.IsMenu())
        {
            if (isBlocking && weapons[0] != null)
            {
                DeactivateBlock(weapons[0].animationRight);
            }
        }


        if (Input.GetKeyDown(KeyCode.R) && !isAttacking && !playerMovement.isInventory && !playerMovement.IsMenu() && !shop.IsShopOpen())
        {
            if (inventory.GetPotions().Count > 0 && !isDrinking) 
            {
                StartCoroutine(DrinkPotion()); 
            }
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking && !isStrongAttacking && hasItemInHand && !playerMovement.isInventory && !shop.IsShopOpen())
        {
            isHolding = true;
            currentHoldTime = 0f;
        }

        isMainHandAttack = !isMainHandAttack; 

        if (isHolding)
        {
            currentHoldTime += Time.deltaTime;
        }
    }
    

    public void TakeDamage(int damageAmount)
    {
        if (playerMovement.isDead || playerMovement.isRolling || boss.IsDeath()) return;
        
        if(!isBlocking)
            isStunning = true;
        
        if(enemyManager != null)
            enemyManager.isDealingDamage = false;

        int finalDamage = isBlocking ? BlockDamage(damageAmount, handIndex) : damageAmount;

        if (isBlocking)
        {
            int reducedDamage = damageAmount - finalDamage;
            SoundManager.Instance.PlayEffects("Shield", audioSource);
        }

        playerStats.CurrentHp -= finalDamage;
        hpBar.fillAmount = Mathf.Clamp01(playerStats.CurrentHp / playerStats.MaxHp);
        print("Hp: " + playerStats.CurrentHp);

        if (!isBlocking)
        {
            StartCoroutine(TakeDamageEffect());
            StartCoroutine(Stun());
        }
        else
        {
            StartCoroutine(ShieldEffect());
        }
        
        TakeDamageSound();
        
        

        if (playerStats.CurrentHp <= 0)
        {
            StartCoroutine(Die());
        }
    }
    
    private IEnumerator TakeDamageEffect()
    {
        takeDamageEffect.Play();
        yield return new WaitForSeconds(0.5f);
        takeDamageEffect.Stop();
    }
    
    private IEnumerator ShieldEffect()
    {
        shieldEffect.Play();
        yield return new WaitForSeconds(0.5f);
        shieldEffect.Stop();
    }

    private void TakeDamageSound()
    {
        SoundManager.Instance.PlayEffects("PlayerTakeDamage", audioSource);
    }
    
    private IEnumerator Stun()
    {
        isDealingDamage = false;
        anim.SetBool("Stun", true);
        print("Stun activate");
        yield return new WaitForSeconds(stunTime);
        print("Stun deactivate");
        anim.SetBool("Stun", false);
        isStunning = false;
    }

    private IEnumerator MagicAttack(string animation, Weapon weapon, int handIndex)
    {
        float originalSpeed = playerMovement.moveSpeed;
        playerMovement.moveSpeed = 0.5f;
        isMagic = true;

        yield return new WaitForSeconds(1);

        playerStats.CurrentStamina -= weapon.staminaCost;
        playerStats.CurrentMana -= weapon.manaCost;
        isAttacking = true;
        isDealingDamage = true;
        GameObject staff;
        anim.SetLayerWeight(anim.GetLayerIndex("MagiclL"), 1);
        if (handIndex == 1)
        {
            staff = weaponHolderLeft.transform.GetChild(0).gameObject;
        }
        else
        {
            staff = weaponHolderRight.transform.GetChild(0).gameObject;
        }

        Transform projectileSpawn = staff.transform.GetChild(0).transform;
        
        

        for (int i = 0; i < weapon.staffLevel; i++)
        {
            GameObject magicProjectile = Instantiate(weapon.projectile, projectileSpawn.position, Quaternion.identity);
            
            Vector3 targetPosition = magicProjectile.transform.position + projectileSpawn.forward * 10f;
            targetPosition.y += cam.transform.forward.y * 10f;

            if (enemyLockOn.GetEnemyLockState())
            {
                magicProjectile.transform.DOMove(enemyLockOn.GetCurrentTarget().position, 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    isAttacking = false;
                    
                    if(i == weapon.staffLevel)
                        isDealingDamage = false;
                    
                    Destroy(magicProjectile);
                    playerMovement.moveSpeed = originalSpeed;
                });
            }
            else
            {
                magicProjectile.transform.DOMove(targetPosition, 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    isAttacking = false;
                    isDealingDamage = false;
                    Destroy(magicProjectile);
                    playerMovement.moveSpeed = originalSpeed;
                });
            }
            
            SoundManager.Instance.PlayEffects("MagicProjectile", audioSource);
            
            yield return new WaitForSeconds(0.2f);
        }
        
        playerMovement.moveSpeed = 3f;
        isMagic = false;
    }

    private IEnumerator Dagger(string animation, Weapon weapon)
    {
        playerStats.CurrentStamina -= weapon.staminaCost;
        isAttacking = true;
        isDealingDamage = true;
        anim.SetTrigger(animation);
        SoundManager.Instance.PlayEffects("Sword", audioSource);
        yield return new WaitForSeconds(1);
        isDealingDamage = false;
        isAttacking = false;
    }


    private IEnumerator Attack(string animation, Weapon weapon)
    {
        playerStats.CurrentStamina -= weapon.staminaCost;
        anim.SetLayerWeight(anim.GetLayerIndex("MainHandAttack"), 1);
        isAttacking = true;
        isDealingDamage = true; 
        anim.SetTrigger(animation);
        SoundManager.Instance.PlayEffects("Sword", audioSource);
        yield return new WaitForSeconds(attackTime);
        isDealingDamage = false;
        isAttacking = false;
    }
    public void EquipWeapon(bool isRightHand, bool isLeftHand)
    {
        isRightHandEquipped = isRightHand;
        isLeftHandEquipped = isLeftHand;
    }

    public bool IsDrinking()
    {
        return isDrinking;
    }


    public void UpdateItemInHandState()
    {
        var weapons = inventory.GetWeapons();

        isRightHandEquipped = weapons.Count > 1 && weapons[1] != null; 
        isLeftHandEquipped = weapons.Count >  0 && weapons[0] != null;

        hasItemInHand = isRightHandEquipped || isLeftHandEquipped;
    }


    public bool IsAttacking()
    {
        return isAttacking;
    }

    public bool IsAttackingM()
    {
        return isAttackingM;
    }

    public bool IsStrongAttacking()
    {
        return isStrongAttacking;
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }

    private IEnumerator StrongAttack()
    {
        if (isRightHandEquipped && !isLeftHandEquipped)
        {
            anim.SetTrigger("StrongAttack");
        }
        else if (isRightHandEquipped && isLeftHandEquipped)
        {
            anim.SetTrigger("StrongAttackMirror"); 
        }

        isStrongAttacking = true;
        isDealingDamage = true;

        yield return new WaitForSeconds(strongAttackTime);

        isStrongAttacking = false;
    }

    private IEnumerator SecondaryAttack()
    {
        anim.SetTrigger("SecondaryAttack");
        isAttackingM = true;
        isDealingDamage = true;
        yield return new WaitForSeconds(secondaryAttackTime);
        isAttackingM = false;
    }

    private IEnumerator DrinkPotion()
    {
        isDrinking = true;
        HideWeapons();
        anim.SetBool("drink", true);

        if (inventory.GetPotions().Count > 0)
        {
            Potion potionToUse = inventory.GetPotions()[0];

         
            redpotion.SetActive(false);
            bluepotion.SetActive(false);
            greenpotion.SetActive(false);

            
            switch (potionToUse.GetPotionType()) 
            {
                case PotionType.Red:
                    redpotion.SetActive(true);
                    break;
                case PotionType.Blue:
                    bluepotion.SetActive(true);
                    break;
                case PotionType.Green:
                    greenpotion.SetActive(true);
                    break;
            }

            float originalSpeed = playerMovement.moveSpeed;
            playerMovement.moveSpeed *= 0.5f;

            yield return new WaitForSeconds(2f);

          
            inventory.UsePotion(potionToUse);
            
            SoundManager.Instance.PlayEffects("Potion", audioSource);

           
            redpotion.SetActive(false);
            bluepotion.SetActive(false);
            greenpotion.SetActive(false);

            playerMovement.moveSpeed = originalSpeed;
        }

        anim.SetBool("drink", false);
        isDrinking = false;
        ShowWeapons();
    }

    private void HideWeapons()
    {
        hiddenWeapons.Clear();
        foreach (GameObject weapon in activeWeaponModels)
        {
            if (weapon.activeSelf)
            {
                hiddenWeapons.Add(weapon);
                weapon.SetActive(false);
            }
        }
    }

    private void ShowWeapons()
    {
        foreach (GameObject weapon in hiddenWeapons)
        {
            weapon.SetActive(true);
        }
        hiddenWeapons.Clear();
    }

    private IEnumerator Die()
    {
        print("Player has died");
        SoundManager.Instance.GetMusicSource().Stop();
        anim.SetTrigger("death");
        deathPanel.SetActive(true);
        playerMovement.isDead = true;

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private int BlockDamage(int damage, int handIndex)
    {
        if(handIndex == 1)
            return Mathf.RoundToInt(damage * (1 - weapons[0].blockingAmount));
        else
            return Mathf.RoundToInt(damage * (1 - weapons[1].blockingAmount));
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyDamage"))
        {
            enemyManager = other.GetComponentInParent<EnemyManager>();

            if (enemyManager != null && enemyManager.IsAttacking() && enemyManager.isDealingDamage)
            {
                TakeDamage(enemyManager.damage);
            }
        }

        if (other.CompareTag("BossDamage"))
        {
            if (boss.isDealingDamage)
            {
                TakeDamage(boss.GetDamage());
                
                boss.isDealingDamage = false;

                if (other.gameObject.layer == LayerMask.NameToLayer("BossFireball"))
                {
                    Destroy(other.gameObject);
                }
            }
        }

        if (other.CompareTag("Arrow"))
        {
            enemyManager = other.GetComponent<Arrow>().enemyManager;
            
            if (enemyManager != null && enemyManager.IsAttacking() && enemyManager.isDealingDamage)
            {
                TakeDamage(enemyManager.damage);
                Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("DeathZone"))
        {
            playerStats.CurrentHp = 0;
            StartCoroutine(Die());
        }

        if (other.CompareTag("DamageZone"))
        {
            DamageZone damageZone = other.GetComponent<DamageZone>();
            damageZone.zoneActive = true;

            if (damageZone.zoneActive)
            {
                StartCoroutine(TakeIntervalDamage(damageZone));
            }
        }

        if (other.CompareTag("Shop"))
        {
            shop = other.GetComponent<ShopSystem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DamageZone"))
        {
            DamageZone damageZone = other.GetComponent<DamageZone>();
            damageZone.zoneActive = false;
        }
    }

    private IEnumerator TakeIntervalDamage(DamageZone damageZone)
    {
        while (damageZone.zoneActive)
        {
            TakeDamage(damageZone.damage);
            yield return new WaitForSeconds(damageZone.damageInterval);
            if(playerStats.CurrentHp <= 0)
                damageZone.zoneActive = false;
        }
    }
    
    private void ActivateBlock(string animation)
    {
        isBlocking = true;
        anim.SetBool(animation, true);
    }
    
    private void DeactivateBlock(string animation)
    {
        isBlocking = false;
        anim.SetBool(animation, false);
    }
}