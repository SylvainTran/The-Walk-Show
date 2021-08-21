using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class HealEvent : WaypointEvent
{
    public float foodHealMin = 1.0f;
    public float foodHealMax = 5.0f;

    public HealEvent(float triggerChance, Action<CharacterModel, GameWaypoint>[] actionMethodPointers) : base(triggerChance, actionMethodPointers)
    {
        
    }

    public override bool ApplyEvent(CharacterModel b)
    {
        float healRange = UnityEngine.Random.Range(foodHealMin, foodHealMax);
        b.Health += healRange;
        AddToEventMarkersFeed(b);
        return true;
    }

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        string healEventAchievement = Enum.GetName(typeof(Enums.CharacterAchievements), 7);
        if (b.eventMarkersMap.EventMarkersFeed.ContainsKey(healEventAchievement))
        {
            b.eventMarkersMap.EventMarkersFeed[healEventAchievement]++;
            return;
        }
        else
        {
            b.eventMarkersMap.EventMarkersFeed.Add(healEventAchievement, 1);
        }
    }
}
