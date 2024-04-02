using UnityEngine;

public class Block : MonoBehaviour
{
    public enum BlockType
    {
        Cube,
        Obstacle,
        TNT
    }

    public BlockType type;
    private int x;
    private int y;

    // When clicked, activate the block
    protected virtual void OnMouseDown()
    {
        ActivateBlock();
    }

    // Common functionality that might be overridden in derived classes
    public virtual void ActivateBlock()
    { 
        
    }

    public virtual void DeactivateBlock()
    {
        gameObject.SetActive(false);
    }

    public virtual void SetType(string blockType)
    {
        // Basic implementation; specific types will override this
    }

    public void Fall(int fallDistance)
    {
        Transform transform = gameObject.transform;
        transform.position = new Vector3(transform.position.x, transform.position.y - ((1.42f * 0.33f) * fallDistance), transform.position.z);
    }

    public void SetX(int x)
    {
        this.x = x;
    }

    public void SetY(int y)
    {
        this.y = y;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }
}
