using SI;
using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    [SerializeField] private Armor armor;
    
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
                if (inventory.isArmorSlotAvailable())
                {
                    inventory.AddArmor(armor); 
                    
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