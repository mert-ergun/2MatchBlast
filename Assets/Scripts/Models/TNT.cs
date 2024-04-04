using UnityEngine;

public class TNT : Block
{
    public Sprite tntSprite;

    public Sprite explodeEffect1;
    public Sprite explodeEffect2;

    void Start()
    {
        UpdateTNTAppearance();
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
    }

    public override void ActivateBlock()
    {
        base.ActivateBlock();
        // Additional activation logic for TNT

        GameManager.Instance.HandleTNTTap(this);
    }

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

    public override void SetType(string blockType)
    {
        if (blockType == "TNT")
        {
            type = BlockType.TNT;
        }
    }

    public override void Explode()
    {
        StartCoroutine(ExplodeEffect());
    }

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
