using System.Drawing;
using UnityEngine;

public class Cube : Block
{
    public enum CubeColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }

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

    public Sprite redTntSprite;
    public Sprite greenTntSprite;
    public Sprite blueTntSprite;
    public Sprite yellowTntSprite;

    void Start()
    {
        UpdateCubeAppearance();
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        // Cube click/tap handling logic
        Debug.Log($"Cube tapped, color: {color}, type: {cubeType}");
    }

    public override void ActivateBlock()
    {
        base.ActivateBlock();
        // Additional activation logic for Cubes
    }

    public override void DeactivateBlock()
    {
        base.DeactivateBlock();
        // Additional deactivation logic for Cubes
    }

    public void MatchCube()
    {
        // Logic to handle cube matching
    }

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

    private CubeColor GetRandomColor()
    {
        return (CubeColor)Random.Range(0, 4);
    }
}
