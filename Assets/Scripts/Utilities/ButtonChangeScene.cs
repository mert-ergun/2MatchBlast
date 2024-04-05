using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
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
            buttonText.text = "Level " + LevelSaver.Instance.level;
        }
    }


    public void ChangeScene(string sceneName)
    {
        if (levelSaver.level == 11) return;
        StartCoroutine(ChangeSceneCoroutine(sceneName));
    }

    private IEnumerator ChangeSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(sceneName);
    }

    public void ReplayLevel()
    {
        LevelSaver.Instance.replayLevel = true;
        StartCoroutine(ReplayLevelCoroutine());
    }

    private IEnumerator ReplayLevelCoroutine()
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMain()
    {
        StartCoroutine(ReturnToMainCoroutine());
    }

    private IEnumerator ReturnToMainCoroutine()
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene("MainScene");
    }
}
