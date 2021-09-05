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

    public new void Start()
    {
        base.Start();
        base.RandomizeWanderParameters();

        quadrantSize = new Vector3(30.0f, 0.0f, 30.0f); // Get this from actual mesh/plane size
        quadrantIndex = parent.GetComponent<CharacterModel>().InQuadrant;
        if (quadrantIndex > -1)
        {
            quadrantTarget = gameController.quadrantMapper.gameWayPoints[quadrantIndex];
            gameController.quadrantMapper.GoToQuadrant(parent.GetComponent<CharacterModel>(), quadrantTarget);
        }
        collisionRadius = new Vector3(25.0f, 0.0f, 25.0f);
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
            }
        }
        if (fleeingState)
        {
            float dist = Vector3.Distance(chasedTarget.transform.position, parent.position);
            if (dist >= safeDistance)
            {
                fleeingState = false;
                chasedTarget = null;
            }
        }
    }

    private void LateUpdate()
    {
        HandleCollisions();
    }

    public Vector3 collisionRadius;
    public void HandleCollisions()
    {
        if (chasedTarget) return;
        //Use the OverlapBox to detect if there are any other colliders within this box area.
        //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
        Collider[] hitColliders = Physics.OverlapBox(parent.position, collisionRadius, Quaternion.identity);
        int i = 0;
        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            //Output all of the collider names
            if (hitColliders[i].GetComponent<Zombie>() || hitColliders[i].GetComponent<Snake>())
            {
                Debug.Log("Predator detected : " + hitColliders[i].name + i);
                chasedTarget = hitColliders[i].gameObject;
                fleeingState = true;
                animator.SetBool("isFleeing", true);
            }
            //Increase the number of Colliders in the array
            i++;
        }
    }
}
