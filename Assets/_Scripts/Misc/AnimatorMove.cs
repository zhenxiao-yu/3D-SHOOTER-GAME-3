using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorMove : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        // Get the Animator component attached to this GameObject
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorMove()
    {
        // Check if the Animator component is null
        if (anim == null)
            return;

        // Make the enemy move along with its Animator component
        // Adjust the parent object's position using the delta position of the Animator
        transform.parent.position += anim.deltaPosition;
    }
}
