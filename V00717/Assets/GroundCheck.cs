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
            NavMeshAgent nav = this.gameObject.AddComponent<NavMeshAgent>();
            nav.agentTypeID = 0;
            nav.radius = 0.3f;
        }
    }
}
