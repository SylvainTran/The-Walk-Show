using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InjuryEvent : WaypointEvent
{
    public float injuryDamageMin = 1.0f;
    public float injuryDamageMax = 5.0f;

    public InjuryEvent(float triggerChance, Action<CharacterModel, GameWaypoint>[] actionMethodPointers) : base(triggerChance, actionMethodPointers)
    {

    }

    public override bool ApplyEvent(CharacterModel b)
    {
        float injuryRange = UnityEngine.Random.Range(injuryDamageMin, injuryDamageMax);
        //Message = $"{b.Name()} has been injured {injuryRange} injury damage taken.\n";
        b.Health -= injuryRange;
        AddToEventMarkersFeed(b);
        if(CheckIfDead(b))
        {
            b.SetLastEvent("Fatal Injury");
            base.NotifyIsDead(b.gameObject);
        }
        return true;
    }

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        string injuryEventAchievement = Enum.GetName(typeof(Enums.CharacterAchievements), 3);
        if (b.eventMarkersMap.EventMarkersFeed.ContainsKey(injuryEventAchievement))
        {
            b.eventMarkersMap.EventMarkersFeed[injuryEventAchievement]++;
            return;
        }
        else
        {
            b.eventMarkersMap.EventMarkersFeed.Add(injuryEventAchievement, 1);
        }
    }
}
