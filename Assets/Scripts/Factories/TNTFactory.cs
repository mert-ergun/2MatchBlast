using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTFactory : BlockFactory
{
    public GameObject tntPrefab;
    public GameObject blocks;

    public override GameObject CreateBlock(string type, Vector2 pos)
    {
        GameObject tnt = Instantiate(tntPrefab, pos, Quaternion.identity);
        tnt.transform.SetParent(blocks.transform);
        tnt.name = "TNT";

        return tnt;
    }
}
