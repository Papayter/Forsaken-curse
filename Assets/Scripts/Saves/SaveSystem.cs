using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using SI;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;

    private Vector3 activeCheckpoint;
    
    private PlayerStats player;
    private PlayerInventory playerInventory;
    
    private PotionInstance potionInstance;

    private string savePathInventory;
    private string savePathPotion;
    private string savePathStats; 
    private string savePathPos;

    /*private CloudSaveManager cloudSaveManager;*/
    
    private HashSet<string> collectedItemIDs = new HashSet<string>();
    private string savePathCollectedItems;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerStats>();
        playerInventory = FindAnyObjectByType<PlayerInventory>();
        
        savePathInventory = Application.persistentDataPath + "/saveInventory.json";
        
        savePathPotion = Application.persistentDataPath + "/savePotion.json";
        
        savePathPos = Application.persistentDataPath + "/savePos.json";
        
        savePathStats = Application.persistentDataPath + "/saveStats.json";
        
        savePathCollectedItems = Application.persistentDataPath + "/collectedItems.json";
        LoadCollectedItems();
        
        if (potionInstance == null)
        {
            potionInstance = new PotionInstance(); 
        }
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdateGetObject()
    {
        player = FindAnyObjectByType<PlayerStats>();
        playerInventory = FindAnyObjectByType<PlayerInventory>();
    }

    public void SetActiveCheckpoint(Vector3 checkpointPosition)
    {
        activeCheckpoint = checkpointPosition;
        SaveData();
    }

    public Vector3 GetActiveCheckpoint()
    {
        return activeCheckpoint;
    }

    private void SaveData()
    {
        string posJson = JsonUtility.ToJson(activeCheckpoint);
        File.WriteAllText(savePathPos, posJson);

        PlayerStatsData statsData = new PlayerStatsData(player.MaxHp, player.MaxStamina, player.MaxMana, 
            player.PlayerStrength, player.PlayerIntelligence, player.PlayerAgility, player.PlayerLevel, player.SkillPoints, player.LevelPrice, player.Money);
        string statsJson = JsonUtility.ToJson(statsData);
        File.WriteAllText(savePathStats, statsJson);
        
        SaveCollectedItems();
        SaveInventory();
        
        foreach (string id in collectedItemIDs)
        {
            PlayerPrefs.SetString("Collected_" + id, "true");
        }
        PlayerPrefs.Save();
    }

    public void DeleteData()
    {
        File.Delete(savePathPos);
        File.Delete(savePathPotion);
        File.Delete(savePathStats);
        File.Delete(savePathCollectedItems);
        File.Delete(savePathInventory);
    }

    private void SaveInventory()
    {
        ItemListWrapper inventoryData = new ItemListWrapper(
            playerInventory.GetPotions(),
            playerInventory.GetWeapons(),
            playerInventory.GetArmors(),
            playerInventory.GetItems()
        );
        string inventoryJson = JsonUtility.ToJson(inventoryData);
        File.WriteAllText(savePathInventory, inventoryJson);
        
        PotionQuantitiesData potionData = new PotionQuantitiesData(playerInventory.GetPotionQuantities());  
        string potionJson = JsonUtility.ToJson(potionData);
        File.WriteAllText(savePathPotion, potionJson);
    }

    public void LoadData()
    {
        UpdateGetObject();
        
        if (File.Exists(savePathPos))
        {
            string posJson = File.ReadAllText(savePathPos);
            Vector3 loadedPos = JsonUtility.FromJson<Vector3>(posJson);
            activeCheckpoint = loadedPos;
        }
        else
        {
            activeCheckpoint = Vector3.zero; 
        }

        if (File.Exists(savePathStats))
        {
            string statsJson = File.ReadAllText(savePathStats);
            PlayerStatsData loadedStats = JsonUtility.FromJson<PlayerStatsData>(statsJson);
            
            player.PlayerStrength = loadedStats.playerStrength;
            player.PlayerIntelligence = loadedStats.playerIntelligence;
            player.PlayerAgility = loadedStats.playerAgility;
    
            player.MaxHp = loadedStats.maxHp;
            player.MaxStamina = loadedStats.maxStamina;
            player.MaxMana = loadedStats.maxMana;

            player.PlayerLevel = loadedStats.playerLevel;
            player.SkillPoints = loadedStats.playerSkillPoints;
            player.LevelPrice = loadedStats.levelPrice;
            player.Money = loadedStats.playerMoney;
            
            player.RestoreStats(loadedStats.maxHp, loadedStats.maxStamina, loadedStats.maxMana);
        }
        
        LoadInventory();
    }

    private void LoadInventory()
    {
        if (File.Exists(savePathInventory))
        {
            string inventoryJson = File.ReadAllText(savePathInventory);
            ItemListWrapper loadedInventory = JsonUtility.FromJson<ItemListWrapper>(inventoryJson);

            playerInventory.SetPotions(loadedInventory.potions);
            playerInventory.CreateListWeapon();
            playerInventory.SetWeapons(loadedInventory.weapons);
            playerInventory.CreateListArmor();
            playerInventory.SetArmors(loadedInventory.armors);
            playerInventory.SetItems(loadedInventory.items);
        }
        if (File.Exists(savePathPotion))
        {
            string potionJson = File.ReadAllText(savePathPotion);
            PotionQuantitiesData loadedPotion = JsonUtility.FromJson<PotionQuantitiesData>(potionJson);
            playerInventory.SetPotionQuantities(loadedPotion.quantities);
            playerInventory.UpdatePotionQuantityText();
        }
    }
    
    private void SaveCollectedItems()
    {
        CollectedItemData data = new CollectedItemData();
        data.ids.AddRange(collectedItemIDs);

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePathCollectedItems, json);
    }

    private void LoadCollectedItems()
    {
        if (File.Exists(savePathCollectedItems))
        {
            string json = File.ReadAllText(savePathCollectedItems);
            CollectedItemData data = JsonUtility.FromJson<CollectedItemData>(json);
            collectedItemIDs = new HashSet<string>(data.ids);
        }
    }
    
    public void AddCollectedItemID(string id)
    {
        if (!collectedItemIDs.Contains(id))
        {
            collectedItemIDs.Add(id);
        }
    }
    
    public bool IsItemCollected(string id)
    {
        return collectedItemIDs.Contains(id);
    }
}