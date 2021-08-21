
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameController gameController;

    public SeasonController(GAME_STATE startingState, GameController gameController)
    {
        currentGameState = startingState;
        this.gameController = gameController;

        if (startingState == GAME_STATE.SEASON_INTRO)
        {
            // Play cinematic video clip
            PlaySeasonIntroVideo();
        }
        GameClockEvent._OnColonistIsDead += AnnounceDeath;
    }

    public void PlaySeasonIntroVideo()
    {

    }

    public int rejectedAuditions = 0;

    public void EndAuditions()
    {
        SetQuadrantSelection();
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
        _OnSeasonIntroAction();
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
        _OnScavengingStateAction(); //TODO add notifications/sounds/etc. on state change, removed because no subscribers yet
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

    public static void CheckQuadrantsReached()
    {
       if(QUADRANTS_REACHED < CreationController.MAX_COLONISTS)
        {
            ++QUADRANTS_REACHED;
        }
       if(QUADRANTS_REACHED == CreationController.MAX_COLONISTS)
        {
            //Debug.Log("Starting scavenging phase");
            SetScavengingState();
        }
    }

    public static void ScavengingPhaseFlag(List<GameObject> images)
    {
        QUADRANTS_ASSIGNED++;

        if (QUADRANTS_ASSIGNED >= CreationController.MAX_COLONISTS)
        {
            DashboardOSController.ClearQuadrantUIActions(images);
            SetScavengingState();
        }
    }
}
