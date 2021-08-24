using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WaypointEvent : GameClockEvent
{
    public System.Delegate[] actionMethodPointers = null;

    public WaypointEvent(float triggerChance, System.Delegate[] actionMethodPointers) : base(triggerChance)
    {
        this.actionMethodPointers = actionMethodPointers;
    }

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        throw new System.NotImplementedException();
    }
}
