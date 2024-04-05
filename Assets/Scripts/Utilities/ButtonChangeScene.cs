using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene transitions and updates button text based on the game level.
/// </summary>
public class ButtonChangeScene : MonoBehaviour
{
    /// <summary>
    /// The text component on the button that displays the current level or status.
    /// </summary>
    private TextMeshProUGUI buttonText;

    /// <summary>
    /// Initializes the button's functionality based on its name and context.
    /// </summary>
    private void Start()
    {
        if (this.name == "ReplayButton")
        {
            return;
        }
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateLevelButtonText();
    }

    /// <summary>
    /// Updates the button text to reflect the current level. If the game is finished, the text will indicate this.
    /// </summary>
    public void UpdateLevelButtonText()
    {
        if (buttonText != null)
        {
            if (LevelSaver.Instance.level == 11)
            {
                buttonText.text = "Finished";
                return;
            }
            buttonText.text = "Level " + LevelSaver.Instance.level;
        }
    }

    /// <summary>
    /// Triggers a scene change with a delay.
    /// </summary>
    /// <param name="sceneName">The name of the scene to transition to.</param>
    public void ChangeScene(string sceneName)
    {
        if (LevelSaver.Instance.level == 11) return;
        StartCoroutine(ChangeSceneCoroutine(sceneName));
    }

    /// <summary>
    /// Coroutine to handle the delay and transition for changing scenes.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    private IEnumerator ChangeSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Initiates the replay of the current level. The level will be reset to its initial state.
    /// </summary>
    public void ReplayLevel()
    {
        LevelSaver.Instance.replayLevel = true;
        StartCoroutine(ReplayLevelCoroutine());
    }

    /// <summary>
    /// Coroutine to handle the delay and reloading of the current scene for a replay.
    /// </summary>
    private IEnumerator ReplayLevelCoroutine()
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Returns the player to the main scene.
    /// </summary>
    public void ReturnToMain()
    {
        StartCoroutine(ReturnToMainCoroutine());
    }

    /// <summary>
    /// Coroutine to handle the delay and transition back to the main scene.
    /// </summary>
    private IEnumerator ReturnToMainCoroutine()
    {
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene("MainScene");
    }
}
