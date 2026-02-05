using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private CharacterStats warrior;
    [SerializeField] private CharacterStats mage;
    [SerializeField] private CharacterStats assassin;

    [SerializeField] private PlayerClasses playerClasses;
    
    [SerializeField] private float playerStrength;
    [SerializeField] private float playerIntelligence;
    [SerializeField] private float playerAgility;

    [SerializeField] private float playerStrengthIncrease;
    [SerializeField] private float playerIntelligenceIncrease;
    [SerializeField] private float playerAgilityIncrease;
    
    [SerializeField] private float mainStatDamageMultiplier = 1.5f;
    [SerializeField] private float mainStatPotionMultiplier = 2f;

    [SerializeField] private float maxHp;
    [SerializeField] private float currentHp;
    [SerializeField] private float maxStamina;
    [SerializeField] private float currentStamina;
    [SerializeField] private float maxMana;
    [SerializeField] private float currentMana;

    [SerializeField] private int playerLevel;
    [SerializeField] private int skillPoints;
    [SerializeField] private float money;
    [SerializeField] private int levelPrice = 100;
    
    private float baseHp;
    private float baseStamina;
    private float baseMana;


    public float PlayerStrength { get => playerStrength; set => playerStrength = value; }

    public float PlayerIntelligence { get => playerIntelligence; set => playerIntelligence = value; }

    public float PlayerAgility { get => playerAgility; set => playerAgility = value; }

    public float MaxHp { get => maxHp; set => maxHp = value; }

    public float CurrentHp { get => currentHp; set => currentHp = Mathf.Clamp(value, 0, maxHp); }

    public float MaxStamina { get => maxStamina; set => maxStamina = value; }

    public float CurrentStamina { get => currentStamina; set => currentStamina = Mathf.Clamp(value, 0, maxStamina); }

    public float MaxMana { get => maxMana; set => maxMana = value; }

    public float CurrentMana { get => currentMana; set => currentMana = Mathf.Clamp(value, 0, maxMana); }

    public int PlayerLevel { get => playerLevel; set => playerLevel = value; }
    public int SkillPoints { get => skillPoints; set => skillPoints = value; }
    public float Money { get => money; set => money = value; }
    public int LevelPrice { get => levelPrice; set => levelPrice = value; }
    public CharacterStats Mage { get => mage; set => mage = value; }
    public CharacterStats Warrior { get => warrior; set => warrior = value; }
    public CharacterStats Assassin { get => assassin; set => assassin = value; }

    private void Start()
    {
        if (ClassManager.Instance != null)
        {
            switch (ClassManager.Instance.classIndex)
            {
                case 1:
                    playerClasses = PlayerClasses.Mage;
                    break;
                case 2:
                    playerClasses = PlayerClasses.Warrior;
                    break;
                case 3:
                    playerClasses = PlayerClasses.Assassin;
                    break;
            }
        }
        
        switch (playerClasses)
        {
            case PlayerClasses.Warrior:
                ApplyStatsFromCharacter(warrior);
                break;
            case PlayerClasses.Mage:
                ApplyStatsFromCharacter(mage);
                break;
            case PlayerClasses.Assassin:
                ApplyStatsFromCharacter(assassin);
                break;
        }

        CalculateMaxStats();
        currentHp = maxHp;
        currentStamina = maxStamina;
        currentMana = maxMana;
    }

    private void ApplyStatsFromCharacter(CharacterStats stats)
    {
        playerStrength = stats.strength;
        playerIntelligence = stats.intelligence;
        playerAgility = stats.agility;

        playerStrengthIncrease = stats.strengthIncrease;
        playerIntelligenceIncrease = stats.intelligenceIncrease;
        playerAgilityIncrease = stats.agilityIncrease;
        
        baseHp = stats.hp;
        baseStamina = stats.stamina;
        baseMana = stats.mana;
    }

    private void CalculateMaxStats()
    {
        maxHp = baseHp * (1 + playerStrength * playerStrengthIncrease);
        maxStamina = baseStamina * (1 + playerAgility * playerAgilityIncrease);
        maxMana = baseMana * (1 + playerIntelligence * playerIntelligenceIncrease);
    }

    public void IncreaseStats(StatType statType)
    {
        switch (statType)
        {
            case StatType.Strength:
                maxHp = baseHp * (1 + playerStrength * playerStrengthIncrease);
                currentHp = maxHp;
                break;
            case StatType.Agility:
                maxStamina = baseStamina * (1 + playerAgility * playerAgilityIncrease);
                currentStamina = maxStamina;
                break;
            case StatType.Intelligence:
                maxMana = baseMana * (1 + playerIntelligence * playerIntelligenceIncrease);
                currentMana = maxMana;
                break;
        }
    }

    public void RestoreStats(float hp, float stamina, float mana)
    {
        CurrentHp = hp;
        CurrentStamina = stamina;
        CurrentMana = mana;
    }

    public PlayerClasses GetPlayerClasses()
    {
        return playerClasses;
    }
    
    public float GetMainStatValue()
    {
        switch (playerClasses)
        {
            case PlayerClasses.Warrior:
                return playerStrength;
            case PlayerClasses.Mage:
                return playerIntelligence;
            case PlayerClasses.Assassin:
                return playerAgility;
            default:
                return 0;
        }
    }
    
    
    public int GetBonusDamage()
    {
        return Mathf.RoundToInt(GetMainStatValue() * mainStatDamageMultiplier);
    }
    
    public int GetBonusPotion()
    {
        return Mathf.RoundToInt(GetMainStatValue() * mainStatPotionMultiplier);
    }
}

public enum PlayerClasses
{
    Warrior,
    Mage,
    Assassin
}

public enum StatType
{
    Strength,
    Agility,
    Intelligence
}
