using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

<<<<<<< Updated upstream
public class GraveDigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
=======
/// <summary>
/// Garbage collector actor role.
/// Picks up dead actors (chasedTarget) and puts them in the nearest graveyard (or creates one if none).
/// Can also burn them instead.
/// 
/// Once done, sits on a chair in front of the mound and philosophizes about life and death. (Observations about decay etc.)
/// 
/// </summary>
public class GraveDigger : Bot
{
    public int ACTOR_SKIN;
    public float digRange = 3.0f;
    public List<Bot> targets;

    public void HandleCollisions()
    {
        if (chasedTarget) return;

        Collider[] hitColliders = Physics.OverlapBox(transform.parent.parent.position, transform.parent.parent.transform.localScale / 2 * 32, Quaternion.identity);
        int i = 0;

        while (i < hitColliders.Length)
        {
            Collider collided = hitColliders[i];
            if (collided != transform.parent.parent.gameObject)
            {
                Bot bot = collided.GetComponent<Bot>();
                if (bot && bot.health <= 0)
                {
                    chasedTarget = collided.gameObject;
                    break;
                }
            }
            i++;
        }
    }

    public override void Start()
    {
        agent = transform.parent.parent.GetComponent<NavMeshAgent>();
        animator = transform.parent.parent.GetComponent<Animator>();
        characterModel = GetComponentInParent<CharacterModel>();
        gameController = FindObjectOfType<GameController>();
        StartCoroutine(base.Wander());
        stoppingRange = 6.10f;
    }

    public void LateUpdate()
    {
        HandleCollisions();
    }

    public void Update()
    {
        if (chasedTarget != null)
        {
            float dist = Vector3.Distance(chasedTarget.transform.position, transform.parent.parent.transform.position);
            if (dist >= digRange)
            {
                chasedTarget = null;
                return;
            }
            else if (dist <= stoppingRange + 1.0f)
            {
                FreezeAgent();
                if (dist <= digRange + 1.0f)
                {
                    // Create a new "Death Visa": dig, move corpse, then sit on a chair when leave, build grave marker
                    if (!isDigging)
                    {
                        isDigging = true;
                        //animator.SetBool("isAttacking", true);
                        transform.parent.parent.transform.LookAt(chasedTarget.transform);
                        StartCoroutine(LockDigState(attackSpeed, chasedTarget));
                    }
                    return;
                }
            }
            else
            {
                base.Seek(chasedTarget.gameObject.transform.position);
            }
        }
        else
        {
            BehaviourCoolDown(false);
            StartCoroutine(base.Wander());
        }
    }

    public bool AnimationCompleted()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9;
    }

    public bool isDigging = false;
    public IEnumerator LockDigState(float digSpeed, GameObject corpse)
    {
        yield return new WaitForEndOfFrame();
        isDigging = true;

        Debug.Log($"{GetComponentInParent<CharacterModel>().NickName} started to dig some corpse!");
        FreezeAgent();
        corpse.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        animator.SetBool("isDigging", true);
        yield return new WaitUntil(AnimationCompleted);
        animator.SetBool("isDigging", false);
        StartCoroutine(MoveCorpse(1.0f));
    }

    public IEnumerator MoveCorpse(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log($"{GetComponentInParent<CharacterModel>().NickName} started moving the corpse.");
    }

>>>>>>> Stashed changes
}
