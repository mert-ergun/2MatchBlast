using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFactory : BlockFactory
{
    public GameObject cubePrefab;
    public GameObject blocks;

    public override Block CreateBlock(string type, Vector2 pos)
    {
        GameObject cube = Instantiate(cubePrefab, pos, Quaternion.identity);
        cube.transform.SetParent(blocks.transform);
        cube.name = "Cube";
        cube.GetComponent<Cube>().SetType(type);

        return cube.GetComponent<Cube>();
    }
}
