using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DetectCharacter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.GetComponent<MainActor>())
        {
            return;
        }
        if(other.gameObject.GetComponent<Bot>().quadrantTarget.intKey == GetComponent<GameWaypoint>().intKey)
        {
            Debug.Log("Arrived at a waypoint destination - possibly triggering some event");
            other.gameObject.GetComponent<Bot>().BehaviourCoolDown(false);
            other.gameObject.GetComponent<Animator>().SetBool("isWalking", false);
            other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            other.gameObject.GetComponent<NavMeshAgent>().ResetPath();
            StartCoroutine(other.gameObject.GetComponent<Bot>().ResetAgentIsStopped(5.0f));
        }
        if(GetComponent<GameWaypoint>().waypointEvent != null && SeasonController.currentGameState == SeasonController.GAME_STATE.SCAVENGING)
        {
            other.GetComponent<CharacterModel>().OnGameClockEventGenerated(GetComponent<GameWaypoint>().waypointEvent);
        }
    }
}
