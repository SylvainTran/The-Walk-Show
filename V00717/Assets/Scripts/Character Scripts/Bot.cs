using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(NavMeshAgent))]
public class Bot : MonoBehaviour
{
    protected NavMeshAgent agent;
    public GameObject target;
    public float wanderRadius;
    public float wanderDistance;
    public float wanderJitter;

    public CharacterModel characterModel;
    public GameWaypoint quadrantTarget = null; // Set when the character is assigned one
    [SerializeField] protected float stoppingRange = 6.10f;
    public Vector3 quadrantSize = Vector3.zero;

    // Combat specific
    [SerializeField]
    protected GameObject chasedTarget;
    [SerializeField]
    protected float attackRange = 6.10f;
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    public float health = 100.0f;
    [SerializeField]
    protected float damage = 1.0f;
    [SerializeField]
    protected float attackSpeed = 1.0f; // Delay in s before next attack
    [SerializeField]
    protected bool fleeingState = false;

    public GameController gameController;
    public int quadrantIndex = -1;
    protected Transform parent;

    // Start is called before the first frame update
    public virtual void Start()
    {
        characterModel = GetComponentInParent<CharacterModel>();
        animator = GetComponentInParent<Animator>();
        gameController = FindObjectOfType<GameController>();
        parent = transform.parent.parent.transform;
        agent = GetComponentInParent<NavMeshAgent>();
    }

    public virtual bool Seek(Vector3 location)
    {
        if (!agent || !agent.isOnNavMesh) return false;
        bool successful = agent.SetDestination(location);

        if (successful)
        {
            coolDown = true;
            return true;
        } else
        {            
            if(agent.pathPending)
            {
                Debug.Log("The path is pending but hanged");
            }
            return false;
        }
    }
    public bool Flee(Vector3 location)
    {
        if (!agent.isOnNavMesh) return false;
        Vector3 fleeVector = location - parent.position;
        agent.SetDestination(parent.position - fleeVector);
        return true;
    }

    protected Vector3 wanderTarget;
    /// <summary>
    /// Wander until arrived at destination.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Wander()
    {
        if (!agent.isOnNavMesh || coolDown) yield return null;
        RandomizeWanderParameters();
        BehaviourCoolDown(true);
        Seek(wanderTarget);
        animator.SetBool("isWalking", true);
        yield return new WaitUntil(ArrivedAtDestination);
        // Reset behaviour and pick a new wander target
        BehaviourCoolDown(false);

        // Repeat if no chasing target
        if (chasedTarget == null && !coolDown)
        {
            StartCoroutine(Wander());
        }
    }

    public void FreezeAgent()
    {
        if (!agent || !agent.isOnNavMesh) return;
        StopAllCoroutines();
        agent.isStopped = true;
        agent.ResetPath();
        BehaviourCoolDown(true);
        animator.SetBool("isWalking", false);
    }

    public bool ArrivedAtDestination()
    {
        if (agent == null || !agent.isOnNavMesh) return false;
        return agent.remainingDistance <= stoppingRange;
    }

    private float maxRadius = 50.0f;
    public virtual Vector3 RandomizeWanderParameters()
    {
        if(quadrantTarget == null || quadrantIndex == -1)
        {
            return transform.position;
        }
        // The wandering is done using a max radius range around the quadrant Target
        // if past it, reset and pick a new wandering target inside the range of the quadrant target radius
        quadrantTarget = gameController.quadrantMapper.gameWayPoints[quadrantIndex];
        wanderTarget = quadrantTarget.transform.position;

        float wanderX = (quadrantTarget.transform.position - new Vector3(Random.Range(-maxRadius, maxRadius), 0.0f, 0.0f)).x;
        float wanderZ = (quadrantTarget.transform.position - new Vector3(0.0f, 0.0f, Random.Range(-maxRadius, maxRadius))).z;
        wanderTarget = new Vector3(wanderX, transform.position.y, wanderZ);        
        wanderDistance = Random.Range(0, 15);
        wanderTarget += new Vector3(wanderDistance, 0.0f, wanderDistance);
        return wanderTarget;
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
        StartCoroutine(ResetBehaviourCooldown(Random.Range(5.0f, 30.0f)));
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
        agent.isStopped = true;
    }

    public IEnumerator ResetAgentIsStopped(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(agent && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = false;
            BehaviourCoolDown(false);
        }
    }
}
