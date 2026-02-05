using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassManager : MonoBehaviour
{
    public static ClassManager Instance;
    public int classIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MageClass()
    {
        classIndex = 1;
        PlayerPrefs.SetInt("ChosenClass", classIndex);
        PlayerPrefs.SetInt("HasGivenStartingWeapon", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void WarriorClass()
    {
        classIndex = 2;
        PlayerPrefs.SetInt("ChosenClass", classIndex);
        PlayerPrefs.SetInt("HasGivenStartingWeapon", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void AssassinClass()
    {
        classIndex = 3;
        PlayerPrefs.SetInt("ChosenClass", classIndex);
        PlayerPrefs.SetInt("HasGivenStartingWeapon", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}