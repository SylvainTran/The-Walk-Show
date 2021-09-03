using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Combatant : Bot
{
    [SerializeField]
    protected float chaseRange = 20.0f;
    [SerializeField]
    protected float sight = 15.0f;

    // Start is called before the first frame update
    private void Start()
    {
        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        characterModel = GetComponent<CharacterModel>();
        animator = GetComponent<Animator>();
    }

    public void BehaviourSetup(GameWaypoint quadrantTarget)
    {
        this.quadrantTarget = quadrantTarget;
    }

    public override IEnumerator Wander()
    {
        yield return StartCoroutine(base.Wander());
    }

    /// <summary>
    /// Seek and destroy
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public override bool Seek(Vector3 target)
    {
        base.Seek(target);

        if (chasedTarget == null)
        {
            return false;
        }

        return true;
    }

    public virtual IEnumerator LockCombatState(float attackSpeed, Combatant opponent)
    {
        yield return new WaitForSeconds(attackSpeed);
        if (opponent.health > 0.0f)
        {
            DealDamage(opponent);
            StartCoroutine(LockCombatState(attackSpeed, opponent));
        }
        else
        {
            StopAllCoroutines();
            animator.SetBool("isAttacking", false);
        }
    }

    public GameObject priorityCollider = null;
    public virtual void HandleCollisions()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            i++;
        }
    }

    public virtual void DetectMainActors()
    {

    }

    // Update is called once per frame
    private void Update()
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

    void FixedUpdate()
    {
        HandleCollisions();
        DetectMainActors();
    }

    public string Name()
    {
        throw new System.NotImplementedException();
    }

    public bool IsEnemyAI()
    {
        throw new System.NotImplementedException();
    }

    public float GetHealth()
    {
        return health;
    }

    public void DealDamage(Combatant opponent)
    {
        Debug.Log($"{this.gameObject.name}  dealt {damage} damage to {opponent.gameObject.name}");
        opponent.TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    public void SetLastEvent(string lastEvent)
    {
        throw new System.NotImplementedException();
    }
}
