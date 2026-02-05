using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    private Transform bonfireWindow;
    private Transform levelUpWindow;
    
    private TMP_Text strengthText;
    private TMP_Text intelligenceText;
    private TMP_Text agilityText;
    private TMP_Text hpText;
    private TMP_Text manaText;
    private TMP_Text staminaText;
    private TMP_Text levelText;
    private TMP_Text skillPointsText;
    private TMP_Text moneyText;
    private TMP_Text levelPriceText;

    private Button levelUpButton;
    private readonly Button[] increaseStatButtons = new Button[3]; 
    private readonly string[] increaseStatButtonNames = { "IncreaseStrengthButton", "IncreaseIntelligenceButton", "IncreaseAgilityButton" };
    
    private CinemachineVirtualCamera virtualCamera;
    private AudioSource audioSource;
    
    public Animator anim;
    private GameObject canvas;

    private PlayerMovement playerMovement; 
    [SerializeField]private PlayerStats playerStats;
    private EnemyManager enemyManager;
    
    private Vector3 playerPosition;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        audioSource = SoundManager.Instance.PlayLoopingEffect("Bonfire", gameObject.transform.position);
        
        playerMovement = FindObjectOfType<PlayerMovement>();
        virtualCamera = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        
        canvas = GameObject.FindWithTag("Canvas");
        bonfireWindow = canvas.transform.Find("BonfireWindow");
        levelUpWindow = canvas.transform.Find("LevelUpWindow");
        
        strengthText = levelUpWindow.Find("StrengthQuantityText").GetComponent<TMP_Text>();
        intelligenceText = levelUpWindow.Find("IntelligenceQuantityText").GetComponent<TMP_Text>();
        agilityText = levelUpWindow.Find("AgilityQuantityText").GetComponent<TMP_Text>();
        
        hpText = levelUpWindow.Find("HPQuantityText").GetComponent<TMP_Text>();
        manaText = levelUpWindow.Find("ManaQuantityText").GetComponent<TMP_Text>();
        staminaText = levelUpWindow.Find("StaminaQuantityText").GetComponent<TMP_Text>();
        
        levelText = levelUpWindow.Find("LevelText").GetComponent<TMP_Text>();
        skillPointsText = levelUpWindow.Find("SkillPointsText").GetComponent<TMP_Text>();
        moneyText = levelUpWindow.Find("MoneyText").GetComponent<TMP_Text>();
        levelPriceText = levelUpWindow.Find("LevelPriceText").GetComponent<TMP_Text>();
        
        levelUpButton = levelUpWindow.Find("AcceptLevelUpButton").GetComponent<Button>();

        for (int i = 0; i < increaseStatButtonNames.Length; i++)
        {
            increaseStatButtons[i] = levelUpWindow.Find(increaseStatButtonNames[i]).GetComponent<Button>();
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            if (anim != null)
            {
                anim.SetBool("Sit", true);
            }
               
            playerMovement.isBonfire = true;

            StartCoroutine(BonfireWindowDelay());
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            playerPosition = other.gameObject.transform.position;
            
            ActivateCheckpoint(playerPosition);
         
            
            playerStats.RestoreStats(playerStats.MaxHp, playerStats.MaxStamina, playerStats.MaxMana);

            virtualCamera.enabled = false;
            UpdateStatsUI();
            ButtonsInteractable();
        }
    }

    private IEnumerator BonfireWindowDelay()
    {
        yield return new WaitForSeconds(1.5f);
        bonfireWindow.gameObject.SetActive(true);
    }

    private void ActivateCheckpoint(Vector3 playerPos)
    {
        SaveSystem.instance.SetActiveCheckpoint(playerPos);
        print("Checkpoint activated at position: " + playerPos);
        PlayerPrefs.SetInt("HasGivenStartingWeapon", 0);
    }

    private void ButtonsInteractable()
    {
        levelUpButton.interactable = playerStats.Money >= playerStats.LevelPrice;

        if (playerStats.SkillPoints > 0)
        {
            for (int i = 0; i < increaseStatButtons.Length; i++)
            {
                increaseStatButtons[i].interactable = true;
                increaseStatButtons[i].GetComponent<TMP_Text>().color = Color.yellow;
            }
        }
        else
        {
            for (int i = 0; i < increaseStatButtons.Length; i++)
            {
                increaseStatButtons[i].interactable = false;
                increaseStatButtons[i].GetComponent<TMP_Text>().color = Color.gray;
            }
        }
    }
    
    private void UpdateStatsUI()
    {
        strengthText.text = $"Strength: {playerStats.PlayerStrength}";  
        intelligenceText.text = $"Intelligence: {playerStats.PlayerIntelligence}";
        agilityText.text = $"Agility: {playerStats.PlayerAgility}";
    
        hpText.text = $"HP: {playerStats.MaxHp:F1}";  
        manaText.text = $"Mana: {playerStats.MaxMana:F1}";
        staminaText.text = $"Stamina: {playerStats.MaxStamina:F1}";
    
        levelText.text = $"Level: {playerStats.PlayerLevel}";
        skillPointsText.text = $"Skill points: {playerStats.SkillPoints}";
        moneyText.text = $"Money: {playerStats.Money}";  
        levelPriceText.text = $"Price: {playerStats.LevelPrice}";
    }

    public void LevelUp()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats null");
        }
        else
        {
            Debug.Log("PlayerStats not null");
        }

        playerStats.Money -= playerStats.LevelPrice;
        playerStats.PlayerLevel++;
        playerStats.SkillPoints += 5;
        playerStats.LevelPrice += 100;
        UpdateStatsUI();
        ButtonsInteractable();
    }

    public void IncreaseStrength()
    {
        playerStats.PlayerStrength++;
        playerStats.SkillPoints--;
        playerStats.IncreaseStats(StatType.Strength);
        UpdateStatsUI();
        ButtonsInteractable();
    }
    
    public void IncreaseIntelligence()
    {
        playerStats.PlayerIntelligence++;
        playerStats.SkillPoints--;
        playerStats.IncreaseStats(StatType.Intelligence);
        UpdateStatsUI();
        ButtonsInteractable();
    }
    
    public void IncreaseAgility()
    {
        playerStats.PlayerAgility++;
        playerStats.SkillPoints--;
        playerStats.IncreaseStats(StatType.Agility);
        UpdateStatsUI();
        ButtonsInteractable();
    }

    public void Leave()
    {
        ActivateCheckpoint(playerMovement.transform.position);
        
        if (anim != null)
        {         
            anim.SetBool("Sit", false);
            StartCoroutine(Get());
          
        }
        bonfireWindow.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        playerMovement.isBonfire = false;
        virtualCamera.enabled = true;
    }

    public void LevelUpWindow()
    {
        bonfireWindow.gameObject.SetActive(false);
        levelUpWindow.gameObject.SetActive(true);
    }

    private IEnumerator Get()
    {
        anim.SetBool("GetUp", true);
        yield return new WaitForSeconds(1);
        anim.SetBool("GetUp", false);
    }

    public void CloseLevelUpWindow()
    {
        bonfireWindow.gameObject.SetActive(true);
        levelUpWindow.gameObject.SetActive(false);
    }
}