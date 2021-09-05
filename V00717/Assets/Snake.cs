using UnityEngine;
using UnityEngine.AI;

public class Snake : Combatant
{
    Coroutine wanderRoutine = null;
    
    public  new void Start()
    {
        base.Start();
        characterModel = GetComponent<CharacterModel>();
        animator = GetComponent<Animator>();
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
        if (chasedTarget ==  null || priorityCollider == null)
        {
            if(wanderRoutine == null)
            {
                wanderRoutine = StartCoroutine(base.Wander());
            }
        }
        else
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
        if(IsAttacked)
        {
            FreezeAgent();
            //GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
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
}
