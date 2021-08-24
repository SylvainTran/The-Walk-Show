using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DanceWaypointEvent : HealEvent
{
    public DanceWaypointEvent(float triggerChance, System.Delegate[] actionMethodPointers) : base(triggerChance, actionMethodPointers)
    {

    }

    public override Texture2D GetEventIcon()
    {
        return (Texture2D)Resources.Load($"Art/Icons/dance");
    }
}
