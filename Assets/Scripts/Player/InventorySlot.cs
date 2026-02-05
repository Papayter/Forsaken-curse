using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SI;
using Unity.VisualScripting;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image icon;
    private Item item;
    private PlayerInventory playerInventory;

    private void Start()
    {
        icon = GetComponent<Image>();
        playerInventory = FindObjectOfType<PlayerInventory>(); 
    }

    public void AddItem(Sprite itemIcon)
    {
        icon.sprite = itemIcon;
        icon.color = new Color(1, 1, 1, 1);
        icon.enabled = true; 
    }

    public Image GetIcon()
    {
        return icon;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            for (int i = 0; i < playerInventory.mainSlots.Length; i++)
            {
                if (playerInventory.mainSlots[i] == this) 
                {
                    playerInventory.DropItem(i, "mainSlot");
                }
            }
            for (int i = 0; i < playerInventory.mainWeaponSlots.Length; i++)
            {
                if (playerInventory.mainWeaponSlots[i] == this) 
                {
                    playerInventory.DropItem(i, "weaponSlot");
                }
            }
            for (int i = 0; i < playerInventory.armorSlots.Length; i++)
            {
                if (playerInventory.armorSlots[i] == this) 
                {
                    playerInventory.DropItem(i, "armorSlot");
                }
            }
        }
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            for (int i = 0; i < playerInventory.mainSlots.Length; i++)
            {
                if (playerInventory.mainSlots[i] == this) 
                {
                    playerInventory.MovePotionToSlot(i);
                }
            }
            
            for (int i = 0; i < playerInventory.mainSlots.Length; i++)
            {
                if (playerInventory.mainSlots[i] == this) 
                {
                    playerInventory.MoveWeaponToSlot(i, true); 
                    playerInventory.MoveArmorToSlot(i);
                }
            }

            for (int i = 0; i < playerInventory.mainWeaponSlots.Length; i++)
            {
                if (playerInventory.mainWeaponSlots[i] == this)
                {
                    playerInventory.MoveWeaponToMainInventory(i);
                }
            }
            
            for (int i = 0; i < playerInventory.armorSlots.Length; i++)
            {
                if (playerInventory.armorSlots[i] == this)
                {
                    playerInventory.MoveArmorToMainInventory(i);
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            for (int i = 0; i < playerInventory.mainSlots.Length; i++)
            {
                if (playerInventory.mainSlots[i] == this) 
                {
                    playerInventory.MoveWeaponToSlot(i, false); 
                }
            }
            
            for (int i = 0; i < playerInventory.mainWeaponSlots.Length; i++)
            {
                if (playerInventory.mainWeaponSlots[i] == this)
                {
                    playerInventory.MoveWeaponToMainInventory(i);
                }
            }
            
            for (int i = 0; i < playerInventory.armorSlots.Length; i++)
            {
                if (playerInventory.armorSlots[i] == this)
                {
                    playerInventory.MoveArmorToMainInventory(i);
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CompareTag("MainSlot"))
        {
            var items = playerInventory.GetItems();
            for (int i = 0; i < items.Count; i++)
            {
                if (playerInventory.mainSlots[i] == this) 
                {
                    playerInventory.StatsWindow(i, 0);
                }
            }
        }

        if (CompareTag("WeaponSlot"))
        {
            var weapons = playerInventory.GetWeapons();
            for (int i = 0; i < weapons.Count; i++)
            {
                if (playerInventory.mainWeaponSlots[i] == this) 
                {
                    playerInventory.StatsWindow(i, 1);
                }
            }
        }
        
        if (CompareTag("ArmorSlot"))
        {
            var armors = playerInventory.GetArmors();
            for (int i = 0; i < armors.Count; i++)
            {
                if (playerInventory.armorSlots[i] == this) 
                {
                    playerInventory.StatsWindow(i, 2);
                }
            }
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        playerInventory.StatsWindowOff();
    }

    public void ClearSlot()
    {
        icon.sprite = null;
        icon.color = new Color(1, 1, 1, 0);
    }
}