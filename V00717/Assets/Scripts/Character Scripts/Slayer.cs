using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slayer : Combatant
{
    public int ACTOR_SKIN;

    public override void HandleCollisions()
    {
        if (chasedTarget || priorityCollider) return;

        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
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
    }

    public void LateUpdate()
    {
        HandleCollisions();
    }

    public void Update()
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

    public override bool Seek(Vector3 target)
    {
        base.Seek(target);

        if (!chasedTarget) return false;

        if (chasedTarget.GetComponent<Snake>() && Vector3.Distance(target, transform.position) <= attackRange)
        {
            // Play animation and attack
            if(!isAttacking)
            {
                isAttacking = true;
                //animator.SetBool("isAttacking", true);
                Snake opponent = chasedTarget.GetComponent<Snake>();
                StartCoroutine(LockCombatState(attackSpeed, opponent));
            }
            return true;
        }
        return false;
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
