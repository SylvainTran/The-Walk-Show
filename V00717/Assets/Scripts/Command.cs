using UnityEngine;

public interface ICommand
{
    public void Execute();
}

public class TriggerPopUpRequest : ICommand
{
    PrimaryMenuButton primaryMenuButton;
    GameObject objectToPopUp;
    public TriggerPopUpRequest(PrimaryMenuButton b)
    {
        primaryMenuButton = b;
        this.objectToPopUp = null;
    }
    public TriggerPopUpRequest(PrimaryMenuButton b, GameObject o)
    {
        primaryMenuButton = b;
        this.objectToPopUp = o;
    }
    public void Execute()
    {
        Debug.Log("Triggering Pop Up Request + Notified its Observers!");
        primaryMenuButton.Notify();
        if(objectToPopUp != null)
        {
            objectToPopUp.SetActive(true);
        }
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