using UnityEngine;
using UnityEngine.AI;

public class Snake : Combatant
{
    public void Start()
    {
        base.Start();
        StartCoroutine(base.Wander());
    }

    // Update is called once per frame
    public void Update()
    {
        if(!chasedTarget)
        {
            StartCoroutine(base.Wander());
        }
        else if (chasedTarget || priorityCollider)
        {
            StopAllCoroutines();
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
        if (chasedTarget || priorityCollider) return;

        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
        int i = 0;

        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            if (collided.GetComponent<Bot>())
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

        if (chasedTarget.GetComponent<Snake>() && Vector3.Distance(target, transform.position) <= attackRange)
        {
            // Play animation and attack
            if (!isAttacking)
            {
                isAttacking = true;
                //animator.SetBool("isAttacking", true);
                Combatant opponent = chasedTarget.GetComponent<Combatant>();
                StartCoroutine(LockCombatState(attackSpeed, opponent));
            }
            return true;
        }
        return false;
    }
}
