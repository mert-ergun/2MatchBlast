using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Used to store the data of a level from a JSON file.
/// </summary>
[System.Serializable]
public class LevelData
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] grid;
    public LevelGoal[] goals = new LevelGoal[0];
}

/// <summary>
/// Used to store the goal data of a level from a JSON file.
/// </summary>
[System.Serializable]
public class LevelGoal
{
    public string type;
    public int count;
}

/// <summary>
/// Used to initialize the level based on the JSON file or Last Save.
/// </summary>
public class LevelInitializer : Singleton<LevelInitializer>
{
    public GameObject gridBackground;
    public TextMeshProUGUI moveCountText;
    public GameObject blockPrefab;
    public GameObject cubePrefab;
    public GameObject obstaclePrefab;
    public GameObject tntPrefab;
    public GameObject goalPrefab;
    public Transform goalParent;
    public GameObject blocks;
    public LevelData levelData;
    public List<GoalDisplay> goalDisplays = new List<GoalDisplay>();

    // Step is the distance between two blocks
    public float step;

    /// <summary>
    /// Starts the level based on the JSON file or Last Save.
    /// </summary>
    private void Start()
    {
        // If the level is being replayed, load the level from the JSON file
        if (LevelSaver.Instance.replayLevel)
        {
            InitializeLevel("Assets/Levels/level_" + LevelSaver.Instance.level.ToString("00") +".json", false);
            LevelSaver.Instance.replayLevel = false;
            return;
        }

        // If there is a last save, load the last save
        if (LevelSaver.Instance.IsLastSave())
        {
            InitializeLevel(LevelSaver.Instance.LoadLastSave(), true);
            return;
        }

        // Load the level from the JSON file
        InitializeLevel("Assets/Levels/level_" + LevelSaver.Instance.level.ToString("00") + ".json", false);
    }

    /// <summary>
    /// Initializes the level based on the JSON file or Last Save.
    /// </summary>
    /// <param name="pathToJson">Path to the JSON file</param>
    /// <param name="saved">True if the level is loaded from the Last Save, false otherwise</param>
    private void InitializeLevel(string pathToJson, bool saved)
    {
        string jsonContents;
        if (saved)
        {
            jsonContents = pathToJson;
        } else
        {
            // Read JSON file
            jsonContents = File.ReadAllText(pathToJson);
        }

        levelData = JsonUtility.FromJson<LevelData>(jsonContents);

        // Get block size
        Vector2 blockSize = blockPrefab.GetComponent<SpriteRenderer>().size;

        // Calculate step based on block size
        step = blockSize.x * blockPrefab.transform.localScale.x;

        // Stretch grid_background based on grid_width and grid_height
        Vector2 gridScale = new Vector2(levelData.grid_width - step / 2, levelData.grid_height - (step / 2) + 0.066f);
        SpriteRenderer spriteRenderer = gridBackground.GetComponent<SpriteRenderer>();
        spriteRenderer.size = gridScale;

        // Set the Sprite Mask size to match the grid background, this mask will be used to hide the blocks outside the grid
        SpriteMask spriteMask = GameObject.Find("SpriteMask").GetComponent<SpriteMask>();
        spriteMask.transform.position = new Vector2(0, -1.016f);
        spriteMask.transform.localScale = new Vector3(gridScale.x * 4.7f, gridScale.y * 3.36f, 0);

        // Update move_count text
        moveCountText.text = levelData.move_count.ToString();

        // Initialize grid as a 2D array of Blocks
        Block[][] grid = new Block[levelData.grid_height][];
        for (int i = 0; i < levelData.grid_height; i++)
        {
            grid[i] = new Block[levelData.grid_width];
        }

        // Place blocks
        for (int i = 0; i < levelData.grid.Length; i++)
        {
            // Calculate position based on i, grid_width, and grid_height
            Vector2 position = CalculatePosition(i, levelData.grid_width, levelData.grid_height);
            
            string blockType = levelData.grid[i];
            GameObject blockObject;

            // Create block based on blockType
            if (blockType == "r" || blockType == "g" || blockType == "b" || blockType == "y" || blockType == "rand")
            {
                blockObject = Instantiate(cubePrefab, position, Quaternion.identity);
                blockObject.transform.parent = blocks.transform;
                blockObject.GetComponent<Cube>().SetType(blockType);
                grid[levelData.grid_height - (i / levelData.grid_width) - 1][i % levelData.grid_width] = blockObject.GetComponent<Cube>();

            } else if (blockType == "t")
            {
                blockObject = Instantiate(tntPrefab, position, Quaternion.identity);
                blockObject.transform.parent = blocks.transform;
                blockObject.GetComponent<TNT>().SetType("TNT");
                grid[levelData.grid_height - (i / levelData.grid_width) - 1][i % levelData.grid_width] = blockObject.GetComponent<TNT>();
            } else if (blockType == "n")
            {
                // Empty block
                grid[levelData.grid_height - (i / levelData.grid_width) - 1][i % levelData.grid_width] = null;
                continue;
            } else
            {
                blockObject = Instantiate(obstaclePrefab, position, Quaternion.identity);
                blockObject.transform.parent = blocks.transform;
                blockObject.GetComponent<Obstacle>().SetType(blockType);
                grid[levelData.grid_height - (i / levelData.grid_width) - 1][i % levelData.grid_width] = blockObject.GetComponent<Obstacle>();

                // If this type of block is not already in the list of obstacles, add it, otherwise increment the count
                if (!levelData.goals.Any(goal => goal.type == blockType))
                {
                    LevelGoal newGoal = new LevelGoal();
                    newGoal.type = blockType;
                    newGoal.count = 1;
                    ArrayUtility.Add(ref levelData.goals, newGoal);
                } else
                {
                    levelData.goals.First(goal => goal.type == blockType).count++;
                }
                
            }
            // Set order in layer
            blockObject.GetComponent<SpriteRenderer>().sortingOrder = i;
        }

        GridManager.Instance.InitializeGrid(grid);
        InitializeGoals(levelData);
    }

    /// <summary>
    /// Calculates the position of a block based on the index, grid width, and grid height.
    /// </summary>
    /// <returns>Position of the block</returns>
    private Vector2 CalculatePosition(int index, int width, int height)
    {
        // Find the grid background's position, get it's left-bottom corner, first block will be placed there, then move to the right until width is reached, then move up
        Vector2 gridBackgroundPosition = gridBackground.transform.position;
        Vector2 gridBackgroundSize = gridBackground.GetComponent<SpriteRenderer>().size;
        Vector2 gridBackgroundLeftBottomCorner = gridBackgroundPosition - gridBackgroundSize / 4;

        int x = index % width;
        int y = index / width;

        Vector2 blockSize = blockPrefab.GetComponent<SpriteRenderer>().size;

        float xPosition = gridBackgroundLeftBottomCorner.x + x * step + step * 0.66f;
        float yPosition = gridBackgroundLeftBottomCorner.y + y * step + step * 0.66f;

        return new Vector2(xPosition, yPosition);
    }

    /// <summary>
    /// Initializes the goals of the level.
    /// </summary>
    private void InitializeGoals(LevelData levelData)
    {
        goalDisplays.Clear();

        int goalsCount = levelData.goals.Length;
        Vector2 parentSize = goalParent.GetComponent<RectTransform>().sizeDelta;
        float goalSize;
        int defaultFontSize = 36;

        // Adjust the spacing or layout settings based on the number of goals
        if (goalsCount == 1)
        {
            // If there's only one goal, it should take the full size of the parent
            goalSize = parentSize.x; 
        }
        else
        {
            // If there are two or more goals, they should be smaller and side by side
            goalSize = parentSize.x / 1.5f;
        } 
        
        for (int i = 0; i < goalsCount; i++)
        {
            GameObject goalObject = Instantiate(goalPrefab, goalParent);
            RectTransform goalRectTransform = goalObject.GetComponent<RectTransform>();

            // Set the size
            goalRectTransform.sizeDelta = new Vector2(goalSize, goalSize);
            // Set the image and text size
            Image goalImage = goalObject.GetComponentInChildren<Image>();
            goalImage.rectTransform.sizeDelta = new Vector2(goalSize, goalSize);
            TextMeshProUGUI goalText = goalObject.GetComponentInChildren<TextMeshProUGUI>();
            goalText.fontSize = (goalsCount == 1 ? defaultFontSize : (defaultFontSize / 2));
            goalText.rectTransform.sizeDelta = new Vector2(goalSize, goalSize);
            Image goalCheck = goalObject.transform.Find("GoalCheck").GetComponent<Image>();
            goalCheck.rectTransform.sizeDelta /= goalsCount;
            if (goalsCount >= 2)
            {
                goalText.rectTransform.anchoredPosition = new Vector2(15, -20);
                goalCheck.rectTransform.anchoredPosition = new Vector2(15, -15);
            }

            // If there are two goals, place them side by side
            if (goalsCount == 2)
            {
                goalRectTransform.anchoredPosition = new Vector2((parentSize.x * i / 2) - parentSize.x/4, 0);
            } // If there are three goals, place 2 side by side and one below, centered
            else if (goalsCount == 3)
            {
                goalRectTransform.anchoredPosition = new Vector2((parentSize.x * i / 2) - parentSize.x / 4, parentSize.y / 6);
                if (i == 2)
                {
                    goalRectTransform.anchoredPosition = new Vector2(0, -parentSize.y / 6);
                }
            }

            // Setup the goal display
            GoalDisplay goalDisplay = goalObject.GetComponent<GoalDisplay>();
            if (goalDisplay != null)
            {
                goalDisplay.SetupGoal(levelData.goals[i]);
                goalDisplays.Add(goalDisplay);
            }
        }
    }
}
