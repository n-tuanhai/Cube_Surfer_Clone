using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToggleRagdoll : MonoBehaviour
{
    private List<Rigidbody> ragdollBodies;
    private Rigidbody mainBody;
    void Start()
    {
        ragdollBodies = GetComponentsInChildren<Rigidbody>().ToList();
        mainBody = ragdollBodies.ElementAt(0);
        ragdollBodies.RemoveAt(0);
        ToggleRagdollStatus(false);
    }

    public void ToggleRagdollStatus(bool state)
    {
        foreach (var rb in ragdollBodies)
        {
            rb.detectCollisions = state;
            rb.isKinematic = !state;
        }

        GetComponent<Animator>().enabled = !state;
        mainBody.isKinematic = state;
        mainBody.detectCollisions = !state;
    }
    
    
}
