using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveDigger : Bot
{
    public Vector3 sensorRange = Vector3.zero;
    public bool isDigging = false;

    public void HandleCollisions()
    {
        if (chasedTarget) return;

        Collider[] hitColliders = Physics.OverlapBox(parent.position, sensorRange, Quaternion.identity);
        int i = 0;

        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            if (collided.gameObject == this.gameObject)
            {
                i++;
                continue;
            }

            if (collided.GetComponentInChildren<Bot>())
            {
                chasedTarget = collided.gameObject;
                break;
            }
            i++;
        }
    }

    public override void Start()
    {
        base.Start();
        if(agent.isOnNavMesh)
        {
            StartCoroutine(base.Wander());
        }
        sensorRange = new Vector3(15.0f, 1.0f, 15.0f);
    }

    public void LateUpdate()
    {
        HandleCollisions();
    }

    public void Work()
    {
        float dist = Vector3.Distance(chasedTarget.transform.position, parent.position);
        if (dist <= sensorRange.magnitude)
        {
            Bot corpse = chasedTarget.GetComponentInChildren<Bot>();
            if (corpse.health > 0f) return;
            if (corpse && dist <= attackRange)
            {
                // Play animation and attack
                if (!isDigging)
                {
                    isDigging = true;
                    FreezeAgent();
                    parent.LookAt(chasedTarget.transform);
                    animator.SetBool("isDigging", true);
                    animator.SetBool("isWalking", false);
                }
                return;
            }
        }
    }

    public void Update()
    {
        if (chasedTarget != null)
        {
            Work();
        }
        else
        {
            StartCoroutine(base.Wander());
        }
    }

    public bool AnimationCompleted()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9;
    }
}
