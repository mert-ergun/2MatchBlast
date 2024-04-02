using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : GameState
{
    public override void EnterState()
    {
        Debug.Log("Entering Falling State");
    }

    public override void Update()
    {
        Debug.Log("Updating Falling State");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Falling State");
    }

    public override void HandleTap(Vector2 pos)
    {
        Debug.Log("Handling Tap in Falling State");
    }
}
