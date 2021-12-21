using System.Collections.Generic;

public interface ISubject
{
    List<Observer> Observers
    {
        get;
        set;
    }
    string stringToLog
    {
        get;
        set;
    }
    public void Attach(Observer o);
    public void Detach(Observer o);
    public void Notify();
}