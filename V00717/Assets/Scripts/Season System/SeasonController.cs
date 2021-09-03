
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class SeasonController
{
    /// <summary>
    /// The game states are like a story
    /// </summary>
    public enum GAME_STATE
    {
        SEASON_INTRO, // When auditioning characters are casted by the player
        QUADRANT_SELECTION, // The player assigns a quadrant for each character
        SCAVENGING, // Characters seek food and establish a base
        RESOLUTION, // When events occur
        FINALE, // Characters reflect
        INTERMISSION // Subscribers vote
    }

    public static GAME_STATE currentGameState;

    /// <summary>
    /// Quadrant north-east available? (Quadrant selection phase).
    /// If null it is available.
    /// </summary>
    public static CharacterModel quadrantNEOwner = null;
    /// <summary>
    /// Quadrant north-west available?
    /// </summary>
    public static CharacterModel quadrantNWOwner = null;
    /// <summary>
    /// Quadrant south-west available?
    /// </summary>
    public static CharacterModel quadrantSWOwner = null;
    /// <summary>
    /// Quadrant south-east available?
    /// </summary>
    public static CharacterModel quadrantSEOwner = null;

    public static int QUADRANTS_ASSIGNED = 0;
    public static int QUADRANTS_REACHED = 0;

    public static GameController gameController;

    /// <summary>
    /// The AI behaviours that each actor will loop. These are game design functions.
    /// </summary>
    public enum ACTOR_ROLES
    {
        VAMPIRE, GRAVEDIGGER, SLAYER, FARMER, ZOMBIE, HUMAN, PREDATOR
    }
    public Vampire vampire;
    public GraveDigger graveDigger;
    public Slayer slayer;
    public Farmer farmer;

    /// <summary>
    /// The "personality" forms for the game design functions.
    /// </summary>
    public enum  ACTOR_SKINS
    {
        SINGER, WRITER, PRODUCER, CLOWN
    }

    public SeasonController(GameController gameController)
    {
        SeasonController.gameController = gameController;

        if (currentGameState == GAME_STATE.SEASON_INTRO)
        {
            PlaySeasonIntroVideo();
            SetSeasonIntro();
        }
        GameClockEvent._OnColonistIsDead += AnnounceDeath;
    }

    public void PlaySeasonIntroVideo()
    {

    }

    public int rejectedAuditions = 0;

    public void EndAuditions()
    {
        DestroyAuditionEditors();
        SetScavengingState();
        gameController.CloseSpecialEventsWindow();
    }

    public void DestroyAuditionEditors()
    {
        foreach(GameObject g in gameController.auditionEditorsInGame)
        {
            GameObject.Destroy(g.gameObject);
        }
    }

    public void AnnounceDeath(GameClockEvent e, GameObject c)
    {
        // Narrator voice saying they're dead, video clip popup, counter update
        if(gameController.Colonists.Count <= 1)
        {
            SetIntermissionState();
            // Show highlights?

            // And then restarts new season from auditions
        }
    }

    public delegate void SeasonIntroAction();
    public static event SeasonIntroAction _OnSeasonIntroAction;
    public static void SetSeasonIntro()
    {
        currentGameState = GAME_STATE.SEASON_INTRO;
        //gameController._OnSeasonIntroAction();
        gameController.SetupIntroPhase();
    }

    public delegate void QuadrantSelectionAction();
    public static event QuadrantSelectionAction _OnQuadrantSelectionAction;
    public static void SetQuadrantSelection()
    {
        currentGameState = GAME_STATE.QUADRANT_SELECTION;
        _OnQuadrantSelectionAction();
    }

    public delegate void ScavengingStateAction();
    public static event ScavengingStateAction _OnScavengingStateAction;
    public static void SetScavengingState()
    {
        currentGameState = GAME_STATE.SCAVENGING;
        Debug.Log("Starting scavenging phase");
        //_OnScavengingStateAction();
        //TODO add notifications/sounds/etc. on state change, removed because no subscribers yet
    }

    public delegate void ResolutionStateAction();
    public static event ResolutionStateAction _OnResolutionStateAction;
    public static void SetResolutionState()
    {
        currentGameState = GAME_STATE.RESOLUTION;
        _OnResolutionStateAction();
    }

    public delegate void FinalStateAction();
    public static event FinalStateAction _OnFinalStateAction;
    public static void SetFinaleState()
    {
        currentGameState = GAME_STATE.FINALE;
        _OnFinalStateAction();
    }

    public delegate void IntermissionStateAction();
    public static event IntermissionStateAction _OnIntermissionStateAction;
    public static void SetIntermissionState()
    {
        currentGameState = GAME_STATE.INTERMISSION;
        _OnIntermissionStateAction();
    }
}
