using System;
using UnityEngine;
using static PlayerCombat;
using static PotionPickup;

namespace SI
{
    [CreateAssetMenu(fileName = "New Potion", menuName = "Items/Potion")]
    public class Potion : Item
    {
        [SerializeField] private int healthRestoreAmount;
        [SerializeField] private int staminaRestoreAmount;
        [SerializeField] private int manaRestoreAmount;
        public PotionInstance potionInstance;
        public GameObject potionModel;

        [SerializeField] private PotionType potionType;

        private string restore;
        internal string tag;

        private void OnEnable()
        {
            if (potionInstance == null)
            {
                potionInstance = new PotionInstance();
            }
        }

        public PotionType GetPotionType()
        {
            return potionType; 
        }

        public string RestoreAmount()
        {
            if (staminaRestoreAmount > 0)
            {
                restore = $"{staminaRestoreAmount} Stamina";
            }

            if (healthRestoreAmount > 0)
            {
                restore = $"{healthRestoreAmount} Health";
            }

            if (manaRestoreAmount > 0)
            {
                restore = $"{manaRestoreAmount} Mana";
            }
            
            return restore;
        }

        public void UsePotion(PlayerMovement player)
        {
            if (healthRestoreAmount > 0)
            {
                player.RestoreHealth(healthRestoreAmount);
            }
            if (staminaRestoreAmount > 0)
            {
                player.RestoreStamina(staminaRestoreAmount);
            }
            if (manaRestoreAmount > 0)
            {
                player.RestoreMana(manaRestoreAmount);
            }
        }
    }
    public enum PotionType
    {
        Red,
        Blue,
        Green
    }
}