using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateLevelButtonText();
    }

    public void UpdateLevelButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = "Level " + levelSaver.level;
        }
    }


    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeSceneCoroutine(sceneName));
    }

    private IEnumerator ChangeSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(sceneName);
    }
}
