using SI;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Character Stats")]
public class CharacterStats : ScriptableObject
{
    public float strength;
    public float intelligence;
    public float agility;
    
    public float strengthIncrease;
    public float intelligenceIncrease;
    public float agilityIncrease;
    
    public float hp;
    public float stamina;
    public float mana;

    public GameObject startWeapon;
}

