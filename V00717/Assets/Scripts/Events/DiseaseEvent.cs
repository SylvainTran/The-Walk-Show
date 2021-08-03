using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseEvent : GameClockEvent
{
    private float healthDecreaseTick = 10.0f;

    public DiseaseEvent(float triggerChance) : base(triggerChance)
    {

    }

    public override bool ApplyEvent(BabyModel b)
    {
        if(!base.ApplyEvent(b))
        {
            return false;
        }
        Message = $"{b.Name()} has gotten a disease on the ship. Health decreased by {healthDecreaseTick}.\n";
        b.Health -= healthDecreaseTick;
        return true;
    }
}
