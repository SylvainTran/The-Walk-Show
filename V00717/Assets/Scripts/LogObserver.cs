using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogObserver : Observer
{
    ISubject subject;
    public LogObserver(ISubject s)
    {
        subject = s;
        subject.Attach(this);
    }
    ~LogObserver()
    {
        subject.Detach(this);
    }
    public override void UpdateState()
    {
        Debug.Log(subject.stringToLog);
    }
}
