using System;
using System.Collections.Generic;
using UnityEngine;

public class GameClockEventController
{
    private GameController GameController;

    // The chance that a game clock event triggers
    public float triggerChance = 50.0f;

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
        // Don't update events if no colonists or controller or not right stage of game
        if (SeasonController.currentGameState == SeasonController.GAME_STATE.QUADRANT_SELECTION || GameController.Colonists == null || GameController.Colonists.Count == 0)
        {
            return;
        }
        int randInt = UnityEngine.Random.Range(1, GameController.Colonists.Count);

        for(int i = 0; i < randInt; i++)
        {
            GameController.dashboardOSController.AssignNewUIActionEvent();
        }
        // TODO iterate a new event for each dead colonist too? 
    }

    // Generates a random event using an index
    public WaypointEvent GenerateRandomWaypointEvent(CharacterModel character)
    {
        // Polymorphic late binding
        WaypointEvent gameClockEvent = null;
        //int randIndex = UnityEngine.Random.Range(0, 4);
        //int randIndex = 3;
        int randIndex = 0; // DEBUG
        switch (randIndex)
        {
            case 0:
                gameClockEvent = new SnakeEvent(triggerChance, new Action<CharacterModel, GameWaypoint>[] { GameController.quadrantMapper.GoToQuadrant });
                break;
            //case 1:
            //    gameClockEvent = new DiseaseEvent(triggerChance);
            //    break;
            //case 2:
            //    gameClockEvent = new BattleEvent(triggerChance); // TODO make by encounter with other gos?
            //    break;
            //case 3:
            //    gameClockEvent = new PendingCallEvent(triggerChance / 2);
            //    break;
            default: // TODO add death as a bug event?
                break;
        }
        return gameClockEvent;
    }
}
