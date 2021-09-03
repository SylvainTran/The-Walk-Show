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

    public override void Start()
    {
        base.Start();
        StartCoroutine(base.Wander());
        stoppingRange = 6.10f;
        attackRange = 6.10f;
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
            } else if(dist <= stoppingRange + 1.0f) //  TODO + renderer.bounds.extents.z
            {
                FreezeAgent();
                if (chasedTarget.GetComponent<Snake>() && dist <= attackRange + 1.0f)
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

    public bool AnimationCompleted()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9;
    }

    public bool isAttacking = false;
    public override IEnumerator LockCombatState(float attackSpeed, Combatant opponent)
    {
        yield return new WaitForEndOfFrame();
        if (opponent.health > 0.0f)
        {
            base.DealDamage(opponent);
            Debug.Log($"{GetComponent<CharacterModel>().NickName} wacked at a snake!");
            FreezeAgent();
            opponent.GetComponent<Snake>().FreezeAgent();
            opponent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            animator.SetBool("isAttacking", true);
            yield return new WaitUntil(AnimationCompleted);
            StartCoroutine(LockCombatState(attackSpeed, opponent));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ResetAgentIsStopped(1.0f));
            animator.SetBool("isAttacking", false);
        }
    }
}
