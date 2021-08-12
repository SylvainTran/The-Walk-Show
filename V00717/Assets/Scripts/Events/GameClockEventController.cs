using System.Collections.Generic;
using UnityEngine;

public class GameClockEventController
{
    private GameController GameController;

    // The chance that a game clock event triggers
    public float triggerChance = default;

    public GameClockEventController(GameController GameController, float triggerChance)
    {
        this.GameController = GameController;
        this.triggerChance = triggerChance;
        TimeController._OnUpdateEventClock += OnEventClockUpdate;
    }

    ~GameClockEventController()
    {
        TimeController._OnUpdateEventClock -= OnEventClockUpdate;
    }
    // Clock events
    public void OnEventClockUpdate()
    {
        // Don't update events if no colonists or controller
        if (GameController.Colonists == null || GameController.Colonists.Count == 0)
        {
            //Debug.LogError("No colonists to poke with events");
            return;
        }
        // Iterate a new event for each colonist
        for(int i = 0; i < GameController.Colonists.Count; i++)
        {
            GameClockEvent e = GenerateRandomEvent();
            GameController.Colonists[i].GetComponent<CharacterModel>().OnGameClockEventGenerated(e);
        }
        // TODO iterate a new event for each dead colonist too? 
    }

    // Generates a random event using an index
    public GameClockEvent GenerateRandomEvent()
    {
        // Polymorphic late binding
        GameClockEvent gameClockEvent = null;
        int randIndex = Random.Range(0, 3);
        //int randIndex = 3;
        switch(randIndex)
        {
            case 0:
                gameClockEvent = new InjuryEvent(triggerChance);
                break;
            case 1:
                gameClockEvent = new DiseaseEvent(triggerChance);
                break;
            case 2:
                gameClockEvent = new BattleEvent(triggerChance); // May require counseling, therapy for PTSD in Mind Room or it'll increase stress levels and lower morale
                break;
            case 3:
                gameClockEvent = new PendingCallEvent(triggerChance / 10);
                break;
            default: // TODO add death as a bug event?
                break;
        }
        return gameClockEvent;
    }
}
