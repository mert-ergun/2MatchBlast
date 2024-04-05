using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the core game logic, including game states, moves, and popups.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// Defines possible states of the game.
    /// </summary>
    public enum GameState
    {
        Idle,
        Falling,
        Finished
    }

    /// <summary>
    /// Current state of the game.
    /// </summary>
    public GameState CurrentGameState { get; private set; } = GameState.Idle;
    private bool isPopupActive = false;

    [SerializeField]
    private TextMeshProUGUI moveCountText;
    [SerializeField]
    private GameObject failPopup;
    [SerializeField]
    private GameObject winPopup;
    [SerializeField]
    private LevelSaver levelSaver;

    /// <summary>
    /// Handles tap actions on blocks within the game.
    /// </summary>
    /// <param name="block">The block that was tapped.</param>
    public void HandleBlockTap(Block block)
    {
        // Prevents block taps when there are no moves left
        if (moveCountText.text == "0")
        {
            return;
        }

        if (CurrentGameState == GameState.Idle)
        {
            StartCoroutine(GridManager.Instance.HandleBlockTap(block));
        }
    }

    /// <summary>
    /// Handles tap actions on TNT objects within the game.
    /// </summary>
    /// <param name="tnt">The TNT that was tapped.</param>
    public void HandleTNTTap(TNT tnt)
    {
        if (moveCountText.text == "0")
        {
            return;
        }

        if (CurrentGameState == GameState.Idle)
        {
            UseMove();
            GridManager.Instance.CheckForCombo(tnt);
        }
    }

    /// <summary>
    /// Updates the game state and activates popups based on game progress.
    /// </summary>
    private void Update()
    {
        if (CurrentGameState == GameState.Idle && !isPopupActive)
        {
            if (CheckGoals())
            {
                isPopupActive = true;
                CurrentGameState = GameState.Finished;
                StartCoroutine(PopupWin());
            }

            if (moveCountText.text == "0" && !CheckGoals())
            {
                isPopupActive = true;
                CurrentGameState = GameState.Finished;
                StartCoroutine(PopupFail());
            }
        }
    }

    /// <summary>
    /// Activates the failure popup and processes relevant UI changes.
    /// </summary>
    private IEnumerator PopupFail()
    {
        failPopup.SetActive(true);
        GameObject overlay = failPopup.transform.Find("Overlay").gameObject;
        overlay.SetActive(true);

        GameObject popupContent = failPopup.transform.Find("PopupContent").gameObject;
        Vector3 originalScale = popupContent.transform.localScale;
        popupContent.transform.localScale = Vector3.zero;

        GameObject popupRibbon = popupContent.transform.Find("PopupRibbon").gameObject;
        TextMeshProUGUI levelText = popupRibbon.transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        levelText.text = "Level " + LevelInitializer.Instance.levelData.level_number;

        float duration = 0.5f;
        float time = 0;

        while (time < duration)
        {
            popupContent.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        popupContent.transform.localScale = originalScale;
        LevelSaver.Instance.SetLevelFromJson();
    }

    /// <summary>
    /// Activates the win popup and transitions to the main scene after a delay.
    /// </summary>
    private IEnumerator PopupWin()
    {
        winPopup.SetActive(true);
        yield return new WaitForSeconds(5f);
        levelSaver.IncreaseLevel();
        SceneManager.LoadScene("MainScene");
    }

    /// <summary>
    /// Decreases the move count when a move is used.
    /// </summary>
    public void UseMove()
    {
        moveCountText.text = (int.Parse(moveCountText.text) - 1).ToString();
    }

    /// <summary>
    /// Initiates the block falling process for all blocks.
    /// </summary>
    public void FallBlock()
    {
        // Fall all the blocks until they reach above another block
        for (int x = GridManager.Instance.GetRow() - 1; x >= 0; x--)
        {
            for (int y = GridManager.Instance.GetColumn() - 1; y >= 0; y--)
            {
                Block block = GridManager.Instance.GetBlock(x, y);
                if (block != null)
                {
                    if (block.type == Block.BlockType.Obstacle)
                    {
                        Obstacle obstacle = (Obstacle)block;
                        if (obstacle.obstacleType != Obstacle.ObstacleType.Vase)
                        {
                            continue;
                        }
                    }
                    int fallDistance = 0;
                    for (int i = x + 1; i < GridManager.Instance.GetRow(); i++)
                    {
                        if (GridManager.Instance.GetBlock(i, y) == null)
                        {
                            fallDistance++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (fallDistance > 0)
                    {
                        block.Fall(fallDistance);

                        GridManager.Instance.SetBlock(x, y, null);
                        GridManager.Instance.SetBlock(x + fallDistance, y, block);
                        block.SetX(x + fallDistance);
                        // Set the sorting order
                        block.GetComponent<SpriteRenderer>().sortingOrder -= fallDistance * GridManager.Instance.GetColumn();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Sets the game state to falling, indicating blocks are currently moving.
    /// </summary>
    public void StartFalling()
    {
        CurrentGameState = GameState.Falling;
    }

    /// <summary>
    /// Sets the game state to idle, indicating blocks have stopped moving.
    /// </summary>
    public void StopFalling()
    {
        CurrentGameState = GameState.Idle;
    }

    /// <summary>
    /// Updates the goal counts based on the current state of the grid.
    /// </summary>
    public void UpdateGoals()
    {
        for (int i = 0; i < LevelInitializer.Instance.levelData.goals.Length; i++)
        {
            LevelGoal goal = LevelInitializer.Instance.levelData.goals[i];
            goal.count = 0;

            for (int x = 0; x < GridManager.Instance.GetRow(); x++)
            {
                for (int y = 0; y < GridManager.Instance.GetColumn(); y++)
                {
                    Block block = GridManager.Instance.GetBlock(x, y);
                    if (block != null && block.type == Block.BlockType.Obstacle)
                    {
                        Obstacle obstacle = (Obstacle)block;
                        switch (goal.type)
                        {
                            case "bo":
                                if (obstacle.obstacleType == Obstacle.ObstacleType.Box)
                                {
                                    goal.count++;
                                }
                                break;
                            case "s":
                                if (obstacle.obstacleType == Obstacle.ObstacleType.Stone)
                                {
                                    goal.count++;
                                }
                                break;
                            case "v":
                                if (obstacle.obstacleType == Obstacle.ObstacleType.Vase)
                                {
                                    goal.count++;
                                }
                                break;
                            default:
                                Debug.LogError("Unknown obstacle type: " + goal.type);
                                break;
                        }
                    }
                }
            }

            if (i < LevelInitializer.Instance.goalDisplays.Count)
            {
                LevelInitializer.Instance.goalDisplays[i].UpdateGoalDisplay(goal.count);
            }
        }
    }

    /// <summary>
    /// Checks if all level goals are met.
    /// </summary>
    /// <returns>True if all goals are met, false otherwise.</returns>
    private bool CheckGoals()
    {
        for (int i = 0; i < LevelInitializer.Instance.levelData.goals.Length; i++)
        {
            LevelGoal goal = LevelInitializer.Instance.levelData.goals[i];
            if (goal.count > 0)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Retrieves the current move count.
    /// </summary>
    /// <returns>The number of remaining moves.</returns>
    public int GetMoveCount()
    {
        return int.Parse(moveCountText.text);
    }
}
