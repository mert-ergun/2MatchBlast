using UnityEngine;

/// <summary>
/// Represents a TNT block with specific behaviors and appearance, especially during explosion.
/// </summary>
public class TNT : Block
{
    /// <summary>
    /// The default sprite for the TNT block.
    /// </summary>
    public Sprite tntSprite;

    // Sprites for the explosion effects
    public Sprite explodeEffect1;
    public Sprite explodeEffect2;

    /// <summary>
    /// Initializes the TNT block's appearance.
    /// </summary>
    void Start()
    {
        UpdateTNTAppearance();
    }

    /// <summary>
    /// Activates the TNT block, triggering specific logic and interactions in the game.
    /// </summary>
    public override void ActivateBlock()
    {
        base.ActivateBlock();

        // Additional activation logic for TNT
        GameManager.Instance.HandleTNTTap(this);
    }

    /// <summary>
    /// Updates the appearance of the TNT block based on its state.
    /// </summary>
    private void UpdateTNTAppearance()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the TNT object.");
            return;
        }

        spriteRenderer.sprite = tntSprite;
    }

    /// <summary>
    /// Sets the type of the block to TNT. Always sets the type to TNT.
    /// </summary>
    /// <param name="blockType">The type to be assigned to the block.</param>
    public override void SetType(string blockType)
    {
        if (blockType == "TNT")
        {
            type = BlockType.TNT;
        }
    }

    /// <summary>
    /// Triggers the explosion effect for the TNT block.
    /// </summary>
    public override void Explode()
    {
        StartCoroutine(ExplodeEffect());
    }

    /// <summary>
    /// A coroutine that handles the visual effect of the TNT explosion.
    /// </summary>
    private System.Collections.IEnumerator ExplodeEffect()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the TNT object.");
            yield break;
        }

        // Set to the first explosion effect and start small
        spriteRenderer.sprite = explodeEffect2;
        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero; // Start from zero scale

        float duration = 0.1f; // Duration for the scaling effect of the first explosion
        float time = 0;

        // Scale up the explodeEffect2
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale; // Ensure it's set to the original scale

        // Wait and switch to explodeEffect1
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.sprite = explodeEffect1;
        transform.localScale = Vector3.zero; // Start from zero scale again

        // Adjust the values for the second effect
        float secondEffectDuration = 0.2f; // Longer duration for the second explosion effect
        Vector3 biggerScale = originalScale * 2; // Make it scale to a bigger size
        time = 0; // Reset the time for the next scaling effect

        // Scale up the explodeEffect1 faster and bigger
        while (time < secondEffectDuration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, biggerScale, time / secondEffectDuration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = biggerScale; // Ensure it's set to the bigger scale

        // Return the TNT to the pool after the effect is complete
        ObjectPool.Instance.ReturnToPool("TNT", gameObject);
    }


}
