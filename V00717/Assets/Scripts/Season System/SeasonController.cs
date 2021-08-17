
public class SeasonController
{
    /// <summary>
    /// The game states are like a story
    /// </summary>
    public enum GAME_STATE
    {
        SEASON_INTRO, // When characters are created by the player
        QUADRANT_SELECTION, // The player assigns a quadrant for each character
        SCAVENGING, // Characters seek food and establish a base
        RESOLUTION, // When events occur
        FINALE, // Characters reflect
        INTERMISSION // Subscribers vote
    }

    public static GAME_STATE currentGameState;

    public SeasonController(GAME_STATE startingState)
    {
        currentGameState = startingState;
        if(startingState == GAME_STATE.SEASON_INTRO)
        {
            // Play cinematic video clip
            PlaySeasonIntroVideo();
        }
    }

    public void PlaySeasonIntroVideo()
    {

    }

    public delegate void SeasonIntroAction();
    public static event SeasonIntroAction _OnSeasonIntroAction;
    public static void SetSeasonIntro()
    {
        currentGameState = SeasonController.GAME_STATE.SEASON_INTRO;
        _OnSeasonIntroAction();
    }

    public delegate void QuadrantSelectionAction();
    public static event QuadrantSelectionAction _OnQuadrantSelectionAction;
    public static void SetQuadrantSelection()
    {
        currentGameState = SeasonController.GAME_STATE.QUADRANT_SELECTION;
        _OnQuadrantSelectionAction();
    }

    public delegate void ScavengingStateAction();
    public static event ScavengingStateAction _OnScavengingStateAction;
    public static void SetScavengingState()
    {
        currentGameState = SeasonController.GAME_STATE.SCAVENGING;
        _OnScavengingStateAction();
    }

    public delegate void ResolutionStateAction();
    public static event ResolutionStateAction _OnResolutionStateAction;
    public static void SetResolutionState()
    {
        currentGameState = SeasonController.GAME_STATE.RESOLUTION;
        _OnResolutionStateAction();
    }

    public delegate void FinalStateAction();
    public static event FinalStateAction _OnFinalStateAction;
    public static void SetFinaleState()
    {
        currentGameState = SeasonController.GAME_STATE.FINALE;
        _OnFinalStateAction();
    }

    public delegate void IntermissionStateAction();
    public static event IntermissionStateAction _OnIntermissionStateAction;
    public static void SetIntermissionState()
    {
        currentGameState = SeasonController.GAME_STATE.INTERMISSION;
        _OnIntermissionStateAction();
    }
}
