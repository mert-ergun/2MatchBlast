using UnityEngine;

public class Obstacle : Block
{
    public enum ObstacleType
    {
        Stone,
        Vase,
        Box
    }

    public ObstacleType obstacleType;

    public Sprite stoneSprite;
    public Sprite vaseSprite1;
    public Sprite vaseSprite2;
    public Sprite boxSprite;

    public bool isExploded = false;


    void Start()
    {
        UpdateObstacleAppearance();
    }


    protected override void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public override void ActivateBlock()
    {
        base.ActivateBlock();
        // Additional activation logic for Obstacles
    }

    private void UpdateObstacleAppearance()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the obstacle object.");
            return;
        }

        switch (obstacleType)
        {
            case ObstacleType.Stone:
                spriteRenderer.sprite = stoneSprite;
                break;
            case ObstacleType.Vase:
                spriteRenderer.sprite = vaseSprite1;
                break;
            case ObstacleType.Box:
                spriteRenderer.sprite = boxSprite;
                break;
            default:
                Debug.LogError("Unknown obstacle type: " + obstacleType);
                break;
        }
    }

    public override void SetType(string blockType)
    {
        switch (blockType)
        {
            case "s":
                obstacleType = ObstacleType.Stone;
                break;
            case "v":
                obstacleType = ObstacleType.Vase;
                break;
            case "bo":
                obstacleType = ObstacleType.Box;
                break;
            default:
                Debug.LogError("Unknown obstacle type: " + blockType);
                break;
        }

        type = BlockType.Obstacle;

        UpdateObstacleAppearance();
    }   

    public override void DeactivateBlock()
    {
        base.DeactivateBlock();
        // Additional deactivation logic for Obstacles
    }

    public override void Explode()
    {
        switch (obstacleType)
        {
            case ObstacleType.Stone:
                Debug.Log("Stone obstacle cannot be exploded.");
                return;
            case ObstacleType.Vase:
                Sprite sprite = GetComponent<SpriteRenderer>().sprite;
                if (sprite == vaseSprite1)
                {
                    GetComponent<SpriteRenderer>().sprite = vaseSprite2;
                }
                else
                {
                    this.gameObject.SetActive(false);
                    base.Explode();
                }
                break;
            case ObstacleType.Box:
                this.gameObject.SetActive(false);
                base.Explode();
                break;
        }
    }
}
