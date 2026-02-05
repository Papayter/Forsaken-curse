using System;

[Serializable]
public class PlayerStatsData
{
    public float maxHp;
    public float maxStamina;
    public float maxMana;
    
    public float playerStrength;
    public float playerIntelligence;
    public float playerAgility;

    public int playerLevel;
    public int playerSkillPoints;
    public int levelPrice;
    public float playerMoney;

    public PlayerStatsData(float hp, float stamina, float mana, float strength, float intelligence, float agility,
        int level, int skillPoints, int price, float money)
    {
        maxHp = hp;
        maxStamina = stamina;
        maxMana = mana;

        playerStrength = strength;
        playerIntelligence = intelligence;
        playerAgility = agility;

        playerLevel = level;
        playerSkillPoints = skillPoints;
        levelPrice = price;
        playerMoney = money;
    }
}