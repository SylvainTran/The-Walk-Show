using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseEvent : GameClockEvent
{
    private float healthDecreaseTick = 10.0f;

    public DiseaseEvent(float triggerChance) : base(triggerChance)
    {

    }

    public override bool ApplyEvent(CharacterModel b)
    {
        if(!base.ApplyEvent(b))
        {
            return false;
        }
        Message = $"{b.Name()} has gotten a disease on the ship. Health decreased by {healthDecreaseTick}.\n";
        b.Health -= healthDecreaseTick;
        AddToEventMarkersFeed(b);
        // Check if dead after it's done especially
        if(CheckIfDead(b)) // TODO make distinction between checking if dead and calling the on dead event
        {
            b.SetLastEvent("Fatal Disease");
            base.NotifyIsDead(b.gameObject);
        }
        return true;
    }

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        if(b.eventMarkersMap.EventMarkersFeed == null)
        {
            return;
        }

        string diseaseEventAchievement = Enum.GetName(typeof(Enums.CharacterAchievements), 1);
        if (b.eventMarkersMap.EventMarkersFeed.ContainsKey(diseaseEventAchievement))
        {
            b.eventMarkersMap.EventMarkersFeed[diseaseEventAchievement]++;
            return;
        }
        else
        {
            b.eventMarkersMap.EventMarkersFeed.Add(diseaseEventAchievement, 1);
        }
    }
}
