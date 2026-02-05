using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private ButtonsType buttonsType;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject controlsMenu;

    public void Click()
    {
        switch (buttonsType)
        {
            case ButtonsType.Play:
                if (PlayerPrefs.HasKey("ChosenClass"))
                {
                    ClassManager.Instance.classIndex = PlayerPrefs.GetInt("ChosenClass");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                SoundManager.Instance.GetMusicSource().Stop();
                break;
            case ButtonsType.Controls:
                controlsMenu.SetActive(!controlsMenu.activeSelf);
                break;
            case ButtonsType.Settings:
                settingsMenu.SetActive(!settingsMenu.activeSelf);
                break;
            case ButtonsType.Exit:
                Application.Quit();
                break;
            case ButtonsType.Back:
                settingsMenu.SetActive(!settingsMenu.activeSelf);
                break;
            case ButtonsType.BackControl:
                controlsMenu.SetActive(!controlsMenu.activeSelf);
                break;
            case ButtonsType.MainMenu:
                SceneManager.LoadScene("MainMenu");
                SoundManager.Instance.PlayMusic("MainMenuTheme");
                break;
            case ButtonsType.ClassMage:
                ClassManager.Instance.MageClass();
                break;
            case ButtonsType.ClassWarrior:
                ClassManager.Instance.WarriorClass();
                break;
            case ButtonsType.ClassAssassin:
                ClassManager.Instance.AssassinClass();
                break;
        }
    }
}

public enum ButtonsType
{
    Play,
    Settings,
    Controls,
    Exit,
    Back,
    BackControl,
    MainMenu,
    ClassMage,
    ClassWarrior,
    ClassAssassin
}