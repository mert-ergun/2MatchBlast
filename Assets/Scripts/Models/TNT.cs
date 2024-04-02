using UnityEngine;

public class TNT : Block
{
    public Sprite tntSprite;

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
        if (blockType == "t")
        {
            type = BlockType.TNT;
        }
    }
}
