using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DetectCharacter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("CharacterBot"))
        {
            return;
        }
        if(other.CompareTag("CharacterBot") && other.gameObject.GetComponent<Bot>().quadrantTarget != null)
        {
            if(other.gameObject.GetComponent<Bot>().quadrantTarget.intKey == GetComponent<GameWaypoint>().intKey)
            {
                other.gameObject.GetComponent<Bot>().BehaviourCoolDown(false);
                other.gameObject.GetComponent<Animator>().SetBool("isWalking", false);
                other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                other.gameObject.GetComponent<NavMeshAgent>().ResetPath();
                StartCoroutine(other.gameObject.GetComponent<Bot>().ResetAgentIsStopped(5.0f));
            }
        }
        if(GetComponent<GameWaypoint>().waypointEvent != null && SeasonController.currentGameState == SeasonController.GAME_STATE.SCAVENGING)
        {
            other.GetComponent<CharacterModel>().OnGameClockEventGenerated(GetComponent<GameWaypoint>().waypointEvent);
        }
    }
}
