using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTracker : MonoBehaviour
{
    [SerializeField] private Transform target;
    public Transform Target { get { return target; } }
    public Vector3 offset;
    public float smoothTime;
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(this.target == null)
        {
            return;
        }

        FollowTarget();
        LookAt();
    }

    /// <summary>
    /// Sets a target to look at and follow
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        this.target = target;
        offset = transform.position - target.position;
    }

    public void LookAt()
    {
        transform.LookAt(target);
    }

    public void FollowTarget()
    {
        Vector3 targetPosition = this.target.position + this.offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
