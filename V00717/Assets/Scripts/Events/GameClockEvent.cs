using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class GameClockEvent
{
    // The chance that the event actually triggers - gameclockeventreaction also inherits it
    protected float triggerChance = default;
    public float TriggerChance { get { return triggerChance; } set { triggerChance = value; } }

    // Message to display in the event log - the gameclockeventreaction also inherits it
    protected string message = default;
    public string Message { get { return message; } set { message = value; } }

    // Dead colonist event (by injury, illness, battle, etc.)

    public GameClockEvent()
    {
        this.triggerChance = 65.0f;
    }

    public GameClockEvent(float triggerChance)
    {
        this.triggerChance = triggerChance;
    }

    // Apply the event
    public virtual bool ApplyEvent(CharacterModel b)
    {
        if (CheckIfDead(b))
        {
            Debug.Log("Colonist is already dead.");
            return false;
        }
        Debug.Log($"Colonist {b.Name()} received an event: {GetType()}");      

        return true;
    }

    public bool CheckIfDead(ICombatant b)
    {
        if (b.GetHealth() <= 0.0f)
        {
            return true;
        }
        return false;
    }

    public void NotifyIsDead(GameObject b)
    {
        if (!b.GetComponent<CharacterModel>().IsEnemyAI())
        {
            string monumentMaterial = "stone"; // TODO randomize materials of death sculpture
            message = $"{b.GetComponent<CharacterModel>().Name()} has died at {b.transform.position}, and has become a monument of {monumentMaterial}.";
        }
    }

    // Add a count to the event type
    protected abstract void AddToEventMarkersFeed(CharacterModel b);

    public override string ToString()
    {
        return this.GetType().ToString();
    }
    // Compare
    public bool Equals(UnityEngine.Object other)
    {
        return other.GetType() == this.GetType();
    }

    public abstract Texture2D GetEventIcon();
}
