using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Snake : Combatant
{
    public  new void Start()
    {
        base.Start();
        sensorRange = new Vector3(30.0f, 0.0f, 30.0f);
    }

    // Update is called once per frame
    public void Update()
    {
        Navigate();
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

        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, sensorRange, Quaternion.identity);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            if (collided.gameObject == parent.gameObject || collided.gameObject.GetComponent<Terrain>() || collided.gameObject.GetComponentInChildren<Snake>())
            {
                i++;
                continue;
            }
            if (collided.GetComponentInChildren<Bot>())
            {
                chasedTarget = collided.gameObject;
                priorityCollider = chasedTarget;
                break;
            }
            i++;
        }
    }

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
        if (health <= 0.0f)
        {
            base.Die();
            return;
        }
        if (countering && combatRoutine == null)
        {
            Debug.Log($"Snake defending from douche {attacker.gameObject.name}");
            combatRoutine = StartCoroutine(LockCombatState(attackSpeed, attacker.GetComponent<Combatant>()));
        }
    }

    public override IEnumerator LockCombatState(float attackSpeed, Combatant opponent)
    {
        yield return new WaitForSeconds(attackSpeed);
        if (health <= 0)
        {
            base.Die();
            yield return null;
        }
        if (opponent == null || opponent.health <= 0.0f)
        {
            if(combatRoutine != null)
            {
                StopCoroutine(combatRoutine);
                combatRoutine = null;
            }
            animator.SetBool("isAttacking", false);
            GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.None;
            chasedTarget = null;
            priorityCollider = null;
            yield return null;
        }
        if (opponent && opponent.health > 0.0f)
        {
            DealDamage(opponent);
            animator.SetBool("isAttacking", true);
            FreezeAgent();
            GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            StartCoroutine(LockCombatState(attackSpeed, opponent));
        }
    }
}
