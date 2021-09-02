using UnityEngine;
using UnityEngine.AI;

public class Snake : Combatant
{
    Animator animator;
    NavMeshAgent agent;

    public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public void Update()
    {
        StartCoroutine(base.Wander());
    }
}
