using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public int speed = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void OnMovement(InputValue value)
    {
        movement = value.Get<Vector2>();

        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetBool("IsWalking", true);
        } 
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void FixedUpdate()
    {
        HandleFlip();

        if (movement != null && (movement.x != 0 || movement.y != 0))
        {
            // Variant 1
            // rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

            // Varaint 2 
            rb.velocity = movement * speed;

            // Variant 3
            //rb.AddForce(movement * speed);
        }
    }

    private void HandleFlip ()
    {
        if (movement.x < 0 && !spriteRenderer.flipX)
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0 && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false;
        }
    }
}
