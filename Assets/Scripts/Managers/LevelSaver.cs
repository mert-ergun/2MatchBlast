using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSaver : Singleton<LevelSaver>
{
    public int level;
    public bool replayLevel = false;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("lastSave"))
        {
            ResetLevel();
        } else
        {
            level = GetLastSaveLevel();
        }
    }

    // Save current level as json
    public void SaveCurrentLevel()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.level_number = LevelInitializer.Instance.levelData.level_number;
        saveData.grid_width = LevelInitializer.Instance.levelData.grid_width;
        saveData.grid_height = LevelInitializer.Instance.levelData.grid_height;
        saveData.move_count = GameManager.Instance.GetMoveCount();
        saveData.grid = GetCurrentGrid();

        float[,] gridPoses = GridManager.Instance.gridPos;

        // Create a new 1D string array to store the grid positions, start from the bottom left corner
        string[] gridPos = new string[(LevelInitializer.Instance.levelData.grid_width * LevelInitializer.Instance.levelData.grid_height) + 1];
        gridPos[0] = saveData.level_number.ToString();
        int index = 1;
        for (int x = LevelInitializer.Instance.levelData.grid_height - 1; x >= 0; x--)
        {
            for (int y = 0; y < LevelInitializer.Instance.levelData.grid_width; y++)
            {
                gridPos[index] = gridPoses[x, y].ToString();
                index++;
            }
        }
        string jsonGridPos = string.Join(";", gridPos);

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("lastSave", json);
        PlayerPrefs.SetString("gridPos", jsonGridPos);
        PlayerPrefs.Save();
    }

    private string[] GetCurrentGrid()
    {
        string[] grid = new string[LevelInitializer.Instance.levelData.grid_width * LevelInitializer.Instance.levelData.grid_height];
        int index = 0;
        // Read the grid starting from the bottom left corner
        for (int x = LevelInitializer.Instance.levelData.grid_height - 1; x >= 0; x--)
        {
            for (int y = 0; y < LevelInitializer.Instance.levelData.grid_width; y++)
            {
                Block block = GridManager.Instance.GetBlock(x, y);

                if (block == null)
                {
                    grid[index] = "n";
                    index++;
                    continue;
                }

                switch (block.type)
                {
                    case Block.BlockType.Cube:
                        Cube cube = (Cube)block;
                        switch (cube.color)
                        {
                            case Cube.CubeColor.Red:
                                grid[index] = "r";
                                break;
                            case Cube.CubeColor.Green:
                                grid[index] = "g";
                                break;
                            case Cube.CubeColor.Blue:
                                grid[index] = "b";
                                break;
                            case Cube.CubeColor.Yellow:
                                grid[index] = "y";
                                break;
                        }
                        break;


                    case Block.BlockType.Obstacle:
                        Obstacle obstacle = (Obstacle)block;
                        switch (obstacle.obstacleType)
                        {
                            case Obstacle.ObstacleType.Box:
                                grid[index] = "bo";
                                break;
                            case Obstacle.ObstacleType.Stone:
                                grid[index] = "s";
                                break;
                            case Obstacle.ObstacleType.Vase:
                                grid[index] = "v";
                                break;
                        }
                        break;


                    case Block.BlockType.TNT:
                        grid[index] = "t";
                        break;
                }
                index++;
            }
        }
        return grid;
    }

    // Load last saved level
    public bool IsLastSave()
    {
        return PlayerPrefs.HasKey("lastSave");
    }

    public string LoadLastSave()
    {
        return PlayerPrefs.GetString("lastSave");
    }

    public int GetLastSaveLevel()
    {
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(LoadLastSave());
        return saveData.level_number;
    }

    public bool CheckForGridPos()
    {
        return PlayerPrefs.HasKey("gridPos");
    }

    public float[,] GetGridPos()
    {
        float[,] gridPos = new float[LevelInitializer.Instance.levelData.grid_height, LevelInitializer.Instance.levelData.grid_width];
        string json = PlayerPrefs.GetString("gridPos");

        string[] strings = json.Split(';');

        int index = 1;
        for (int x = LevelInitializer.Instance.levelData.grid_height - 1; x >= 0; x--)
        {
            for (int y = 0; y < LevelInitializer.Instance.levelData.grid_width; y++)
            {
                gridPos[x, y] = float.Parse(strings[index]);
                index++;
            }
        }

        return gridPos;
    }

    public int GetGridPosLevel()
    {
        string json = PlayerPrefs.GetString("gridPos");
        string[] strings = json.Split(';');
        return int.Parse(strings[0]);
    }

    public void ResetLevel()
    {
        level = 1;
        SetLevelFromJson();
    }

    public void IncreaseLevel()
    {
        level++;
        SetLevelFromJson();
    }

    public void SetLevelFromJson()
    {
        if (level == 11)
        {
            return;
        }
        string jsonPath = "Assets/Levels/level_" + level.ToString("00") + ".json";
        string jsonContents = System.IO.File.ReadAllText(jsonPath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonContents);

        string json = JsonUtility.ToJson(saveData);

        PlayerPrefs.SetString("lastSave", json);
        PlayerPrefs.DeleteKey("gridPos");
        PlayerPrefs.Save();
    }

}
