using System;
using UnityEngine;
public class GameClockEvent
{
    // The chance that the event actually triggers
    private float triggerChance = default;
    public float TriggerChance { get { return triggerChance; } set { triggerChance = value; } }

    // Message to display in the event log
    private string message = default;
    public string Message { get { return message; } set { message = value; } }

    // Dead colonist event (by injury, illness, battle, etc.)
    public delegate void OnColonistIsDead(GameClockEvent e);
    public static event OnColonistIsDead _OnColonistIsDead;

    public GameClockEvent(float triggerChance)
    {
        this.triggerChance = triggerChance;
    }

    // Apply the event
    public virtual bool ApplyEvent(BabyModel b)
    {
        if (CheckIfDead(b))
        {
            Debug.Log("Colonist is already dead.");
            return false;
        }
        Debug.Log($"Colonist {b.Name} received an event: {GetType()}");
        return true;
    }

    public bool CheckIfDead(ICombatant b)
    {
        if (b.GetHealth() <= 0.0f)
        {
            if (!b.IsEnemyAI())
            {
                //message = $"{b.CombatName()} has died.";
                //_OnColonistIsDead(this);
            }
            return true;
        }
        return false;
    }

    // Compare
    public bool Equals(UnityEngine.Object other)
    {
        return other.GetType() == this.GetType();
    }
}
