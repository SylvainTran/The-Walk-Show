using UnityEngine;
using UnityEngine.AI;

public class Snake : Combatant
{
    Coroutine wanderRoutine = null;
    Coroutine combatRoutine = null;
    public bool countering;
    
    public  new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        if (agent == null)
        {
            agent = GetComponentInParent<NavMeshAgent>();
            if (agent == null)
                return;
        }
        if (IsAttacked)
        {
            FreezeAgent();
            GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            countering = true;
            return;
        }
        if (chasedTarget ==  null || priorityCollider == null)
        {
            if(wanderRoutine == null)
            {
                wanderRoutine = StartCoroutine(base.Wander());
            }
        }
        else
        {
            //StopAllCoroutines();
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
                StartCoroutine(base.Wander());
            }
        }
    }

    public void LateUpdate()
    {
        HandleCollisions();
    }

    public override void HandleCollisions()
    {
        if (agent == null)
        {
            agent = GetComponentInParent<NavMeshAgent>();
            if(agent == null)
                return;
        }
        if (chasedTarget || priorityCollider) return;

        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, parent.GetChild(1).transform.GetComponent<Renderer>().bounds.extents, Quaternion.identity);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            if (collided.gameObject == this.gameObject) continue;

            if (collided.GetComponentInChildren<Bot>())
            {
                chasedTarget = collided.gameObject;
                priorityCollider = chasedTarget;
                break;
            }
            i++;
        }
    }
    public bool isAttacking = false;
    public override bool Seek(Vector3 target)
    {
        base.Seek(target);
        if (!chasedTarget) return false;

        if (chasedTarget.GetComponentInChildren<Bot>() && Vector3.Distance(target, transform.position) <= attackRange)
        {
            // Play animation and attack
            if (!isAttacking)
            {
                isAttacking = true;
                //animator.SetBool("isAttacking", true);
                Combatant opponent = chasedTarget.GetComponentInChildren<Combatant>();
                StartCoroutine(LockCombatState(attackSpeed, opponent));
            }
            return true;
        }
        return false;
    }
    public override void TakeDamage(GameObject attacker, float damage)
    {
        health -= damage;
        parent.transform.LookAt(attacker.transform);
        countering = true;
        if(countering && combatRoutine == null)
        {
            Debug.Log($"Snake defending from douche {attacker.gameObject.name}");
            combatRoutine = StartCoroutine(base.LockCombatState(attackSpeed, attacker.GetComponent<Combatant>()));
        }
        if (health <= 0.0f)
        {
            Die();
        }
    }
}
