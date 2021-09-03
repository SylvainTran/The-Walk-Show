using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slayer : Combatant
{
    public int ACTOR_SKIN;

    public override void HandleCollisions()
    {
        if (chasedTarget || priorityCollider) return;

        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2 * 32, Quaternion.identity);
        int i = 0;
        
        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            if(collided.GetComponent<Snake>())
            {
                chasedTarget = collided.gameObject;
                priorityCollider = chasedTarget;
                break;
            }
            i++;
        }
    }

    public void SetLastEvent(string lastEvent)
    {
        throw new System.NotImplementedException();
    }

    public override void Start()
    {
        base.Start();
        StartCoroutine(base.Wander());
    }

    public void LateUpdate()
    {
        HandleCollisions();
    }

    public void Update()
    {
        if(chasedTarget != null)
        {
            if (priorityCollider)
            {
                base.Seek(priorityCollider.transform.position);
            }
            else
            {
                base.Seek(chasedTarget.gameObject.transform.position);
            }
            float dist = Vector3.Distance(chasedTarget.transform.position, transform.position);
            if (dist >= chaseRange)
            {
                chasedTarget = null;
                priorityCollider = null;
                StopAllCoroutines();
                StartCoroutine(base.Wander());
            } else if(dist <= stoppingRange)
            {
                FreezeAgent();
                if (chasedTarget.GetComponent<Snake>() && dist <= attackRange)
                {
                    // Play animation and attack
                    if (!isAttacking)
                    {
                        isAttacking = true;
                        //animator.SetBool("isAttacking", true);
                        transform.LookAt(chasedTarget.transform);
                        Snake opponent = chasedTarget.GetComponent<Snake>();
                        StartCoroutine(LockCombatState(attackSpeed, opponent));
                    }
                }
            }
        } else
        {
            StartCoroutine(base.Wander());
        }
    }

    public void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    public void DealDamage(Combatant opponent)
    {
        base.DealDamage(opponent);
    }

    public float GetHealth()
    {
        return health;
    }

    public bool IsEnemyAI()
    {
        return false;
    }

    public bool isAttacking = false;
    public new IEnumerator LockCombatState(float attackSpeed, Combatant opponent)
    {
        yield return new WaitForSeconds(attackSpeed);
        if (opponent.health > 0.0f)
        {
            base.DealDamage(opponent);
            Debug.Log($"{GetComponent<CharacterModel>().NickName} wacked at a snake!");
            //animator.SetBool("isAttacking", true);
            StartCoroutine(LockCombatState(attackSpeed, opponent));
        }
        else
        {
            StopAllCoroutines();
            //animator.SetBool("isAttacking", false);
        }
    }

    public string Name()
    {
        throw new System.NotImplementedException();
    }
}
