using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnakeEvent : InjuryEvent
{
    public SnakeEvent(float triggerChance, Action<CharacterModel, GameWaypoint>[] actionMethodPointers) : base(triggerChance, actionMethodPointers)
    {
    }

    public override Texture2D GetEventIcon()
    {
        return (Texture2D)Resources.Load($"Art/Icons/snake");
    }
}
