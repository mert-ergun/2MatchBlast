using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : GameState
{
    public override void EnterState()
    {
        Debug.Log("Entering Start State");
    }

    public override void Update()
    {
        Debug.Log("Updating Start State");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Start State");
    }

    public override void HandleTap(Vector2 pos)
    {
        Debug.Log("Handling Tap in Start State");
    }
    
}
