using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Door : MonoBehaviour
{
    [SerializeField] private string doorKeyName;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 openRotation;

    public void DoorOpen()
    {
        if (string.IsNullOrEmpty(doorKeyName))
        {
            transform.DORotate(openRotation, duration);
        }
        else
        {
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

            if (playerMovement.keyNames.Count > 0)
            {
                print(playerMovement.keyNames[0]);
                for (int i = 0; i < playerMovement.keyNames.Count; i++)
                {
                    string cleanedKey = playerMovement.keyNames[i].ToLower().Replace(" ", "");
                    string cleanedDoorKey = doorKeyName.ToLower().Replace(" ", "");

                    if (cleanedKey == cleanedDoorKey)
                    {
                        transform.DORotate(openRotation, duration);
                        return; 
                    }
                }
            }
            
        }
    }
}