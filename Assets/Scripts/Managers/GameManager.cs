using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Idle,
        Falling,
        Finished
    }

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
    public void HandleBlockTap(Block block)
    {
        if (moveCountText.text == "0")
        {
            return;
        }

        if (CurrentGameState == GameState.Idle)
        {
            StartCoroutine(GridManager.Instance.HandleBlockTap(block));
        }
    }

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

    private IEnumerator PopupFail()
    {
        // Activate the fail popup and overlay
        failPopup.SetActive(true);
        GameObject overlay = failPopup.transform.Find("Overlay").gameObject;
        overlay.SetActive(true); 

        GameObject popupContent = failPopup.transform.Find("PopupContent").gameObject; // Ensure you have a 'PopupContent' object
        Vector3 originalScale = popupContent.transform.localScale;
        popupContent.transform.localScale = Vector3.zero;

        // Update the level text
        GameObject popupRibbon = popupContent.transform.Find("PopupRibbon").gameObject;
        TextMeshProUGUI levelText = popupRibbon.transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        levelText.text = "Level " + LevelInitializer.Instance.levelData.level_number;

        float duration = 0.5f;
        float time = 0;

        // Animate only the popup content scaling, not the overlay
        while (time < duration)
        {
            popupContent.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        popupContent.transform.localScale = originalScale;

        LevelSaver.Instance.SetLevelFromJson();
    }

    private IEnumerator PopupWin()
    {
        winPopup.SetActive(true);
        // Wait for a short delay and then return to the main menu
        yield return new WaitForSeconds(5f);
        levelSaver.IncreaseLevel();
        SceneManager.LoadScene("MainScene");
    }


    public void UseMove()
    {
        // Update move count
        moveCountText.text = (int.Parse(moveCountText.text) - 1).ToString();
    }

    public void FallBlock()
    {
        // Fall all the blocks until they reach above another block
        for (int x = GridManager.Instance.GetRow()-1; x >= 0; x--)
        {
            for (int y = GridManager.Instance.GetColumn()-1; y >= 0; y--)
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

    public void StartFalling()
    {
        CurrentGameState = GameState.Falling;
    }


    public void StopFalling()
    {
        CurrentGameState = GameState.Idle;
    }

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

            // Update the corresponding goal display
            if (i < LevelInitializer.Instance.goalDisplays.Count)
            {
                LevelInitializer.Instance.goalDisplays[i].UpdateGoalDisplay(goal.count);
            }
        }

    }

    private bool CheckGoals()
    {
        // Check if all goals are met
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

    public int GetMoveCount()
    {
        return int.Parse(moveCountText.text);
    }

}
