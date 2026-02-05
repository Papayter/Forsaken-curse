using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SI
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
    public class Weapon : Item
    {
        public int damageAmount;
        public float blockingAmount;
        public float staminaCost;
        public float manaCost;
        public GameObject projectile;
        public GameObject weaponModel;
        public GameObject weaponModelLeft;
        public Sprite spellIcon;
        public int staffLevel;
        public string animationRight;
        public string animationLeft;
        public WeaponType weaponType;
        
        public void EquipWeapon(Transform handTransform, int handIndex)
        {
            if (weaponModel != null && handIndex == 0)
            {
                GameObject equippedWeapon = Instantiate(weaponModel, handTransform);
                equippedWeapon.transform.localPosition = weaponModel.transform.position;
                equippedWeapon.transform.localRotation = weaponModel.transform.rotation;
            }
            if (weaponModelLeft != null && handIndex == 1)
            {
                GameObject equippedWeapon = Instantiate(weaponModelLeft, handTransform);
                equippedWeapon.transform.localPosition = weaponModelLeft.transform.position;
                equippedWeapon.transform.localRotation = weaponModelLeft.transform.rotation;
            }
        }
    }

    public enum WeaponType
    {
        Sword,
        Shield,
        Staff,
        Dagger
    }
}
