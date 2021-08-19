using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCharacter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("CharacterBot"))
        {
            return;
        }
        if(SeasonController.currentGameState != SeasonController.GAME_STATE.QUADRANT_SELECTION)
        {
            return;
        }
        if(SeasonController.currentGameState == SeasonController.GAME_STATE.QUADRANT_SELECTION)
        {
            if (other.gameObject.GetComponent<CharacterModel>().InQuadrant == GetComponent<GameWaypoint>().intKey)
            {
                SeasonController.CheckQuadrantsReached();
            }
        }
        if(GetComponent<GameWaypoint>().waypointEvent != null && SeasonController.currentGameState == SeasonController.GAME_STATE.SCAVENGING)
        {
            other.GetComponent<CharacterModel>().OnGameClockEventGenerated(GetComponent<GameWaypoint>().waypointEvent);
        }
    }
}
