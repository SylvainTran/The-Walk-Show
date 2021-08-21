using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DiseaseEvent : WaypointEvent
{
    private float healthDecreaseTick = 10.0f;
    private Image icon;

    public DiseaseEvent(float triggerChance, Action<CharacterModel, GameWaypoint>[] actionMethodPointers) : base(triggerChance, actionMethodPointers)
    {
        //icon = (Image)Resources.Load("Assets/Art/Icons/InjuryEventIcon.png");
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

    public override Texture2D GetEventIcon()
    {
        throw new NotImplementedException();
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
