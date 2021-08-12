using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSentimentEvent : GameClockEvent
{
    public delegate void OnNewSentimentEvent(GameClockEvent e, ICombatant c);
    public static event OnNewSentimentEvent _OnNewSentimentEvent;

    public NewSentimentEvent() : base(0.0f)
    {

    }
    public NewSentimentEvent(float triggerChance) : base(triggerChance)
    {

    }

    public override bool ApplyEvent(CharacterModel b)
    {
        Message = $"{b.Name()} wants to talk with you.\n";
        SendNotification(b);
        return true;
    }
    // Needs to be resolved
    public void SendNotification(CharacterModel b)
    {
        _OnNewSentimentEvent(this, b);
    }

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        throw new System.NotImplementedException();
    }
}
