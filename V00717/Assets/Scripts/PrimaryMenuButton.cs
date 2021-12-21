using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrimaryMenuButton : MonoBehaviour, ISubject
{
    Button button;
    List<ICommand> onClickCommands;
    private List<Observer> observers;
    private string stringToLog;

    public List<Observer> Observers { get => observers; set => observers = value; }
    string ISubject.stringToLog { get => stringToLog; set => stringToLog = value; }

    public virtual void Start()
    {
        observers = new List<Observer>();
        stringToLog = $"[Game Engine Click Event Log]: `{this.GetType().Name}` was clicked.";
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleButton);
        onClickCommands = new List<ICommand>();
    }
    public void AddOnClickCommand(ICommand onClickCommand)
    {
        this.onClickCommands.Add(onClickCommand);
    }
    public void ExecuteButtonActions()
    {
        foreach(ICommand command in onClickCommands)
        {
            command.Execute();
        }
    }
    public void HandleButton()
    {
        ExecuteButtonActions();
    }
    public void Attach(Observer o)
    {
        observers.Add(o);
    }
    public void Detach(Observer o)
    {
        observers.Remove(o);
    }
    public virtual void Notify()
    {
        foreach (Observer o in observers)
        {
            o.UpdateState();
        }
    }
}
