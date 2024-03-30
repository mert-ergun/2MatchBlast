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

[System.Serializable]
public class LevelGoal
{
    public string type;
    public int count;
}

public class LevelInitializer : MonoBehaviour
{
    public GameObject gridBackground;
    public TextMeshProUGUI moveCountText;
    public GameObject blockPrefab;
    public GameObject cubePrefab;
    public GameObject obstaclePrefab;
    public GameObject tntPrefab;
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
        // Read JSON file
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
            
            string blockType = levelData.grid[i];
            GameObject blockObject;

            // Create block based on blockType
            if (blockType == "r" || blockType == "g" || blockType == "b" || blockType == "y" || blockType == "rand")
            {
                blockObject = Instantiate(cubePrefab, position, Quaternion.identity);
                blockObject.transform.parent = blocks.transform;
                blockObject.GetComponent<Cube>().SetType(blockType);
            } else if (blockType == "t")
            {
                blockObject = Instantiate(tntPrefab, position, Quaternion.identity);
                blockObject.transform.parent = blocks.transform;
                blockObject.GetComponent<TNT>().SetType(blockType);
            } else
            {
                blockObject = Instantiate(obstaclePrefab, position, Quaternion.identity);
                blockObject.transform.parent = blocks.transform;
                blockObject.GetComponent<Obstacle>().SetType(blockType);
            }
            // Set order in layer
            blockObject.GetComponent<SpriteRenderer>().sortingOrder = i;
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
