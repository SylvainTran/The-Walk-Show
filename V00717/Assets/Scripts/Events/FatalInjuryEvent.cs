using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatalInjuryEvent : GameClockEvent
{
    public FatalInjuryEvent() : base(0.0f)
    {

    }
    public FatalInjuryEvent(float triggerChance) : base(triggerChance)
    {

    }

    public override bool ApplyEvent(BabyModel b)
    {
        Message = $"{b.Name} has died from a fatal injury on the ship.\n";
        b.Health = 0.0f;
        return true;
    }
}
