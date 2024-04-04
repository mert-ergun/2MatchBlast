using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockFactory : MonoBehaviour
{
    public abstract GameObject CreateBlock(string type, Vector2 pos);
}
