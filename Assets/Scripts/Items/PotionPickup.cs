using System;
using SI;
using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    [SerializeField] private Potion potion;

    private bool isPickedUp;
    
    private WorldItem worldItem;

    private void Start()
    {
        worldItem = GetComponent<WorldItem>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E) && !isPickedUp)
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                if (inventory.isPotionSlotAvailable())
                {
                    inventory.AddPotion(potion);
                    isPickedUp = true; 
                    
                    SaveSystem.instance.AddCollectedItemID(worldItem.GetItemID());
                    
                    Destroy(gameObject); 
                }
                else
                {
                    print("inventory is full");
                }
            }
        }
    }
}