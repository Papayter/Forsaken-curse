using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour 
{
    [SerializeField] private int moneyCount ;
     private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            playerStats.Money+= moneyCount;
            print(playerStats.Money);
            Destroy(gameObject);

        }

    
    }
}