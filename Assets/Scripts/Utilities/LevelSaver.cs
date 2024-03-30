using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSaver : MonoBehaviour
{
    public int level;

    private void Start()
    {
        // Get scene name
        string sceneName = SceneManager.GetActiveScene().name;

        // If scene name is MainMenu
        if (sceneName == "MainMenu")
        {
            if (PlayerPrefs.HasKey("level"))
            {
                if (level < 1)
                {
                    ResetLevel();
                }
                if (PlayerPrefs.GetInt("level") == level)
                {
                    return;
                } else 
                {
                    SaveLevel();
                }
            } else
            {
                if (level < 1)
                {
                    ResetLevel();
                }
                SaveLevel();
            } 
        }
        else if (sceneName == "Gameplay")
        {
            if (IsStartedFromMainMenu())
            {
                SetStartedFromMainMenu(false);
                LoadLevel();
            } else
            {
                SaveLevel();
            }  
        } 
    }

    public void SaveLevel()
    {
        PlayerPrefs.SetInt("level", level);
    }

    private bool IsStartedFromMainMenu()
    {
        return PlayerPrefs.GetInt("fromMainMenu") == 1;
    }

    public void SetStartedFromMainMenu(bool value)
    {
        PlayerPrefs.SetInt("fromMainMenu", value ? 1 : 0);
    }

    public void LoadLevel()
    {
        level = PlayerPrefs.GetInt("level");
    }

    public void ResetLevel()
    {
        level = 1;
        SaveLevel();
    }

    public void IncreaseLevel()
    {
        level++;
        SaveLevel();
    }

}
