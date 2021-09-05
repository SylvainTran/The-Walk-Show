using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slayer : Combatant
{
    public int ACTOR_SKIN;

    public override void HandleCollisions()
    {
        if (chasedTarget || priorityCollider) return;

        Collider[] hitColliders = Physics.OverlapBox(parent.position, parent.transform.localScale / 2 * 32, Quaternion.identity);
        int i = 0;
        
        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            if (collided.gameObject == this.gameObject) continue;
            if(collided.GetComponentInChildren<Snake>())
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
        stoppingRange = 2.90f;
        attackRange = 3.0f;
    }

    public void LateUpdate()
    {
        HandleCollisions();
    }

    public void Update()
    {
        if(chasedTarget != null)
        {
            // TODO funny observation about the hat being the new transform reference/point of origin => What about it being displaced
            float dist = Vector3.Distance(chasedTarget.transform.position, transform.parent.parent.transform.position);
            if (dist >= chaseRange)
            {
                chasedTarget = null;
                priorityCollider = null;
                return;
            } else if(dist <= stoppingRange + 1.0f) //  TODO + renderer.bounds.extents.z
            {
                FreezeAgent();
                Snake opponent = chasedTarget.GetComponentInChildren<Snake>();
                if (opponent && dist <= attackRange + 1.0f)
                {
                    // Play animation and attack
                    if (!isAttacking)
                    {
                        isAttacking = true;
                        parent.LookAt(chasedTarget.transform);
                        opponent.IsAttacked = true;
                        animator.SetBool("isAttacking", true);
                        StartCoroutine(LockCombatState(attackSpeed, opponent));
                    }
                    return;
                }
            }
            if (priorityCollider)
            {
                base.Seek(priorityCollider.transform.position);
            }
            else
            {
                base.Seek(chasedTarget.gameObject.transform.position);
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
            Debug.Log($"{GetComponentInParent<CharacterModel>().NickName} wacked at a snake!");
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
