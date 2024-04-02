using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    public abstract void EnterState();
    public abstract void Update();
    public abstract void ExitState();

    public abstract void HandleTap(Vector2 pos);
}
