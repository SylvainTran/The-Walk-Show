using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchedulingButton : PrimaryMenuButton
{
    public override void Start()
    {
        base.Start();
        AddOnClickCommand(new TriggerPopUpRequest(this, GameEngine.SchedulingWindow));
        // TODO: Weight the pros and cons of adding the currentState->Action() as a command object here too - could be called from anywhere, which is good
        GameEngine.GetGameEngine().AddLogObserver(new LogObserver(this));
    }
}
