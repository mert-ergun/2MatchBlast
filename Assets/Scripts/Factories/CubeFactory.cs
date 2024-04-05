using UnityEngine;

/// <summary>
/// A concrete factory for creating cube blocks.
/// </summary>
public class CubeFactory : BlockFactory
{
    /// <summary>
    /// Prefab for the cube block.
    /// </summary>
    public GameObject cubePrefab;

    /// <summary>
    /// The parent object under which all created cubes will be organized.
    /// </summary>
    public GameObject blocks;

    /// <summary>
    /// Creates a cube block of a specific type at the given position.
    /// </summary>
    /// <param name="type">The type of cube to create. This will be the color of the cube.</param>
    /// <param name="pos">The world position where the cube should be instantiated.</param>
    /// <returns>A new cube block GameObject.</returns>
    public override GameObject CreateBlock(string type, Vector2 pos)
    {
        // Instantiate a new cube from the prefab at the given position and with default rotation.
        GameObject cube = Instantiate(cubePrefab, pos, Quaternion.identity);

        // Set the parent of the cube to organize it within the scene hierarchy.
        cube.transform.SetParent(blocks.transform);

        // Name the cube for easier identification in the hierarchy.
        cube.name = "Cube " + type;

        // Set the type of the cube, which may influence its behavior or appearance.
        cube.GetComponent<Cube>().SetType(type);

        return cube;
    }
}
