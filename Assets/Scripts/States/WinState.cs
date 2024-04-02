using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : GameState
{
    public override void EnterState()
    {
        Debug.Log("Entering Win State");
    }

    public override void Update()
    {
        Debug.Log("Updating Win State");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Win State");
    }

    public override void HandleTap(Vector2 pos)
    {
        Debug.Log("Handling Tap in Win State");
    }
}
