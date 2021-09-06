using System.Collections;
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
    public override void TakeDamage(GameObject attacker, float m_damage)
    {
        health -= m_damage;
        parent.transform.LookAt(attacker.transform);
        countering = true; // Temporary fix => could counter over several frames in future time
        if(countering && combatRoutine == null)
        {
            Debug.Log($"Snake defending from douche {attacker.gameObject.name}");
            combatRoutine = StartCoroutine(LockCombatState(attackSpeed, attacker.GetComponent<Combatant>()));
        }
        if (health <= 0.0f)
        {
            base.Die();
        }
    }

    public override IEnumerator LockCombatState(float attackSpeed, Combatant opponent)
    {
        yield return new WaitForSeconds(attackSpeed);
        if (health <= 0)
        {
            base.Die();
        }
        if (opponent.health > 0.0f)
        {
            DealDamage(opponent);
            animator.SetBool("isAttacking", true);
            StartCoroutine(LockCombatState(attackSpeed, opponent));
        }
        else
        {
            StopAllCoroutines();
            animator.SetBool("isAttacking", false);
        }
    }
}
