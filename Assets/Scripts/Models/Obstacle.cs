using System.Collections;
using UnityEngine;

/// <summary>
/// Represents an obstacle block with specific types and appearances.
/// </summary>
public class Obstacle : Block
{
    /// <summary>
    /// Defines possible types for the obstacle. Each type has a unique appearance and behavior.
    /// </summary>
    public enum ObstacleType
    {
        Stone,
        Vase,
        Box
    }

    public ObstacleType obstacleType;

    // Sprites for each type
    public Sprite stoneSprite;
    public Sprite vaseSprite1;
    public Sprite vaseSprite2;
    public Sprite boxSprite;

    /// <summary>
    /// Initializes the obstacle's appearance based on its type at startup.
    /// </summary>
    void Start()
    {
        UpdateObstacleAppearance();
    }

    /// <summary>
    /// Updates the obstacle's sprite based on its type.
    /// </summary>
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

    /// <summary>
    /// Sets the obstacle's type based on a string identifier.
    /// </summary>
    /// <param name="blockType">A string representing the obstacle's type.</param>
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

    /// <summary>
    /// Triggers the explosion behavior of the obstacle, with additional type-specific effects.
    /// </summary>
    public override void Explode()
    {
        switch (obstacleType)
        {
            case ObstacleType.Stone:
                this.gameObject.SetActive(false);
                base.Explode();
                break;
            case ObstacleType.Vase:
                Sprite sprite = GetComponent<SpriteRenderer>().sprite;
                if (sprite == vaseSprite1)
                {
                    ShakeVase();
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

    /// <summary>
    /// A coroutine to shake the vase, simulating a wobble effect.
    /// </summary>
    /// <param name="duration">How long the shake lasts.</param>
    /// <param name="magnitude">The magnitude of the shake.</param>
    private IEnumerator ShakeVaseCoroutine(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = originalPosition.x + Random.Range(-1f, 1f) * magnitude;
            float y = originalPosition.y + Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x, y, originalPosition.z);
            elapsed += Time.deltaTime;

            yield return null; // Wait until the next frame
        }

        // Return the vase to its original position
        transform.position = originalPosition;
    }

    /// <summary>
    /// Initiates a shaking effect on the vase.
    /// </summary>
    private void ShakeVase()
    {
        if (obstacleType == ObstacleType.Vase)
        {
            // Start the shake coroutine with desired duration and magnitude
            StartCoroutine(ShakeVaseCoroutine(0.1f, 0.02f));
        }
    }

}
