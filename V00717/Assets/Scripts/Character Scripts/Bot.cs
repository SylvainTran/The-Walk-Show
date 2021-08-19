using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Bot : MonoBehaviour
{
    protected NavMeshAgent agent;
    public GameObject target;
    public float wanderRadius;
    public float wanderDistance;
    public float wanderJitter;

    public CharacterModel characterModel;
    public GameWaypoint quadrantTarget = null; // Set when the character is assigned one
    public float stoppingRange = 0.01f;
    public Vector3 quadrantSize = Vector3.zero;

    private void OnEnable()
    {
        SeasonController._OnScavengingStateAction += SeekWithinQuadrant;
    }

    private void OnDisable()
    {
        SeasonController._OnScavengingStateAction -= SeekWithinQuadrant;
    }
    // Start is called before the first frame update
    public void Start()
    {
        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        characterModel = GetComponent<CharacterModel>();
        quadrantSize = new Vector3(30.0f, 0.0f, 30.0f); // Get this from actual mesh/plane size
    }

    public void Seek(Vector3 location)
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }
        agent.SetDestination(location);        
    }

    public void Flee(Vector3 location)
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    Vector3 wanderTarget = Vector3.zero;
    public void Wander()
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }
        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter,
                                        0,
                                        Random.Range(-1.0f, 1.0f) * wanderJitter);
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);
        Seek(targetWorld);
    }

    protected bool coolDown = true;
    public void BehaviourCoolDown(bool state)
    {
        coolDown = state;
    }

    public void ViewTombstoneBehaviour(GameObject go)
    {
        BehaviourCoolDown(true);
        Seek(go.transform.position);
        StartCoroutine(ResetBehaviourCooldown(UnityEngine.Random.Range(5.0f, 30.0f)));
        // Trigger 'paying hommage' event to event log and broadcast viewers chat for reactions. The key word is REACTION.

    }

    private IEnumerator ResetBehaviourCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        BehaviourCoolDown(false);
    }

    public void Die()
    {
        BehaviourCoolDown(true);
        GetComponent<NavMeshAgent>().isStopped = true;
    }

    public void MoveToQuadrant(GameWaypoint v)
    {
        if(v != null)
        {
            quadrantTarget = v;
            Debug.Log("Moving to quadrant waypoint at: " + quadrantTarget.transform.position);
            Seek(quadrantTarget.transform.position);
        }
    }

    public void SeekWithinQuadrant()
    {
        if(quadrantTarget == null || coolDown == false)
        {
            return;
        }
        BehaviourCoolDown(false);
        // Clamp wander radius from the quadrantTarget position
        // wanderDistance = quadrantSize.z;
    }

    public void WrapQuadrant()
    {
        if(quadrantTarget == null)
        {
            return;
        }
        if (Vector3.Distance(quadrantTarget.transform.position, transform.position) >= Vector3.Distance(quadrantTarget.transform.position, quadrantSize))
        {
            MoveToQuadrant(quadrantTarget);
            // Another way
            // could be to add an actual box collider around each quadrant that is enabled once the characters
            // are set in place. Or that collider is used to check the distance at that moment.
        }
    }

    public void Update()
    {
        //if (!coolDown)
        //{
            //Wander();
        //}
        //WrapQuadrant();
    }
}
