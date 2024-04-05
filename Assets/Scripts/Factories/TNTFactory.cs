using UnityEngine;

/// <summary>
/// A concrete factory for creating TNT blocks.
/// </summary>
public class TNTFactory : BlockFactory
{
    /// <summary>
    /// Prefab for the TNT block.
    /// </summary>
    public GameObject tntPrefab;

    /// <summary>
    /// The parent object under which all created TNT blocks will be organized.
    /// </summary>
    public GameObject blocks;

    /// <summary>
    /// Creates a TNT block at a specific position in the game world.
    /// </summary>
    /// <param name="type">The type of TNT to create, will be "TNT" always.</param>
    /// <param name="pos">The world position where the TNT block should be instantiated.</param>
    /// <returns>A new TNT block GameObject.</returns>
    public override GameObject CreateBlock(string type, Vector2 pos)
    {
        // Instantiate a new TNT block from the prefab at the given position and with default rotation.
        GameObject tnt = Instantiate(tntPrefab, pos, Quaternion.identity);

        // Set the parent of the TNT to organize it within the scene hierarchy.
        tnt.transform.SetParent(blocks.transform);

        // Name the TNT for easier identification in the hierarchy.
        tnt.name = "TNT";

        // Set the type of the TNT, which may influence its behavior or appearance.
        tnt.GetComponent<TNT>().SetType(type);

        return tnt;
    }
}
