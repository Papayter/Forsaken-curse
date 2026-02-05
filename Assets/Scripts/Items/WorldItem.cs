using UnityEngine;

namespace SI
{
    public class WorldItem : MonoBehaviour
    {
        private string itemID;
        private string saveKey;

        private void Awake()
        {
            saveKey = "WorldItem" + transform.position; 

            if (PlayerPrefs.HasKey(saveKey))
            {
                itemID = PlayerPrefs.GetString(saveKey);
            }
            else
            {
                itemID = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString(saveKey, itemID); 
                PlayerPrefs.Save();
            }
        }

        private void Start()
        {
            if (SaveSystem.instance.IsItemCollected(itemID) && PlayerPrefs.HasKey("Collected_" + itemID))
            {
                Destroy(gameObject);
            }
        }

        public string GetItemID()
        {
            return itemID;
        }
    }
}