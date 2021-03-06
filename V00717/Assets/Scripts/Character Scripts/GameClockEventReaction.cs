using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClockEventReaction : GameClockEvent
{
    private Viewer randomViewer = null;
    private Subscriber randomSubscriber = null;

    public GameClockEventReaction(string message, Viewer randomViewer, Subscriber randomSubscriber) : base()
    {
        this.message = message;
        this.randomViewer = randomViewer;
        this.randomSubscriber = randomSubscriber;
    }

    public override Texture2D GetEventIcon()
    {
        throw new System.NotImplementedException();
    }

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        throw new System.NotImplementedException();
    }

}
