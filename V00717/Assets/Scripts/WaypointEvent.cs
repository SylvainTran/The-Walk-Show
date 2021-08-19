using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointEvent : GameClockEvent
{
    public Action<CharacterModel, GameWaypoint>[] actionMethodPointers = null;

    public WaypointEvent(float triggerChance, Action<CharacterModel, GameWaypoint>[] actionMethodPointers) : base(triggerChance)
    {
        this.actionMethodPointers = actionMethodPointers;
    }

    public override Image GetEventIcon()
    {
        throw new System.NotImplementedException();
    }

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        throw new System.NotImplementedException();
    }
}
