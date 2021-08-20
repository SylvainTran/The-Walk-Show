using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChannelController : MonoBehaviour
{
    public List<Viewer> viewers;
    public List<Subscriber> subscribers;
    public List<Patron> patrons;

    public void GenerateRandomViewers()
    {

    }

    public GameClockEventReaction GenerateReaction(GameClockEvent e)
    {
        string message = "";

        Viewer randomViewer = viewers[UnityEngine.Random.Range(0, viewers.Count)];
        Subscriber randomSubscriber = subscribers[UnityEngine.Random.Range(0, subscribers.Count)];

        return new GameClockEventReaction(message, randomViewer, randomSubscriber);
    }
}
