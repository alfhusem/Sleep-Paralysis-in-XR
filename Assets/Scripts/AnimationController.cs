using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AnimationController : MonoBehaviour
{

    public Transform playerTransform;
    public Animator animator;
    public bool inPlaceAnimation;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        Vector3 positionA = transform.position;
        Vector3 positionB = playerTransform.position;
        positionA.y = 0;
        positionB.y = 0;
        // Now calculate the distance
        float distanceToPlayer = Vector3.Distance(positionA, positionB);
        
        // Start walking and face the player
        if (animator.GetBool("isWalking") || animator.GetBool("isCrawling") || animator.GetBool("isRunning")) 
        {
            // Rotate to face the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); // Keep original up orientation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5); // Smooth rotation

            if (distanceToPlayer < 1f) 
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isCrawling", false);
                animator.SetBool("isRunning", false);
            }
        }
                 
    }

}
