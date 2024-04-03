using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Idle,
        Falling,
    }

    public GameState CurrentGameState { get; private set; } = GameState.Idle;

    [SerializeField]
    private TextMeshProUGUI moveCountText;
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

}
