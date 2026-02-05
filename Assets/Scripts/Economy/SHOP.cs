using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using SI;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    [Header("Shop Settings")]
    [SerializeField] private GameObject shopMenuUI;
    [SerializeField] private Transform shopItemContainer;

    [Header("Shop Items")]
    [SerializeField] private List<ShopItem> availableItems;

    [SerializeField] private List<TextMeshProUGUI> shopItemText;
    private PlayerInventory playerInventory;
    private bool isShopOpen;
    private bool isShopping;

    private CinemachineVirtualCamera virtualCamera;

    [Serializable]
    public class ShopItem
    {
        public Item item;
        public int price;
        public Button itemButton;
    }

    private void Start()
    {
        virtualCamera = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        
        for (int i = 0; i < shopItemText.Count; i++)
        {
            shopItemText[i].text = $"{availableItems[i].price}";
        }
        
        shopMenuUI.SetActive(false);
        
        foreach (var shopItem in availableItems)
        {
            shopItem.itemButton.onClick.AddListener(() => PurchaseItem(shopItem));
        }
    }

    private void Update()
    {
        if (isShopping && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isShopping = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shopMenuUI.SetActive(false);
            isShopping = false;
            isShopOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            virtualCamera.enabled = true;
        }
        
    }

    private void ToggleShop()
    {
        virtualCamera.enabled = false;
        
        isShopOpen = shopMenuUI.activeInHierarchy == false;
        IsShopOpen();
        
        shopMenuUI.SetActive(isShopOpen);

        if (isShopOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            virtualCamera.enabled = true;
        }
    }

    private void PurchaseItem(ShopItem shopItem)
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        PlayerInventory inventory = playerStats.GetComponent<PlayerInventory>();
        
        if (playerStats.Money >= shopItem.price)
        {
            playerStats.Money -= shopItem.price;

            
            if (shopItem.item is Weapon weapon)
            {
                inventory.AddWeapon(weapon);
                if (inventory.IsWeaponSlotAvailable())
                {
                    inventory.EquipWeapon(weapon); 
                }
            }
            else if (shopItem.item is Potion potion)
            {
                inventory.AddPotion(potion);
            }
            else if (shopItem.item is Armor armor)
            {
                inventory.AddArmor(armor);
            }

            Debug.Log($"Purchased {shopItem.item.itemName}");
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    public bool IsShopOpen()
    {
        return isShopOpen;
    }

    public bool IsShopping()
    {
        return isShopping;
    }
}