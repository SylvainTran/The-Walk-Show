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
    public new Renderer renderer; // for the bounds
    private bool isAttacked = false;
    public bool IsAttacked { get { return isAttacked; } set { isAttacked = true; } }
    public bool isAttacking = false;
    public Vector3 sensorRange = Vector3.zero;
    public Coroutine combatRoutine;
    public Coroutine wanderRoutine;
    public bool countering;

    // Start is called before the first frame update
    private new void Start()
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
        if (!agent || !agent.isOnNavMesh || coolDown)
        {
            yield return null;
        }
        NavMeshPath plannedPath = new NavMeshPath();
        bool success = false;
        if (!success) // TODO make while condition here until success is true => I think there is an issue with the navmesh agent height?
        {
            wanderTarget = RandomizeWanderParameters();
            NavMesh.CalculatePath(parent.position, wanderTarget, NavMesh.AllAreas, plannedPath);
            success = plannedPath.status != NavMeshPathStatus.PathInvalid;
        }
        if(success)
        {
            agent.SetPath(plannedPath);
        } else
        {
            // fall back => TODO also add return to nearest game way point if this also fails, but should not. At any rate, we should clamp the max wander zone to the original instantiation vector
            Vector3 randomPoint = parent.position + Random.insideUnitSphere * wanderRadius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                wanderTarget = hit.position;
                Seek(wanderTarget);
            }
        }
        BehaviourCoolDown(true);
        animator.SetBool("isWalking", true);
        yield return new WaitUntil(ArrivedAtDestination);
        BehaviourCoolDown(false);
        if(wanderRoutine != null)
            StopCoroutine(wanderRoutine);
        wanderRoutine = null;

        if (chasedTarget == null && !coolDown)
        {
            wanderRoutine = StartCoroutine(Wander());
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
            localWander = new Vector3(Random.Range(-wanderRadius, wanderRadius), 0.0f, Random.Range(-wanderRadius, wanderRadius));
            localWander *= Random.Range(-wanderJitter, wanderJitter);
            localWander.Normalize();
            wanderTarget = transform.position + transform.TransformDirection(localWander);
            wanderDistance = Random.Range(15, 45);
            wanderTarget += new Vector3(wanderDistance, 0.0f, wanderDistance);
            if (renderer != null)
            {
                radius = renderer.bounds.extents.magnitude;
            }
            wanderTarget += new Vector3(0.0f, t_height + 0.5f, 0.0f); // TODO check if this is correct: y => y + t_height + radius
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

    public virtual void Hunt()
    {
        if (wanderRoutine != null)
            StopCoroutine(wanderRoutine);
        wanderRoutine = null;
        // TODO funny observation about the hat being the new transform reference/point of origin => What about it being displaced
        float dist = Vector3.Distance(chasedTarget.transform.position, parent.position);
        if (dist >= chaseRange)
        {
            chasedTarget = null;
            priorityCollider = null;
            return;
        }
        else if (dist <= sensorRange.magnitude) //  TODO + renderer.bounds.extents.z
        {
            Combatant opponent = chasedTarget.GetComponentInChildren<Combatant>();
            if (opponent && dist <= attackRange)
            {
                // Play animation and attack
                if (!isAttacking)
                {
                    isAttacking = true;
                    FreezeAgent();
                    parent.LookAt(chasedTarget.transform);
                    animator.SetBool("isAttacking", true);
                    animator.SetBool("isWalking", false);
                    combatRoutine = StartCoroutine(LockCombatState(attackSpeed, opponent));
                }
                return;
            }
        }
        if (dist >= stoppingRange)
        {
            if (priorityCollider)
            {
                base.Seek(priorityCollider.transform.position);
            }
            else
            {
                base.Seek(chasedTarget.gameObject.transform.position);
            }
            animator.SetBool("isWalking", true);
        }
    }

    public void Navigate()
    {
        if (agent == null)
        {
            agent = parent.GetComponent<NavMeshAgent>();
            if (agent == null)
                return;
        }
        if (IsAttacked)
        {
            FreezeAgent();
            StopCoroutine(wanderRoutine);
            wanderRoutine = null;
            parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            countering = true;
            return;
        }
        if (chasedTarget == null || priorityCollider == null)
        {
            if (wanderRoutine == null)
            {
                wanderRoutine = StartCoroutine(base.Wander());
            }
        }
        else
        {
            Hunt();
        }
    }

    public virtual IEnumerator LockCombatState(float attackSpeed, Combatant opponent)
    {
        yield return new WaitForSeconds(attackSpeed);
        if (health <= 0)
        {
            Die();
            yield return null;
        }
        if (opponent == null || opponent.health <= 0.0f)
        {
            if (combatRoutine != null)
            {
                StopCoroutine(combatRoutine);
                combatRoutine = null;
            }
            animator.SetBool("isAttacking", false);
            isAttacking = false;
            chasedTarget = null;
            priorityCollider = null;
            StartCoroutine(ResetAgentIsStopped(0.0f));
            base.Wander();
            yield return null;
        }
        if (opponent && opponent.health > 0.0f)
        {
            DealDamage(opponent);
            animator.SetBool("isAttacking", true);
            combatRoutine = StartCoroutine(LockCombatState(attackSpeed, opponent));
        }
        else
        {
            if(combatRoutine != null)
                StopCoroutine(combatRoutine);
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
        opponent.TakeDamage(this.gameObject, damage);
    }

    public virtual void TakeDamage(GameObject attacker, float m_damage)
    {
        isAttacked = true;
        health -= m_damage;
        if (health <= 0.0f)
        {
            Die();
            return;
        }
        parent.transform.LookAt(attacker.transform);
        StartCoroutine(LockCombatState(attackSpeed, attacker.GetComponent<Combatant>()));
    }

    public void SetLastEvent(string lastEvent)
    {
        throw new System.NotImplementedException();
    }
}
