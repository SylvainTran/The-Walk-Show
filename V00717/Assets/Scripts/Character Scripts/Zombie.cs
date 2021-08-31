using System.Collections;
using UnityEngine;

public class Zombie : Bot, ICombatant
{
    /// <summary>
    /// The chased actor or npc by this zombie.
    /// </summary>
    public GameObject chasedTarget;
    public float attackRange = 3.0f;
    public Animator animator;

    public float health = 100.0f;
    public float damage = 1.0f;
    public float attackSpeed = 1.0f; // Delay in s before next attack

    // Start is called before the first frame update
    private void Start()
    {
        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        characterModel = GetComponent<CharacterModel>();
        quadrantSize = new Vector3(30.0f, 0.0f, 30.0f);
        animator = GetComponent<Animator>();
    }

    public void BehaviourSetup(GameWaypoint quadrantTarget)
    {
        this.quadrantTarget = quadrantTarget;
    }

    public override void Wander()
    {
        base.Wander();
    }

    /// <summary>
    /// Seek and destroy
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public override bool Seek(Vector3 target)
    {
        if(chasedTarget == null)
        {
            return false;
        }
        base.Seek(target);

        if (chasedTarget.GetComponent<CharacterModel>() && Vector3.Distance(transform.position, target) <= attackRange)
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

    public void HandleCollisions()
    {
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            //Output all of the collider names
            if(hitColliders[i].CompareTag("CharacterBot"))
            {
                Debug.Log("Hit : " + hitColliders[i].name + i);
                chasedTarget = hitColliders[i].gameObject;
            }
            //Increase the number of Colliders in the array
            i++;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(chasedTarget == null)
        {
            Wander();
        } else
        {
            Seek(chasedTarget.gameObject.transform.position);
        }
    }

    void FixedUpdate()
    {
        // HandleCollisions();
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
