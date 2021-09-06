using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundCheck : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Collider collided = collision.collider;
        Debug.Log($"Collision with {collided.gameObject.name} from {this.gameObject.name}");
        if (collided.CompareTag("Ground"))
        {
            Debug.Log("Touched the ground - disabling physics and re-enabling its navmesh component");
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().isKinematic = true;
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if(agent == null)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
                agent.agentTypeID = 0;
                agent.radius = 0.1f;
                if (GetComponentInChildren<MainActor>() || GetComponentInChildren<Zombie>())
                {
                    agent.baseOffset = 2.1f;
                }
                agent.ResetPath();
            }
        }
    }
}
