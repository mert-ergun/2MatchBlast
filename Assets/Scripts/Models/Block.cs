using UnityEngine;
using UnityEngine.UIElements;

public class Block : MonoBehaviour
{
    public enum BlockType
    {
        Cube,
        Obstacle,
    }

    public BlockType type;

    // When clicked, activate the block
    protected virtual void OnMouseDown()
    {
        ActivateBlock();
    }

    // Common functionality that might be overridden in derived classes
    public virtual void ActivateBlock()
    {
        // General activation logic
    }

    public virtual void DeactivateBlock()
    {
        gameObject.SetActive(false);
    }

    public virtual void SetType(string blockType)
    {
        // Basic implementation; specific types will override this
    }

}
