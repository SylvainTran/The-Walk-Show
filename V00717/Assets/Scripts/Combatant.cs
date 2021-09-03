using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Combatant : Bot
{
    [SerializeField]
    protected float chaseRange = 20.0f;
    [SerializeField]
    protected float sight = 15.0f;
    public Renderer renderer; // for the bounds
    // Start is called before the first frame update
    private void Start()
    {
        base.Start();
    }
    public void BehaviourSetup(GameWaypoint quadrantTarget)
    {
        this.quadrantTarget = quadrantTarget;
    }
    public override IEnumerator Wander()
    {
        if (!agent)
        {
            agent = this.GetComponent<NavMeshAgent>();
        }
        if (!agent.isOnNavMesh || coolDown)
        {
            yield return null;
        }
        NavMeshPath plannedPath = new NavMeshPath();
        bool success = false;
        if (!success) // TODO make while condition here until success is true => I think there is an issue with the navmesh agent height?
        {
            wanderTarget = RandomizeWanderParameters();
            NavMesh.CalculatePath(transform.position, wanderTarget, NavMesh.AllAreas, plannedPath);
            for (int i = 0; i < plannedPath.corners.Length - 1; i++)
            {
                Debug.DrawLine(plannedPath.corners[i], plannedPath.corners[i + 1], Color.red);
            }
            success = plannedPath.status != NavMeshPathStatus.PathInvalid;
        }
        if(success)
        {
            agent.SetPath(plannedPath);
        } else
        {
            // fall back => 
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                wanderTarget = hit.position;
                Seek(wanderTarget);
            }
        }
        BehaviourCoolDown(true);
        yield return new WaitUntil(ArrivedAtDestination);
        BehaviourCoolDown(false);
        if (chasedTarget == null && !coolDown)
        {
            StartCoroutine(Wander());
        }
    }

    public override Vector3 RandomizeWanderParameters()
    {
        Vector3 localWander;
        // Terrain adjustment
        float t_height = Terrain.activeTerrain.SampleHeight(wanderTarget);
        float radius = Vector3.up.y;

        Vector3 point;
        int attempts = 0;
        while (attempts < 5)
        {
            localWander= new Vector3(Random.Range(-wanderRadius, wanderRadius), 0.0f, Random.Range(-wanderRadius, wanderRadius));
            localWander *= Random.Range(-wanderJitter, wanderJitter);
            localWander.Normalize();
            wanderTarget = transform.position + transform.TransformDirection(localWander);
            wanderDistance = Random.Range(15, 45);
            wanderTarget += new Vector3(wanderDistance, 0.0f, wanderDistance);
            if (renderer != null)
            {
                radius = renderer.bounds.extents.magnitude;
            }
            wanderTarget += new Vector3(0.0f, t_height + 0.5f, 0.0f); // y => y + t_height + radius
            NavMeshHit hit;
            if (NavMesh.SamplePosition(wanderTarget, out hit, 25.0f, NavMesh.AllAreas))
            {
                wanderTarget = hit.position;
                break;
            }
            ++attempts;
        }

        return wanderTarget;
        //Debug.Log($"The quadrant Index: {quadrantIndex} for {GetComponent<CharacterModel>().NickName}, Wandering routine-going to: {wanderTarget} in world position, from quadrantTarget {quadrantTarget.transform.position}");
    }

    /// <summary>
    /// Seek and destroy
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public override bool Seek(Vector3 target)
    {
        return base.Seek(target);
    }

    public virtual IEnumerator LockCombatState(float attackSpeed, Combatant opponent)
    {
        yield return new WaitForSeconds(attackSpeed);
        if (opponent.health > 0.0f)
        {
            DealDamage(opponent);
            StartCoroutine(LockCombatState(attackSpeed, opponent));
        }
        else
        {
            StopAllCoroutines();
            animator.SetBool("isAttacking", false);
        }
    }

    public GameObject priorityCollider = null;
    public virtual void HandleCollisions()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            i++;
        }
    }

    public virtual void DetectMainActors()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (chasedTarget == null)
        {
            StartCoroutine(base.Wander());
        }
        else
        {
            if (priorityCollider)
            {
                Seek(priorityCollider.transform.position);
            }
            else
            {
                Seek(chasedTarget.gameObject.transform.position);
            }
            if (Vector3.Distance(chasedTarget.transform.position, transform.position) >= chaseRange)
            {
                chasedTarget = null;
                priorityCollider = null;
                StopAllCoroutines();
            }
        }
    }

    void FixedUpdate()
    {
        HandleCollisions();
        DetectMainActors();
    }

    public string Name()
    {
        throw new System.NotImplementedException();
    }

    public bool IsEnemyAI()
    {
        throw new System.NotImplementedException();
    }

    public float GetHealth()
    {
        return health;
    }

    public void DealDamage(Combatant opponent)
    {
        Debug.Log($"{this.gameObject.name}  dealt {damage} damage to {opponent.gameObject.name}");
        opponent.TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public void SetLastEvent(string lastEvent)
    {
        throw new System.NotImplementedException();
    }
}
