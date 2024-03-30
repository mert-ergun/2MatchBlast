using UnityEngine;
using System.IO;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;
using TMPro;

[System.Serializable]
public class LevelData
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] grid;
}

public class LevelInitializer : MonoBehaviour
{
    public GameObject gridBackground;
    public TextMeshProUGUI moveCountText;
    public GameObject blockPrefab; 
    public LevelSaver levelSaver;

    private void Start()
    {
        string pathToJson = levelSaver.level.ToString();
        if (pathToJson.Length == 1)
        {
            pathToJson = "0" + pathToJson;
        }
        InitializeLevel("Assets/Levels/level_" + pathToJson + ".json");
    }

    private void InitializeLevel(string pathToJson)
    {
        string jsonContents = File.ReadAllText(pathToJson);
        LevelData levelData = JsonUtility.FromJson<LevelData>(jsonContents);

        // Stretch grid_background based on grid_width and grid_height
        Vector2 gridScale = new Vector2(levelData.grid_width, levelData.grid_height);
        SpriteRenderer spriteRenderer = gridBackground.GetComponent<SpriteRenderer>();
        spriteRenderer.size = gridScale;

        // Update move_count text
        moveCountText.text = levelData.move_count.ToString();

        // Place blocks
        for (int i = 0; i < levelData.grid.Length; i++)
        {
            // Calculate position based on i, grid_width, and grid_height
            Vector2 position = CalculatePosition(i, levelData.grid_width, levelData.grid_height);
            GameObject block = Instantiate(blockPrefab, position, Quaternion.identity);

            // Assuming you have a script attached to your blockPrefab that manages the block type
            //block.GetComponent<Block>().SetType(levelData.grid[i]);
        }
    }

    private Vector2 CalculatePosition(int index, int width, int height)
    {
        // Calculate the position of the block based on its index
        int x = index % width;
        int y = index / width; // integer division

        // Convert grid position to world position if necessary
        Vector2 basePosition = new Vector2(-width / 2, -height / 2); // Center the grid
        Vector2 worldPosition = new Vector2(x, y) + basePosition;

        return worldPosition;
    }
}
