using System;
using System.Collections.Generic;
using UnityEngine;

public class GameClockEventController
{
    private GameController GameController;

    // The chance that a game clock event triggers
    public float triggerChance = 50.0f;
    public string[] colors = { "cyan", "black", "blue", "magenta", "red", "blue", "yellow", "white", "pink", "green", "purple" };
    public string[] sizes = { "small", "medium", "large", "extra-large", "extra-extra-large"};
    public string[] adjectives = { "tasty", "sour", "ripe", "delightful", "sweet", "bittersweet" };

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
        int randIndex = UnityEngine.Random.Range(0, 3); // DEBUG
        int randColor = UnityEngine.Random.Range(0, colors.Length);
        int randSizes = UnityEngine.Random.Range(0, sizes.Length);
        int randAdjectives = UnityEngine.Random.Range(0, adjectives.Length);
        string randomAdverb = GameController.adverbsDatabase.adverbs[UnityEngine.Random.Range(0, GameController.adverbsDatabase.adverbs.Length)];
        switch (randIndex)
        {
            case 0:
                gameClockEvent = new SnakeEvent(triggerChance, new QuadrantMapper.NavigationAttempt[] { GameController.quadrantMapper.GoToQuadrant });
                gameClockEvent.Message = $"{character.NickName} {randomAdverb} walked into a {sizes[randSizes]}-sized {colors[randColor]} snake and they got injured.";
                break;
            case 1:
                gameClockEvent = new FruitWaypointEvent(triggerChance, new QuadrantMapper.NavigationAttempt[] { GameController.quadrantMapper.GoToQuadrant });
                gameClockEvent.Message = $"{character.NickName} {randomAdverb} picked up a {sizes[randSizes]}-sized {colors[randColor]} and {adjectives[randAdjectives]} fruit.";
                break;
            case 2:
                gameClockEvent = new DanceWaypointEvent(triggerChance, new QuadrantMapper.NavigationAttempt[] { GameController.quadrantMapper.GoToQuadrant });
                gameClockEvent.Message = $"{character.NickName} couldn't shake the itch to dance away and busted some {randomAdverb} moves.";
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
