using UnityEngine;

public interface ICommand
{
    public void Execute();
}

public class TriggerPopUpRequest : ICommand
{
    PrimaryMenuButton primaryMenuButton;
    public TriggerPopUpRequest(PrimaryMenuButton b)
    {
        primaryMenuButton = b;
    }
    public void Execute()
    {
        Debug.Log("Triggering Pop Up Request + Notified its Observers!");
        primaryMenuButton.Notify();
    }
}
public class NotifyConsoleLogger : ICommand
{
    public void Execute()
    {
        Debug.Log("Notifying Console Logger!");
    }
}
public class TriggerSceneChange : ICommand
{
    PrimaryMenuButton primaryMenuButton;
    SceneController sceneController;
    public TriggerSceneChange(PrimaryMenuButton b, SceneController sc)
    {
        primaryMenuButton = b;
        sceneController = sc;
    }
    public void Execute()
    {
        Debug.Log("Triggering Scene Change Request!");
        primaryMenuButton.Notify();
        sceneController.LoadNextScene();
    }
}