using System.Collections;
using UnityEngine;

public class Zombie : Combatant
{

    public override void DetectMainActors()
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

    public override void HandleCollisions()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
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
                    priorityCollider = chasedTarget; // TODO reset to null if dead or out of range
                    break;
                }
            }
            i++;
        }
    }

    public override bool Seek(Vector3 target)
    {
        base.Seek(target);

        if (!chasedTarget) return false;
        if (chasedTarget.GetComponent<CharacterModel>() && Vector3.Distance(target, transform.position) <= attackRange)
        {
            Combatant opponent = chasedTarget.GetComponent<Combatant>();
            StartCoroutine(LockCombatState(attackSpeed, opponent));
            return true;
        }
        return false;
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
}
