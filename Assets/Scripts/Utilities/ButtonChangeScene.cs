using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonChangeScene : MonoBehaviour
{
    private TextMeshProUGUI buttonText;

    [SerializeField]
    private LevelSaver levelSaver;

    private void Start()
    {
        if (this.name == "ReplayButton")
        {
            return;
        }
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateLevelButtonText();
    }

    public void UpdateLevelButtonText()
    {
        if (buttonText != null)
        {
            if (levelSaver.level == 11)
            {
                buttonText.text = "Finished";
                return;
            }
            buttonText.text = "Level " + levelSaver.level;
        }
    }


    public void ChangeScene(string sceneName)
    {
        if (levelSaver.level == 11) return;
        levelSaver.SaveLevel();
        levelSaver.SetStartedFromMainMenu(true);
        StartCoroutine(ChangeSceneCoroutine(sceneName));
    }

    private IEnumerator ChangeSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(sceneName);
    }

    public void ReplayLevel()
    {
        int levelNumber = LevelInitializer.Instance.levelData.level_number;
        StartCoroutine(ReplayLevelCoroutine(levelNumber));
    }

    private IEnumerator ReplayLevelCoroutine(int levelNumber)
    {
        yield return new WaitForSeconds(0.6f);
        levelSaver.level = levelNumber;
        levelSaver.SaveLevel();
        levelSaver.SetStartedFromMainMenu(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMain()
    {
        levelSaver.level = LevelInitializer.Instance.levelData.level_number;
        levelSaver.SaveLevel();
        levelSaver.SetFromLevel(true);
        StartCoroutine(ReturnToMainCoroutine());
    }

    private IEnumerator ReturnToMainCoroutine()
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene("MainScene");
    }
}
