using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

<<<<<<< Updated upstream
public class Slayer : Bot
=======
/// <summary>
/// Lead action actor role.
/// 
/// Kills things like predators or the vampire actor. Or gets killed.
/// 
/// After the deed is done, makes a prayer and reflects on his slaying. Potentially brings a trophy with him.
/// </summary>
public class Slayer : Combatant
>>>>>>> Stashed changes
{
    public int ACTOR_SKIN;

    public void SeekPredators()
    {

<<<<<<< Updated upstream
=======
        Collider[] hitColliders = Physics.OverlapBox(transform.parent.parent.position, transform.parent.parent.transform.localScale / 2 * 32, Quaternion.identity);
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
        agent = transform.parent.parent.GetComponent<NavMeshAgent>();
        animator = transform.parent.parent.GetComponent<Animator>();
        StartCoroutine(base.Wander());
        stoppingRange = 6.10f;
        attackRange = 6.10f;
    }

    public void LateUpdate()
    {
        HandleCollisions();
>>>>>>> Stashed changes
    }

    private void Update()
    {
        
    }
}
