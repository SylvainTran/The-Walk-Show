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
        if(other.CompareTag("CharacterBot"))
        {
            other.gameObject.GetComponent<Animator>().SetBool("isWalking", false);
            other.gameObject.GetComponent<Bot>().quadrantTarget = null;
        }
        if(GetComponent<GameWaypoint>().waypointEvent != null && SeasonController.currentGameState == SeasonController.GAME_STATE.SCAVENGING)
        {
            other.GetComponent<CharacterModel>().OnGameClockEventGenerated(GetComponent<GameWaypoint>().waypointEvent);
        }
    }
}
