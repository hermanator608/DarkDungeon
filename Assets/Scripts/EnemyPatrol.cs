using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    private Animator animator;
    private AIPath aiPath;
    private bool isFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        aiPath = GetComponent<AIPath>();
    }

    void FixedUpdate()
    {
        // Update walking animation
        if (aiPath.velocity != null && aiPath.velocity != Vector3.zero)
        {
            animator.SetBool("IsWalking", true);
        } else
        {
            animator.SetBool("IsWalking", false);
        }

        // Rotate Enemy
        if (aiPath.velocity.x >= 0.01f && !isFacingRight)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);

            isFacingRight = true;
        }
        else if (aiPath.velocity.x <= -0.01f && isFacingRight)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            isFacingRight = false;
        }
    }
}
