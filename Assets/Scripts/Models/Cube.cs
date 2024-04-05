using UnityEngine;

/// <summary>
/// Represents a Cube block with specific color and type properties.
/// </summary>
public class Cube : Block
{
    /// <summary>
    /// Defines possible colors for the cube.
    /// </summary>
    public enum CubeColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }

    /// <summary>
    /// Defines possible types for the cube. TNT type cubes will transform into TNT objects when activated.
    /// </summary>
    public enum CubeType
    {
        Normal,
        TNT
    }

    public CubeColor color;
    public CubeType cubeType;

    // Sprites for each color and type
    public Sprite redSprite;
    public Sprite greenSprite;
    public Sprite blueSprite;
    public Sprite yellowSprite;

    // TNT sprites for each color
    public Sprite redTntSprite;
    public Sprite greenTntSprite;
    public Sprite blueTntSprite;
    public Sprite yellowTntSprite;

    /// <summary>
    /// Initializes the cube appearance based on its properties.
    /// </summary>
    void Start()
    {
        UpdateCubeAppearance();
    }

    /// <summary>
    /// Activates the cube, triggering game-specific logic.
    /// </summary>
    public override void ActivateBlock()
    {
        base.ActivateBlock();
        // Additional activation logic for Cubes
        GameManager.Instance.HandleBlockTap(this);
    }

    /// <summary>
    /// Updates the cube's appearance based on its current properties.
    /// </summary>
    private void UpdateCubeAppearance()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the cube object.");
            return;
        }

        // Choose the sprite based on the cube's color and type
        switch (color)
        {
            case CubeColor.Red:
                spriteRenderer.sprite = cubeType == CubeType.Normal ? redSprite : redTntSprite;
                break;
            case CubeColor.Green:
                spriteRenderer.sprite = cubeType == CubeType.Normal ? greenSprite : greenTntSprite;
                break;
            case CubeColor.Blue:
                spriteRenderer.sprite = cubeType == CubeType.Normal ? blueSprite : blueTntSprite;
                break;
            case CubeColor.Yellow:
                spriteRenderer.sprite = cubeType == CubeType.Normal ? yellowSprite : yellowTntSprite;
                break;
        }
    }

    /// <summary>
    /// Sets the cube's color based on a string identifier.
    /// </summary>
    /// <param name="blockType">A string representing the cube's type.</param>
    public override void SetType(string blockType)
    {
        type = BlockType.Cube;

        switch (blockType)
        {
            case "r":
                color = CubeColor.Red;
                break;
            case "g":
                color = CubeColor.Green;
                break;
            case "b":
                color = CubeColor.Blue;
                break;
            case "y":
                color = CubeColor.Yellow;
                break;
            case "rand":
                color = GetRandomColor();
                break;
        }

        UpdateCubeAppearance();
    }

    /// <summary>
    /// Sets the cube to be a TNT type.
    /// </summary>
    public void SetTNT()
    {
        cubeType = CubeType.TNT;
        UpdateCubeAppearance();
    }

    /// <summary>
    /// Sets the cube to be a normal (non-TNT) type.
    /// </summary>
    public void SetNormal()
    {
        cubeType = CubeType.Normal;
        UpdateCubeAppearance();
    }

    private CubeColor GetRandomColor()
    {
        return (CubeColor)Random.Range(0, 4);
    }
}
