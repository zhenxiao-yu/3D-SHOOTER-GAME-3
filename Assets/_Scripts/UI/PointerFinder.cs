using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerFinder : MonoBehaviour
{
    public float radius = 1.0f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
