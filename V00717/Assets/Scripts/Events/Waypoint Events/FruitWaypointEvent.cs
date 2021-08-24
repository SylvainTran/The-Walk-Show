using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class FruitWaypointEvent : HealEvent
{
    public FruitWaypointEvent(float triggerChance, System.Delegate[] actionMethodPointers) : base(triggerChance, actionMethodPointers)
    {

    }

    protected override void AddToEventMarkersFeed(CharacterModel b)
    {
        string danceEventAchievement = Enum.GetName(typeof(Enums.CharacterAchievements), 8);
        if (b.eventMarkersMap.EventMarkersFeed.ContainsKey(danceEventAchievement))
        {
            b.eventMarkersMap.EventMarkersFeed[danceEventAchievement]++;
            return;
        }
        else
        {
            b.eventMarkersMap.EventMarkersFeed.Add(danceEventAchievement, 1);
        }
    }

    public override Texture2D GetEventIcon()
    {
        int randIcon = UnityEngine.Random.Range(1, 5);
        return (Texture2D)Resources.Load($"Art/Icons/fruits_0{randIcon}");
    }
}
