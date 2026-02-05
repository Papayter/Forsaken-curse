using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SI
{
    public class Item : ScriptableObject
    {
        [Header("Item information")]
        public Sprite itemIcon;
        public string itemName;
        public string itemDescription;
        public GameObject itemModel;
        public ItemType itemType;
    }

    public enum ItemType
    {
        Potion,
        Weapon,
        Armor
    }
}