using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : GameState
{
    public override void EnterState()
    {
        Debug.Log("Entering Play State");
    }

    public override void Update()
    {
        Debug.Log("Updating Play State");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Play State");
    }

    public override void HandleTap(Vector2 pos)
    {
        Debug.Log("Handling Tap in Play State");
    }
}
