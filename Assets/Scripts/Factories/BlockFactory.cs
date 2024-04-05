using UnityEngine;

/// <summary>
/// An abstract factory class responsible for creating block game objects.
/// </summary>
public abstract class BlockFactory : MonoBehaviour
{
    /// <summary>
    /// Creates a block game object based on the specified type and position.
    /// </summary>
    /// <param name="type">The type of block to create, which determines the block's characteristics and appearance.</param>
    /// <param name="pos">The position in the game world where the block should be instantiated.</param>
    /// <returns>A new block game object of the specified type at the given position.</returns>
    public abstract GameObject CreateBlock(string type, Vector2 pos);
}
