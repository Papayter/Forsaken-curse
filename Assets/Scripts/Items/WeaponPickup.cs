using System;
using SI;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    
    private WorldItem worldItem;

    private void Start()
    {
        worldItem = GetComponent<WorldItem>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            PlayerInventory inventory = other.gameObject.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                if (inventory.IsMainSlotAvailable())
                {
                    inventory.AddWeapon(weapon); 
                    
                    if(inventory.IsWeaponSlotAvailable())
                        inventory.EquipWeapon(weapon); 
                    
                    SaveSystem.instance.AddCollectedItemID(worldItem.GetItemID());
                    
                    Destroy(gameObject); 
                }
                else
                {
                    print("Weapon slots is full");
                }
            }
        }
    }
}
