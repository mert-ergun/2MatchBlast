using UnityEngine;

/// <summary>
/// Manages saving and loading level data, as well as player progression.
/// </summary>
public class LevelSaver : Singleton<LevelSaver>
{
    /// <summary>
    /// Current level the player is on.
    /// </summary>
    public int level;

    /// <summary>
    /// Flag indicating whether the current level is being replayed.
    /// </summary>
    public bool replayLevel = false;

    /// <summary>
    /// Initializes the level data at the start of the game.
    /// </summary>
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

    /// <summary>
    /// Saves the current level's state to persistent local storage.
    /// </summary>
    public void SaveCurrentLevel()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.level_number = LevelInitializer.Instance.levelData.level_number;
        saveData.grid_width = LevelInitializer.Instance.levelData.grid_width;
        saveData.grid_height = LevelInitializer.Instance.levelData.grid_height;
        saveData.move_count = GameManager.Instance.GetMoveCount();
        saveData.grid = GetCurrentGrid();

        float[,] gridPoses = GridManager.Instance.gridPos;

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

    /// <summary>
    /// Retrieves the current grid's state as an array of strings.
    /// </summary>
    /// <returns>An array representing the current state of the grid.</returns>
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

    /// <summary>
    /// Checks if there is saved data from a previous session.
    /// </summary>
    /// <returns>True if there is saved data, false otherwise.</returns>
    public bool IsLastSave()
    {
        return PlayerPrefs.HasKey("lastSave");
    }

    /// <summary>
    /// Loads the last saved game data.
    /// </summary>
    /// <returns>The saved game data in string format.</returns>
    public string LoadLastSave()
    {
        return PlayerPrefs.GetString("lastSave");
    }

    /// <summary>
    /// Retrieves the last saved level number.
    /// </summary>
    /// <returns>The level number from the last save.</returns>
    public int GetLastSaveLevel()
    {
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(LoadLastSave());
        return saveData.level_number;
    }

    /// <summary>
    /// Checks if there is saved grid position data from a previous session.
    /// </summary>
    /// <returns>True if there is saved grid position data, false otherwise.</returns>
    public bool CheckForGridPos()
    {
        return PlayerPrefs.HasKey("gridPos");
    }

    /// <summary>
    /// Retrieves the grid position data from the last save.
    /// </summary>
    /// <returns>A 2D array of floats representing the grid positions.</returns>
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

    /// <summary>
    /// Retrieves the level number from the grid position data to check if the player is on the same level.
    /// </summary>
    /// <returns>The level number from the grid position data.</returns>
    public int GetGridPosLevel()
    {
        string json = PlayerPrefs.GetString("gridPos");
        string[] strings = json.Split(';');
        return int.Parse(strings[0]);
    }

    /// <summary>
    /// Resets the level progress to the first level.
    /// </summary>
    public void ResetLevel()
    {
        level = 1;
        SetLevelFromJson();
    }

    /// <summary>
    /// Advances the level by one, updating the current level data accordingly.
    /// </summary>
    public void IncreaseLevel()
    {
        level++;
        SetLevelFromJson();
    }

    /// <summary>
    /// Sets the current level data based on a JSON file corresponding to the current level.
    /// </summary>
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
