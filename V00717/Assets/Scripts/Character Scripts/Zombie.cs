using System.Collections;
using UnityEngine;

public class Zombie : Bot, ICombatant
{
    [SerializeField]
    private float chaseRange = 20.0f;
    [SerializeField]
    private float sight = 15.0f;

    // Start is called before the first frame update
    private void Start()
    {
        base.Start();
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
        if (chasedTarget.GetComponent<CharacterModel>() && Vector3.Distance(target, transform.position) <= attackRange)
        {
            // Play animation and attack
            animator.SetBool("isAttacking", true);
            CharacterModel opponent = chasedTarget.GetComponent<CharacterModel>();
            StartCoroutine(LockCombatState(attackSpeed, opponent));
        }
        
        return true;
    }

    public IEnumerator LockCombatState(float attackSpeed, CharacterModel opponent)
    {
        yield return new WaitForSeconds(attackSpeed);
        if (opponent.Health > 0.0f)
        {
            DealDamage(opponent);
            StartCoroutine(LockCombatState(attackSpeed, opponent));
        } else
        {
            StopAllCoroutines();
            animator.SetBool("isAttacking", false);
        }
    }

    public GameObject priorityCollider = null;
    public void HandleCollisions()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            if (collided.gameObject.GetComponent<MainActor>())
            {
                if(collided.gameObject.GetComponent<MainActor>().ActorRole != (int)SeasonController.ACTOR_ROLES.VAMPIRE)
                {
                    chasedTarget = hitColliders[i].gameObject;
                    priorityCollider = chasedTarget; // TODO reset to null if dead or out of range
                    break;
                }
            }
            i++;
        }
    }

    public void DetectMainActors()
    {
        // Prioritize the in-range ones
        if(priorityCollider)
        {
            return;
        }
        // TODO should we use a line of sight or collider? But the game primarily uses waypoints...
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale * sight, Quaternion.identity);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            if (collided.gameObject.GetComponent<MainActor>())
            {
                if (collided.gameObject.GetComponent<MainActor>().ActorRole != (int)SeasonController.ACTOR_ROLES.VAMPIRE)
                {
                    chasedTarget = hitColliders[i].gameObject;
                    break;
                }
            }
            i++;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(chasedTarget == null)
        {
            StartCoroutine(base.Wander());
        } else
        {
            if(priorityCollider)
            {
                Seek(priorityCollider.transform.position);
            } else
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
        throw new System.NotImplementedException();
    }

    public void DealDamage(ICombatant opponent)
    {
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
