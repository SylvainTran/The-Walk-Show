using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeEvent : InjuryEvent
{
    public SnakeEvent(float triggerChance, Action<CharacterModel, GameWaypoint>[] actionMethodPointers) : base(triggerChance, actionMethodPointers)
    {
    }
}
