using UnityEngine;
using System.IO;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;
using TMPro;
using UnityEditor.Build.Reporting;

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
    public GameObject blocks;

    public float step;

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

        // Get block size
        Vector2 blockSize = blockPrefab.GetComponent<SpriteRenderer>().size;

        // Calculate step based on block size
        step = blockSize.x * blockPrefab.transform.localScale.x;

        // Stretch grid_background based on grid_width and grid_height
        Vector2 gridScale = new Vector2(levelData.grid_width - step / 2, levelData.grid_height - step / 2);
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
            block.transform.SetParent(blocks.transform);

            // Assuming you have a script attached to your blockPrefab that manages the block type
            //block.GetComponent<Block>().SetType(levelData.grid[i]);
        }
    }

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

}
