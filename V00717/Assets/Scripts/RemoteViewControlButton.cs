using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteViewControlButton : PrimaryMenuButton
{
    public override void Start()
    {
        base.Start();
        AddOnClickCommand(new TriggerPopUpRequest(this));
        AddOnClickCommand(new TriggerSceneChange(this, GameEngine.GetGameEngine().gameObject.GetComponent<SceneController>()));
        GameEngine.GetGameEngine().AddLogObserver(new LogObserver(this));
    }
}
