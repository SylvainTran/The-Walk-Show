using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoungeButton : PrimaryMenuButton
{
    public override void Start()
    {
        base.Start();
        AddOnClickCommand(new TriggerPopUpRequest(this));
        GameEngine.GetGameEngine().AddLogObserver(new LogObserver(this));
    }
}
