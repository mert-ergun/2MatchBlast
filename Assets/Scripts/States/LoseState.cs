using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseState : GameState
{
    public override void EnterState()
    {
        Debug.Log("Entering Lose State");
    }

    public override void Update()
    {
        Debug.Log("Updating Lose State");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Lose State");
    }

    public override void HandleTap(Vector2 pos)
    {
        Debug.Log("Handling Tap in Lose State");
    }
}
