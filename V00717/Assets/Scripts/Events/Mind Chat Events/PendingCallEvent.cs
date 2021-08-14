using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendingCallEvent : GameClockEvent
{
    public delegate void OnPendingCallEvent(GameClockEvent e, CharacterModel c);
    public static event OnPendingCallEvent _OnPendingCallEvent;

    public PendingCallEvent() : base(0.0f)
    {

    }
    public PendingCallEvent(float triggerChance) : base(triggerChance)
    {

    }
    
    public override bool ApplyEvent(CharacterModel b)
    {
        if (!b.IsInPendingCall)
        {
            Message = $"{b.Name()} wants to talk with you.\n";
            AddToEventMarkersFeed(b);
            SendNotification(b);
            return true;
        } else
        {
            return false;
        }
    }
    public IEnumerator WaitForCall(float waitTime, CharacterModel b)
    {
        yield return new WaitForSeconds(waitTime);
        Hang(b);
    }
    // Needs to be resolved
    public void SendNotification(CharacterModel b)
    {
        _OnPendingCallEvent(this, b);
        b.IsInPendingCall = true;
    }

    public void Hang(CharacterModel b)
    {
        if(b.IsInPendingCall)
        {
            // Cancel call and be angry // more stressed
        } else
        {
            return;
        }
    }
    // Method to pick up call in dashboard OS controller? that methods sets isInPendingCall to false

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        string pendingCall = Enums.ToString(Enums.CharacterAchievements.PENDING_CALLS);
        if (b.eventMarkersMap.EventMarkersFeed.ContainsKey(pendingCall))
        {
            b.eventMarkersMap.EventMarkersFeed[pendingCall]++;
            return;
        }
        else
        {
            b.eventMarkersMap.EventMarkersFeed.Add(pendingCall, 1);
        }
    }
}
