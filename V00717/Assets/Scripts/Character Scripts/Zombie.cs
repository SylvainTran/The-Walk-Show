using System.Collections;
using UnityEngine;

public class Zombie : Combatant
{
    private new void Start()
    {
        base.Start();
        sensorRange = new Vector3(15.0f, 0.0f, 15.0f);
    }

    public override void HandleCollisions()
    {
        // Prioritize the in-range ones
        if(priorityCollider)
        {
            return;
        }
        // TODO should we use a line of sight or collider? But the game primarily uses waypoints...
        Collider[] hitColliders = Physics.OverlapBox(parent.position, sensorRange, Quaternion.identity);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            if (collided.gameObject == parent.gameObject)
            {
                i++;
                continue;
            }

            if (collided.gameObject.GetComponentInChildren<Bot>())
            {
                if (collided.gameObject.GetComponentInChildren<Vampire>() == null)
                {
                    chasedTarget = hitColliders[i].gameObject;
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
        if (chasedTarget.GetComponent<CharacterModel>() && Vector3.Distance(target, parent.position) <= attackRange)
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
        if(chasedTarget == null || priorityCollider == null)
        {
            wanderRoutine = StartCoroutine(base.Wander());
        } else
        {
            Hunt();
        }
    }

    void FixedUpdate()
    {
        HandleCollisions();
    }
}
