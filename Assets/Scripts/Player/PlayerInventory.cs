using System;
using System.Collections;
using SI;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    private List<Potion> potions = new List<Potion>();
    private List<int> potionQuantities = new List<int>();
    private List<Weapon> weapons = new List<Weapon>();
    private List<Armor> armors = new List<Armor>();
    private List<Item> items = new List<Item>();
    public InventorySlot[] potionSlots; 
    public InventorySlot[] weaponSlots;
    public InventorySlot[] spellSlots;
    public InventorySlot[] armorSlots;
    public InventorySlot[] mainSlots;
    
    public InventorySlot[] mainWeaponSlots;
    [SerializeField] private Transform weaponHolderL;
    [SerializeField] private Transform weaponHolderR;

    private GameObject currentWeapon;

    private PlayerStats player;

    private TextMeshProUGUI statsText;
    private TextMeshProUGUI loreText;

    private bool isCreatedListW;
    private bool isCreatedListA;
    
    [SerializeField] private GameObject statsWindow;
    [SerializeField] private GameObject loreWindow;
    
    [SerializeField] private TMP_Text potionQuantityText;

    private void Start()
    {
        player = GetComponent<PlayerStats>();
        statsText = statsWindow.GetComponentInChildren<TextMeshProUGUI>();
        loreText = loreWindow.GetComponentInChildren<TextMeshProUGUI>();
        
        if(!isCreatedListW)
            weapons = new List<Weapon>(new Weapon[weaponSlots.Length]);
        if(!isCreatedListA)
            armors = new List<Armor>(new Armor[armorSlots.Length]);
        
        if (PlayerPrefs.HasKey("HasGivenStartingWeapon") && PlayerPrefs.GetInt("HasGivenStartingWeapon") == 1)
        {
            Vector3 spawnPosition = player.transform.position + player.transform.forward * 2f; 
            spawnPosition.y += 3f;
            
            GameObject weaponToSpawn = null;

            switch (ClassManager.Instance.classIndex)
            {
                case 1: 
                    weaponToSpawn = player.Mage.startWeapon;
                    break;
                case 2: 
                    weaponToSpawn = player.Warrior.startWeapon;
                    break;
                case 3: 
                    weaponToSpawn = player.Assassin.startWeapon;
                    break;
            }

            if (weaponToSpawn != null)
            {
               Instantiate(weaponToSpawn, spawnPosition, Quaternion.identity);
            }
        }
        
        UpdatePotionQuantityText();
        if (weapons.Count > 1 && weapons[0] != null && weapons[1] != null)
            StartCoroutine(UpdateSpellIcon());
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f) 
        {
            ShiftForward();
        }
        else if (scroll < 0f) 
        {
            ShiftBackward();
        }
    }

    public void StatsWindow(int index, int itemType)
    {
        Item item;
        
        switch (itemType)
        {
            case 0:
                item = items[index];
                break;
            case 1:
                item = weapons[index];
                break;
            case 2:
                item = armors[index];
                break;
            default:
                print("Unknown item type");
                return;
        }
        statsWindow.SetActive(true);
        loreWindow.SetActive(true);
        
        switch (item)
        {
            case Weapon weapon:
                switch (weapon.weaponType)
                {
                    case WeaponType.Sword:
                        if (player.GetPlayerClasses() == PlayerClasses.Warrior)
                        {
                            statsText.text = $"{weapon.itemName}\n" +
                                             $"Damage: {weapon.damageAmount} <color=#FF0000>+{player.GetBonusDamage()}</color>";
                        }
                        else
                        {
                            statsText.text = $"{weapon.itemName}\n" +
                                             $"Damage: {weapon.damageAmount}";
                        }
                        loreText.text = $"{item.itemDescription}";
                        break;
                    case WeaponType.Dagger: 
                        if (player.GetPlayerClasses() == PlayerClasses.Assassin)
                        {
                            statsText.text = $"{weapon.itemName}\n" +
                                             $"Damage: {weapon.damageAmount} <color=#00FF00>+{player.GetBonusDamage()}</color>";
                        }
                        else
                        {
                            statsText.text = $"{weapon.itemName}\n" +
                                             $"Damage: {weapon.damageAmount}";
                        }
                        loreText.text = $"{item.itemDescription}";
                        break;
                    case WeaponType.Staff:
                        if (player.GetPlayerClasses() == PlayerClasses.Mage)
                        {
                            statsText.text = $"{weapon.itemName}\n" +
                                             $"Damage: {weapon.damageAmount} <color=#0000FF>+{player.GetBonusDamage()}</color>\n" +
                                             $"Mana cost: {weapon.manaCost}";
                        }
                        else
                        {
                            statsText.text = $"{weapon.itemName}\n" +
                                             $"Damage: {weapon.damageAmount}\n" +
                                             $"Mana cost: {weapon.manaCost}";
                        }
                        loreText.text = $"{item.itemDescription}";
                        break;
                    case WeaponType.Shield:
                        statsText.text = $"{weapon.itemName}\n" +
                                         $"Blocking: {weapon.blockingAmount * 100}%" ;
                        loreText.text = $"{item.itemDescription}";
                        break;
                }

                break;
            case Potion potion:
                statsText.text = $"{potion.itemName}\n" +
                                 $"Restore: {potion.RestoreAmount()}\n" +
                                 $"Quantity: {potion.potionInstance.quantity}";
                loreWindow.SetActive(false);
                break;
            case Armor armor:
                statsText.text = $"{armor.itemName}\n " +
                                 $"Extra Hp: {armor.extraHp} " +
                                 $"Extra Stamina: {armor.extraStamina} " +
                                 $"Extra Mana: {armor.extraMana}";
                loreWindow.SetActive(false);
                break;
            default:
                statsWindow.SetActive(false);
                loreWindow.SetActive(false);
                break;
        }
    }

    public void StatsWindowOff()
    {
        statsWindow.SetActive(false);
        loreWindow.SetActive(false);
    }

    public void CreateListWeapon()
    {
        weapons = new List<Weapon>(new Weapon[weaponSlots.Length]);
        isCreatedListW = true;
    }

    public void CreateListArmor()
    {
        armors = new List<Armor>(new Armor[armorSlots.Length]);
    }
    
    public List<Item> GetItems()
    {
        return items;
    }

    public async void SetItems(List<Item> loadedItems)
    {
        items = loadedItems;

        await Task.Delay(100);

        UpdateMainInventoryUI();
    }
    
    private void ShiftForward()
    {
        if (potions.Count > 1)
        {
            Potion first = potions[0];
            potions.RemoveAt(0);
            potions.Add(first);
            UpdatePotionInventoryUI();
            UpdatePotionQuantityText();
        }
    }

    private void ShiftBackward()
    {
        if (potions.Count > 1)
        {
            Potion last = potions[^1];
            potions.RemoveAt(potions.Count - 1);
            potions.Insert(0, last);
            UpdatePotionInventoryUI();
            UpdatePotionQuantityText();
        }
    }

    public void AddPotion(Potion potion)
    {
        Potion existingPotion = potions.FirstOrDefault(p => p.itemName == potion.itemName);

        if (existingPotion != null)
        {
            existingPotion.potionInstance.quantity++;

            print($"{potion.itemName} added to stack. Quantity: {existingPotion.potionInstance.quantity}");
            UpdatePotionQuantityText();
            return; 
        }
        
        if (potions.Count < potionSlots.Length) 
        {
            potions.Add(potion);
            potionSlots[potions.Count - 1].AddItem(potion.itemIcon); 
            UpdatePotionQuantityText();
            print($"{potion.itemName} added to inventory");
        }
        else if (potions.Count == potionSlots.Length)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].itemName == potion.itemName)
                {
                    potion.potionInstance.quantity++; 
                    print($"{potion.itemName} added to stack in main inventory. Quantity: {potion.potionInstance.quantity}");
                    return;
                }
            }
            
            int nullIndex = items.IndexOf(null);

            if (nullIndex != -1)
            {
                items[nullIndex] = potion;
                mainSlots[nullIndex].AddItem(potion.itemIcon);
            }
            else
            {
                items.Add(potion);
                mainSlots[items.Count - 1].AddItem(potion.itemIcon);
            }
        }
        else
        {
            print("Inventory is full!");
        }
    }

    public void UpdatePotionQuantityText()
    {
        if(potions.Count != 0)
            potionQuantityText.text = potions[0].potionInstance.quantity.ToString();
    }

    public List<int> GetPotionQuantities()
    {
        for (int i = 0; i < potions.Count; i++)
        {
            potionQuantities.Add(potions[i].potionInstance.quantity);
        }

        for (int i = 0; i < items.Count; i++)
        {
            Potion potion = items[i] as Potion;
            if (potion != null)
            {
                potionQuantities.Add(potion.potionInstance.quantity);
            }
        }
        
        return potionQuantities;
    }
    public void SetPotionQuantities(List<int> quantities)
    {
        for (int i = 0; i < potions.Count; i++)
        {
            potions[i].potionInstance.quantity = quantities[i];
        }
        
        for (int i = 0; i < items.Count; i++)
        {
            Potion potion = items[i] as Potion;
            if (potion != null)
            {
                for (int j = 2; j < items.Count + potions.Count; j++)
                {
                    potion.potionInstance.quantity = quantities[j];
                }
            }
        }
    }
    
    
    public void MovePotionToSlot(int mainSlotIndex)
    {
        if (mainSlotIndex < items.Count && items[mainSlotIndex] is Potion potion)
        {
            if (potions.Count < potionSlots.Length)
            {
                for (int i = 0; i < potionSlots.Length; i++)
                {
                    if (i < potions.Count) continue;
                
                    potions.Add(potion);
                    potionSlots[i].AddItem(potion.itemIcon);
                    items[mainSlotIndex] = null;
                    mainSlots[mainSlotIndex].ClearSlot();
                    print($"{potion.itemName} moved to potion slot");
                    UpdatePotionQuantityText();
                    UpdatePotionInventoryUI();
                    UpdateMainInventoryUI();
                    break;
                }
            }
            else
            {
                var replacedPotion = potions[0];
                potions[0] = potion;
                potionSlots[0].AddItem(potion.itemIcon);
                
                items[mainSlotIndex] = replacedPotion;
                mainSlots[mainSlotIndex].AddItem(replacedPotion.itemIcon);
            
                print($"{potion.itemName} replaced {replacedPotion.itemName} in potion slots");
                UpdatePotionQuantityText();
                UpdatePotionInventoryUI();
                UpdateMainInventoryUI();
            }
        }
    }
    
    public void UsePotion(Potion potion)
    {
        potion.UsePotion(GetComponent<PlayerMovement>());

        if (potion.potionInstance.quantity <= 1)
        {
            potions.Remove(potion);
        }
        else
        {
            potion.potionInstance.quantity--;
        }
        
        if(potions.Count != 0)
            UpdatePotionQuantityText();
        else
            potionQuantityText.text = "";
        
        UpdatePotionInventoryUI(); 
        print($"{potion.itemName} used");
    }

    public List<Potion> GetPotions()
    {
        return potions;
    }
    
    public async void SetPotions(List<Potion> loadedPotions)
    {
        potions = loadedPotions;
        
        await Task.Delay(100);

        UpdatePotionInventoryUI();
    }

    public void AddWeapon(Weapon weapon)
    {
        bool hasNull = weapons.Any(w => w == null);
        
        if (hasNull)
        {
            int nullIndex = weapons.FindIndex(w => w == null);

            if (nullIndex != -1)
            {
                weapons[nullIndex] = weapon;
                weaponSlots[nullIndex].AddItem(weapon.itemIcon);
                print($"{weapon.itemName} added to inventory");
            
                UpdateMainWeaponSlots();
            }
        }
        else
        {
            int nullIndex = items.IndexOf(null);
            
            if (nullIndex != -1)
            {
                items[nullIndex] = weapon;
                mainSlots[nullIndex].AddItem(weapon.itemIcon);
            }
            else if(items.Count < mainSlots.Length)
            {
                items.Add(weapon);
                mainSlots[items.Count - 1].AddItem(weapon.itemIcon); 
            }
        }
    }

    
    public void MoveWeaponToMainInventory(int weaponSlotIndex)
    {
        if (weaponSlotIndex < weapons.Count && weapons[weaponSlotIndex] != null)
        {
            Weapon weapon = weapons[weaponSlotIndex];
            
            int nullIndex = items.FindIndex(item => item == null);

            if (nullIndex != -1) 
            {
                items[nullIndex] = weapon;
                mainSlots[nullIndex].AddItem(weapon.itemIcon);
            }
            else if (items.Count < mainSlots.Length)
            {
                items.Add(weapon);
                mainSlots[items.Count - 1].AddItem(weapon.itemIcon);
            }
            else
            {
                print("Main inventory is full");
                return;
            }
            weapons[weaponSlotIndex] = null;
            weaponSlots[weaponSlotIndex].ClearSlot();
            mainWeaponSlots[weaponSlotIndex].ClearSlot();
            
            RemoveWeaponModel(weaponSlotIndex);
            
            UpdateMainWeaponSlots();
            UpdateSpellSlots(weapon, false);
            UpdateWeaponInventoryUI();
            UpdateMainInventoryUI();
        }
        else
        {
            print("Weapon slot is empty!");
        }
    }


    
    public void MoveWeaponToSlot(int mainSlotIndex, bool isLeftHand)
    {
        if (mainSlotIndex < items.Count && items[mainSlotIndex] is Weapon weapon)
        {
            int targetIndex = isLeftHand ? 0 : 1;
            
            if (weapons.Count <= targetIndex && items[mainSlotIndex] != null || weapons[targetIndex] == null)
            {
                while (weapons.Count <= targetIndex)
                {
                    weapons.Add(null);
                }
                
                weapons[targetIndex] = weapon;
                weaponSlots[targetIndex].AddItem(weapon.itemIcon);
                items[mainSlotIndex] = null;
                mainSlots[mainSlotIndex].ClearSlot();

                print($"{weapon.itemName} moved to weapon slot {targetIndex}");
                
                UpdateEquippedWeaponModel(weapon, targetIndex);
                
                UpdateMainWeaponSlots();
                UpdateSpellSlots(weapon, true);
                UpdateWeaponInventoryUI();
                UpdateMainInventoryUI();
            }
            else
            {
                var replacedWeapon = weapons[targetIndex];
                weapons[targetIndex] = weapon;
                weaponSlots[targetIndex].AddItem(weapon.itemIcon);
                
                items[mainSlotIndex] = replacedWeapon;
                mainSlots[mainSlotIndex].AddItem(replacedWeapon.itemIcon);

                print($"{weapon.itemName} replaced {replacedWeapon.itemName} in weapon slot {targetIndex}");

                switch (targetIndex)
                {
                    case 0 when weapons[1] != null:
                    {
                        if(weapon.weaponType != WeaponType.Staff && weapons[1].weaponType != WeaponType.Staff)
                            ClearSpellSlots();
                        break;
                    }
                    case 1 when weapons[0] != null:
                    {
                        if(weapon.weaponType != WeaponType.Staff && weapons[0].weaponType != WeaponType.Staff)
                            ClearSpellSlots();
                        break;
                    }
                    default:
                    {
                        if(weapons[0] != null || weapons[1] != null)
                        {
                            if(weapon.weaponType != WeaponType.Staff)
                                ClearSpellSlots();
                        }
                        break;
                    }
                }
                
                
                UpdateEquippedWeaponModel(weapon, targetIndex);
                UpdateSpellSlots(weapon, true);
                UpdateMainWeaponSlots();
                UpdateWeaponInventoryUI();
                UpdateMainInventoryUI();
            }
            
        }
    }
    
    public void DropItem(int slotIndex, string slotType)
    {
        switch (slotType)
        {
            case "mainSlot":
                if (items.Count > slotIndex && items[slotIndex] != null)
                {
                    Vector3 offset = transform.forward * 0.8f;
            
                    if(items[slotIndex].itemType == ItemType.Weapon)
                        offset += transform.up * 0.5f;
            
                    Quaternion rotation = Quaternion.Euler(0f, 0f, 90f);
            
                    Instantiate(items[slotIndex].itemModel, gameObject.transform.position + offset, rotation);
                    items[slotIndex] = null;
                }
                break;
            case "weaponSlot":
                if (weapons.Count > slotIndex && weapons[slotIndex] != null)
                {
                    Vector3 offset = transform.forward * 0.8f;
            
                    offset += transform.up * 0.5f;
                    
                    Quaternion rotation = Quaternion.Euler(0f, 0f, 90f);
            
                    Instantiate(weapons[slotIndex].itemModel, gameObject.transform.position + offset, rotation);
                    weapons[slotIndex] = null;
                    
                    
                    weaponSlots[slotIndex].ClearSlot();
                    mainWeaponSlots[slotIndex].ClearSlot();
                    spellSlots[0].ClearSlot();
                    
                    RemoveWeaponModel(slotIndex);
                }
                break;
            case "armorSlot":
                if (armors.Count > slotIndex && armors[slotIndex] != null)
                {
                    Vector3 offset = transform.forward * 0.8f;
            
                    offset += transform.up * 0.5f;
                    
                    Quaternion rotation = Quaternion.Euler(0f, 0f, 90f);
            
                    Instantiate(armors[slotIndex].itemModel, gameObject.transform.position + offset, rotation);
                    armors[slotIndex] = null;
                    
                    armorSlots[slotIndex].ClearSlot();
                }
                break;
        }
        
        UpdateArmorInventoryUI();
        UpdateMainWeaponSlots();
        UpdateWeaponInventoryUI();
        UpdateMainInventoryUI();
    }
    
    public void AddArmor(Armor armor)
    {
        bool hasNull = armors.Any(a => a == null);

        if (hasNull)
        {
            int slotIndex = -1;
            for (int i = 0; i < armors.Count; i++)
            {
                if (armors[i] == null)
                {
                    slotIndex = i;
                    break;
                }
            }

            if (slotIndex != -1)
            {
                armor.IncreaseStats(player);
                armors[slotIndex] = armor;
                armorSlots[slotIndex].AddItem(armor.itemIcon);
                print($"{armor.itemName} added to armor slot {slotIndex}");

                UpdateMainWeaponSlots();
            }
        }
        else
        {
            int nullIndex = items.IndexOf(null);

            if (nullIndex != -1)
            {
                items[nullIndex] = armor;
                mainSlots[nullIndex].AddItem(armor.itemIcon);
            }
            else
            {
                items.Add(armor);
                mainSlots[items.Count - 1].AddItem(armor.itemIcon); 
            }
        }
    }
    public void MoveArmorToSlot(int mainSlotIndex)
    {
        if (mainSlotIndex < items.Count && items[mainSlotIndex] is Armor armor)
        {
            int slotIndex = -1;
            
            for (int i = 0; i < armors.Count; i++)
            {
                if (armors[i] == null)
                {
                    slotIndex = i;
                    break;
                }
            }
            
            if (armors.Count <= slotIndex && items[mainSlotIndex] != null || armors[slotIndex] == null)
            {
                
                armors[slotIndex] = armor;
                armorSlots[slotIndex].AddItem(armor.itemIcon);
                items[mainSlotIndex] = null;
                mainSlots[mainSlotIndex].ClearSlot();
                UpdateMainInventoryUI();
                armor.IncreaseStats(player);
            }
            else 
            {
                var replacedArmor = armors[slotIndex];
                replacedArmor.ReduseStats(player);
                armors[slotIndex] = armor;
                armorSlots[slotIndex].AddItem(armor.itemIcon);
                items[mainSlotIndex] = replacedArmor;
                mainSlots[mainSlotIndex].AddItem(replacedArmor.itemIcon);
                UpdateMainInventoryUI();
                armor.IncreaseStats(player);
            }
        }
        
    }
    public void MoveArmorToMainInventory(int armorSlotIndex)
    {
        if (armorSlotIndex < armors.Count && armors[armorSlotIndex] != null)
        {
            Armor armor = armors[armorSlotIndex];
            
            int nullIndex = items.FindIndex(item => item == null);

            if (nullIndex != -1) 
            {
                items[nullIndex] = armor;
                mainSlots[nullIndex].AddItem(armor.itemIcon);
            }
            else if (items.Count < mainSlots.Length)
            {
                items.Add(armor);
                mainSlots[items.Count - 1].AddItem(armor.itemIcon);
            }
            else
            {
                print("Main inventory is full");
                return;
            }
            armors[armorSlotIndex] = null;
            armorSlots[armorSlotIndex].ClearSlot();
            
            UpdateMainInventoryUI();
            armor.ReduseStats(player);
        }
        else
        {
            print("Armor slot is empty!");
        }
    }
    private int GetSlotIndexByArmorType(ArmorType armorType)
    {
        switch (armorType)
        {
            case ArmorType.Helmet:
                return 0; 
            case ArmorType.Tunic:
                return 1; 
            case ArmorType.Pants:
                return 2; 
            case ArmorType.Boots:
                return 3; 
            default:
                return -1; 
        }
    }

    public List<Armor> GetArmors()
    {
        return armors;
    }

    public async void SetArmors(List<Armor> loadedArmors)
    {
        for (int i = 0; i < loadedArmors.Count; i++)
        {
            if (loadedArmors[i] != null)
            {
                int slotIndex = GetSlotIndexByArmorType(loadedArmors[i].armorType);
                
                if (slotIndex >= 0 && slotIndex < armors.Count)
                {
                    armors[slotIndex] = loadedArmors[i];
                }
            }
        }

        await Task.Delay(100);
        UpdateArmorInventoryUI();
    }

    
    private void UpdateEquippedWeaponModel(Weapon newWeapon, int leftHand)
    {
        if(newWeapon == null) return;
        
        if (leftHand == 0)
        {
            for (int i = weaponHolderR.childCount - 1; i >= 0; i--)
            {
                Destroy(weaponHolderR.GetChild(i).gameObject);
            }
        
            if (newWeapon.weaponModel != null)
            {
                currentWeapon = Instantiate(newWeapon.weaponModel, weaponHolderR);
                currentWeapon.transform.localPosition = newWeapon.weaponModel.transform.position;
                currentWeapon.transform.localRotation = newWeapon.weaponModel.transform.rotation;
            }
        }
        else
        {
            for (int i = weaponHolderL.childCount - 1; i >= 0; i--)
            {
                Destroy(weaponHolderL.GetChild(i).gameObject);
            }
        
            if (newWeapon.weaponModelLeft != null)
            {
                currentWeapon = Instantiate(newWeapon.weaponModelLeft, weaponHolderL);
                currentWeapon.transform.localPosition = newWeapon.weaponModelLeft.transform.position;
                currentWeapon.transform.localRotation = newWeapon.weaponModelLeft.transform.rotation;
            }
        }
    }
    
    private void RemoveWeaponModel(int targetIndex)
    {
        if (targetIndex == 0)
        {
            for (int i = weaponHolderR.childCount - 1; i >= 0; i--)
            {
                Destroy(weaponHolderR.GetChild(i).gameObject);
            }
        }
        else
        {
            for (int i = weaponHolderL.childCount - 1; i >= 0; i--)
            {
                Destroy(weaponHolderL.GetChild(i).gameObject);
            }
        }
    }

    
    public bool IsWeaponSlotAvailable()
    {
        return weaponHolderR.childCount < 1 || weaponHolderL.childCount < 1;
    }

    public bool IsMainSlotAvailable()
    {
        if (items.Count >= mainSlots.Length)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public bool isPotionSlotAvailable()
    {
        if (potions.Count >= potionSlots.Length && items.Count >= mainSlots.Length)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public bool isArmorSlotAvailable()
    {
        if (armors.Count >= armorSlots.Length && items.Count >= mainSlots.Length)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    

    public List<Weapon> GetWeapons()
    {
        return weapons;
    }
    public async void SetWeapons(List<Weapon> loadedWeapons)
    {
        weapons = loadedWeapons;
        await Task.Delay(10);
        
        UpdateWeaponInventoryUI();
        UpdateMainWeaponSlots();

        for (int i = 0; i <loadedWeapons.Count; i++)
        {
            UpdateEquippedWeaponModel(loadedWeapons[i], i);
        }
        
    }
    
    public void EquipWeapon(Weapon weapon)
    {
        if (weaponHolderR.childCount >= 1)
        {
            weapon.EquipWeapon(weaponHolderL, 1);
            print($"{weapon.itemName} equipped");
        }
        else 
        {
            weapon.EquipWeapon(weaponHolderR, 0);
            print($"{weapon.itemName} equipped");
        }

        UpdateSpellSlots(weapon, true);
    }
    private void UpdatePotionInventoryUI()
    {
        for (int i = 0; i < potionSlots.Length; i++)
        {
            if (i < potions.Count)
            {
                potionSlots[i].AddItem(potions[i].itemIcon);
            }
            else
            {
                potionSlots[i].ClearSlot();
            }
        }
    }
    
    private void UpdateMainWeaponSlots()
    {
        for (int i = 0; i < mainWeaponSlots.Length; i++)
        {
            if (i < weapons.Count)
            {
                if (weapons[i] != null && weapons[i].itemIcon != null)
                {
                    mainWeaponSlots[i].AddItem(weapons[i].itemIcon);
                }
            }
            else
            {
                mainWeaponSlots[i].ClearSlot();
            }
        }
    }

    private void UpdateWeaponInventoryUI()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (i < weapons.Count)
            {
                if (weapons[i] != null && weapons[i].itemIcon != null)
                {
                    print(weapons[i]);
                    print(weapons[i].itemIcon);
                    weaponSlots[i].AddItem(weapons[i].itemIcon);
                }
            }
            else
            {
                weaponSlots[i].ClearSlot();
            }
        }
    }
    
    private void UpdateArmorInventoryUI()
    {
        for (int i = 0; i < armorSlots.Length; i++)
        {
            if (i < armors.Count)
            {
                if (armors[i] != null && armors[i].itemIcon != null)
                {
                    armorSlots[i].AddItem(armors[i].itemIcon);
                }
            }
            else
            {
                armorSlots[i].ClearSlot();
            }
        }
    }

    
    private void UpdateMainInventoryUI()
    {
        for (int i = 0; i < mainSlots.Length; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                mainSlots[i].AddItem(items[i].itemIcon);
            }
            else
            {
                mainSlots[i].ClearSlot();
            }
        }
    }

    private void UpdateSpellSlots(Weapon weapon, bool isMoveToSlot)
    {
        if (weapon.weaponType == WeaponType.Staff && isMoveToSlot)
        {
           spellSlots[0].AddItem(weapon.spellIcon); 
        }
        else if(weapon.weaponType == WeaponType.Staff && !isMoveToSlot)
        {
            spellSlots[0].ClearSlot();
        }
    }

    private void ClearSpellSlots()
    {
        spellSlots[0].ClearSlot();
    }

    private IEnumerator UpdateSpellIcon()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].weaponType == WeaponType.Staff)
            {
                spellSlots[0].AddItem(weapons[i].spellIcon); 
            }
        }
    }
}