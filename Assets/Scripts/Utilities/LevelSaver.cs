using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSaver : MonoBehaviour
{
    public int level = 1;

    private void Start()
    {
        if (PlayerPrefs.HasKey("level"))
        {
            level = PlayerPrefs.GetInt("level");
        }
    }

    public void SaveLevel()
    {
        PlayerPrefs.SetInt("level", level);
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
