using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObjects : MonoBehaviour, InterfaceHitableObj
{
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component or add one if it doesn't exist
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    public void Hit(RaycastHit hit, int damage = 1)
    {
        // Apply a force to the object in the opposite direction of the hit normal
        rb.AddForce(-hit.normal * 100f);
    }
}
