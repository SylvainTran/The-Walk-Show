using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainActor : Bot
{
    /// <summary>
    /// This should be set after this actor gets cast in the Auditions.
    /// </summary>
    private int actorRole = 1;// TODO assign at creation
    public int ActorRole { get { return actorRole; } set { actorRole = value; } }
    public float safeDistance = 35.0f;

    public void Start()
    {
        base.Start();
        base.RandomizeWanderParameters();
<<<<<<< Updated upstream
=======

        quadrantSize = new Vector3(30.0f, 0.0f, 30.0f); // Get this from actual mesh/plane size
        quadrantIndex = GetComponentInParent<CharacterModel>().InQuadrant;
        if (quadrantIndex > -1)
        {
            quadrantTarget = gameController.quadrantMapper.gameWayPoints[quadrantIndex];
            gameController.quadrantMapper.GoToQuadrant(GetComponentInParent<CharacterModel>(), quadrantTarget);
        }
>>>>>>> Stashed changes
    }

    public void Update()
    {
        if (chasedTarget == null)
        {
            if(quadrantTarget == null)
            {
                return;
            }
            if (!coolDown && SeasonController.currentGameState != SeasonController.GAME_STATE.SEASON_INTRO)
            {
                StartCoroutine(base.Wander());
            }
        }
        else
        {
            if(actorRole == (int)SeasonController.ACTOR_ROLES.SLAYER)
            {
                Seek(chasedTarget.gameObject.transform.position);
                // TODO Attack until death, and chasedTarget resets to null
            }
<<<<<<< Updated upstream
            else if(actorRole != (int)SeasonController.ACTOR_ROLES.VAMPIRE)
            {
                //if(fleeingState)
                //{
                //    return;
                //}
                // Everybody else just runs away
                if(base.Flee(chasedTarget.transform.position))
                {
                    fleeingState = true;
                }
            }
=======
>>>>>>> Stashed changes
        }
        if (fleeingState)
        {
            float dist = Vector3.Distance(chasedTarget.transform.position, transform.position);
            if (dist >= safeDistance)
            {
                fleeingState = false;
                chasedTarget = null;
                animator.SetBool("isFleeing", false);
            }
        }
    }

    private void LateUpdate()
    {
        HandleCollisions();
    }

    public float collisionRadius = 30.0f;
    public void HandleCollisions()
    {
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2 * collisionRadius, Quaternion.identity);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            //Output all of the collider names
            if (hitColliders[i].GetComponent<Zombie>() || hitColliders[i].GetComponent<Snake>())
            {
                Debug.Log("Hit : " + hitColliders[i].name + i);
                chasedTarget = hitColliders[i].gameObject;
                fleeingState = true;
                animator.SetBool("isWalking", false);
                base.Flee(chasedTarget.transform.position);
                animator.SetBool("isFleeing", true);
                break;
            }
            //Increase the number of Colliders in the array
            i++;
        }
    }
}
