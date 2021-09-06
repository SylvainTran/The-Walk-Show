using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slayer : Combatant
{
    public int ACTOR_SKIN;
    public Vector3 sensorRange = Vector3.zero;
    public Coroutine combatRoutine;
    public Coroutine wanderRoutine;
   
    public override void HandleCollisions()
    {
        if (chasedTarget || priorityCollider) return;

        Collider[] hitColliders = Physics.OverlapBox(parent.position, sensorRange, Quaternion.identity);
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
        damage = 20.0f;
        stoppingRange = 2.9f;
        attackRange = stoppingRange + 1.0f;
        sensorRange = new Vector3(15.0f, 1.0f, 15.0f);
    }

    public void LateUpdate()
    {
        HandleCollisions();
    }

    public void Update()
    {
        if(chasedTarget != null)
        {
            if (wanderRoutine != null)
                StopCoroutine(wanderRoutine);
            wanderRoutine = null;
            // TODO funny observation about the hat being the new transform reference/point of origin => What about it being displaced
            float dist = Vector3.Distance(chasedTarget.transform.position, transform.parent.parent.transform.position);
            if (dist >= chaseRange)
            {
                chasedTarget = null;
                priorityCollider = null;
                return;
            } else if(dist <= sensorRange.magnitude) //  TODO + renderer.bounds.extents.z
            {
                Snake opponent = chasedTarget.GetComponentInChildren<Snake>();
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
            if(dist >= stoppingRange)
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
        else
        {
            if (wanderRoutine == null)
            {
                wanderRoutine = StartCoroutine(base.Wander());
            }
        }
    }

    public bool AnimationCompleted()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9;
    }

    public bool isAttacking = false;
    public override IEnumerator LockCombatState(float attackSpeed, Combatant opponent)
    {
        yield return new WaitForSeconds(attackSpeed);
        if(health <= 0)
        {
            Die();
        }
        if (opponent.health > 0.0f)
        {
            base.DealDamage(opponent);
            Debug.Log($"{GetComponentInParent<CharacterModel>().NickName} wacked at a snake! Snake HP: {opponent.health}");
            yield return new WaitUntil(AnimationCompleted);
            StartCoroutine(LockCombatState(attackSpeed, opponent));
        }
        else
        {
            isAttacking = false;
            chasedTarget = null;
            priorityCollider = null;
            animator.SetBool("isAttacking", false);
            StartCoroutine(ResetAgentIsStopped(0.0f));
            StopCoroutine(combatRoutine);
            base.Wander();
        }
    }
}
