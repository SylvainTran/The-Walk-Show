using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dancer : Combatant
{
    // Start is called before the first frame update
    void Start()
    {
        isDancing = true;
        base.Start();
        FreezeAgent();
        GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        parent.GetComponent<Animator>().SetBool("isDancing", true);   
    }
}
