using System;
using UnityEngine;

namespace SI
{
    [CreateAssetMenu(fileName = "New Armor", menuName = "Items/Armor")]
    public class Armor : Item
    {
        public float extraHp;
        public float extraStamina;
        public float extraMana;
        public ArmorType armorType;

        public void IncreaseStats(PlayerStats player)
        {
            if (player != null)
            {
                player.MaxHp += extraHp;
                player.MaxStamina += extraStamina;
                player.MaxMana += extraMana;
            }
        }

        public void ReduseStats(PlayerStats player)
        {
            if (player != null)
            {
                player.MaxHp -= extraHp;
                player.MaxStamina -= extraStamina;
                player.MaxMana -= extraMana;
            }
        }
    }

    public enum ArmorType
    {
        Helmet,
        Tunic,
        Pants,
        Boots
    }
}