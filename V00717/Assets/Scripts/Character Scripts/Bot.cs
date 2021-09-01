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

    // Combat specific
    [SerializeField]
    protected GameObject chasedTarget;
    [SerializeField]
    protected float attackRange = 3.0f;
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected float health = 100.0f;
    [SerializeField]
    protected float damage = 1.0f;
    [SerializeField]
    protected float attackSpeed = 1.0f; // Delay in s before next attack
    [SerializeField]
    protected bool fleeingState = false;

    /// <summary>
    /// If this is set to true, then the character will focus on finding gold in its quadrant
    /// unless the player assigns a direct task to them.
    /// </summary>
    public bool seekGold = false;

    GameController gameController;
    int quadrantIndex = -1;

    // Start is called before the first frame update
    public virtual void Start()
    {
        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        characterModel = GetComponent<CharacterModel>();
        quadrantSize = new Vector3(30.0f, 0.0f, 30.0f); // Get this from actual mesh/plane size
        animator = GetComponent<Animator>();
        gameController = FindObjectOfType<GameController>();
        quadrantIndex = GetComponent<CharacterModel>().InQuadrant;
        if (quadrantIndex > -1)
        {
            quadrantTarget = gameController.quadrantMapper.gameWayPoints[quadrantIndex];
            gameController.quadrantMapper.GoToQuadrant(GetComponent<CharacterModel>(), quadrantTarget);
        }
    }

    public virtual bool Seek(Vector3 location)
    {
        if(!agent)
        {
            agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        }
        if (!agent.isOnNavMesh)
        {
            Debug.Log("Agent not set on navmesh correctly.");
            return false;
        }
        bool successful = agent.SetDestination(location);

        if (successful)
        {
            GetComponent<Animator>().SetBool("isWalking", true);
            coolDown = true;
            return true;
        } else
        {
            Debug.Log("Failed to set a new path.");
            
            if(agent.pathPending)
            {
                Debug.Log("The path is pending but hanged");
            }
            return false;
        }
    }
    public bool Flee(Vector3 location)
    {
        if (!agent.isOnNavMesh)
        {
            return false;
        }
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
        return true;
    }

    protected Vector3 wanderTarget;
    /// <summary>
    /// Wander until arrived at destination.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Wander()
    {
        if (!agent.isOnNavMesh || coolDown)
        {
            yield return null;
        }
        RandomizeWanderParameters();
        BehaviourCoolDown(true);
        Seek(wanderTarget);
        yield return new WaitUntil(ArrivedAtDestination);
        // Reset behaviour and pick a new wander target
        BehaviourCoolDown(false);

        // Repeat if no chasing target
        if (chasedTarget == null)
        {
            StartCoroutine(Wander());
        }
    }

    public bool ArrivedAtDestination()
    {
        if (agent == null) return false;
        return agent.remainingDistance <= stoppingRange;
    }

    private float maxRadius = 50.0f;
    public void RandomizeWanderParameters()
    {
        if(quadrantTarget == null || quadrantIndex == -1)
        {
            return;
        }
        // The wandering is done using a max radius range around the quadrant Target
        // if past it, reset and pick a new wandering target inside the range of the quadrant target radius
        quadrantTarget = gameController.quadrantMapper.gameWayPoints[quadrantIndex];
        wanderTarget = quadrantTarget.transform.position;

        float wanderX = (quadrantTarget.transform.position - new Vector3(Random.Range(-maxRadius, maxRadius), 0.0f, 0.0f)).x;
        float wanderZ = (quadrantTarget.transform.position - new Vector3(0.0f, 0.0f, Random.Range(-maxRadius, maxRadius))).z;
        wanderTarget = new Vector3(wanderX, 0.0f, wanderZ);        
        wanderDistance = Random.Range(0, 25);
        // TODO decide if want to add jitter and factor in wanderRadius too

        Debug.Log($"The quadrant Index: {quadrantIndex} for {GetComponent<CharacterModel>().NickName}, Wandering routine-going to: {wanderTarget} in world position, from quadrantTarget {quadrantTarget.transform.position}");
    }

    protected bool coolDown = false;
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

    public IEnumerator ResetAgentIsStopped(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.agent.isStopped = false;
    }
}
