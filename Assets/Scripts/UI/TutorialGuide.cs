using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialGuide : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private string customText;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = window.GetComponentInChildren<TextMeshProUGUI>();
        
    }
    

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            text.text = customText;
            window.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            window.SetActive(false);
        }
    }
}
