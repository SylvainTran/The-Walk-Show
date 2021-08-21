using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class WaypointEvent : GameClockEvent
{
    public Action<CharacterModel, GameWaypoint>[] actionMethodPointers = null;

    public WaypointEvent(float triggerChance, Action<CharacterModel, GameWaypoint>[] actionMethodPointers) : base(triggerChance)
    {
        this.actionMethodPointers = actionMethodPointers;
    }

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        throw new System.NotImplementedException();
    }
}
