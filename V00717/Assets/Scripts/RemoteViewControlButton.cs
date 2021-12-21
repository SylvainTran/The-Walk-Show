using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteViewControlButton : PrimaryMenuButton
{
    public override void Start()
    {
        base.Start();
        AddOnClickCommand(new TriggerPopUpRequest(this));
        // TODO: Don't change scenes, just the camera and UI
        AddOnClickCommand(new TriggerSceneChange(this, GameEngine.GetGameEngine().SceneController));
        GameEngine.GetGameEngine().RemoteViewControl();
        GameEngine.GetGameEngine().AddLogObserver(new LogObserver(this));
    }
}
