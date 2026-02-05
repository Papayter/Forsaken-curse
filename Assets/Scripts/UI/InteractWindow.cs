using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractWindow : MonoBehaviour
{
   [SerializeField] private GameObject interactWindow;
   
   private void OnTriggerStay(Collider other)
   {
        if (other.CompareTag("Item"))
        {
            interactWindow.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                interactWindow.SetActive(false);
            }
        }
   }

    private void OnTriggerExit(Collider other)
    {
        interactWindow.SetActive(false);
    }
}
