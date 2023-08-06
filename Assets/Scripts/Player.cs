using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Light2D light;
    private float nextAttackTime = 0f;

    public int speed = 5;
    public Transform attackPoint;
    public float attackRate = 2f;
    public float attackRange = 0.5f;
    public int attackDamage = 50;
    public LayerMask enemyLayers;
    private int currentHealth;
    public int maxHealth = 100;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        light = GetComponentInChildren<Light2D>();

        currentHealth = maxHealth;
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

    private void OnCasting(InputValue value)
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Casting");
            nextAttackTime = Time.time + 1f / attackRate;
            Attack();
        }
    }

    private void Attack()
    {
        // Detect Hit
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Da,age them
        foreach(Collider2D hit in hits) 
        {
            Debug.Log("We hit " + hit.name);
            hit.GetComponent<Enemy>().TakeDamage(attackDamage);
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
            light.gameObject.transform.SetLocalPositionAndRotation(new Vector3(0.25f, 0.5f), transform.rotation);
            attackPoint.gameObject.transform.SetLocalPositionAndRotation(new Vector3(-attackPoint.gameObject.transform.localPosition.x, attackPoint.gameObject.transform.localPosition.y), attackPoint.gameObject.transform.localRotation);
        }
        else if (movement.x > 0 && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false;
            light.gameObject.transform.SetLocalPositionAndRotation(new Vector3(-0.25f, 0.5f), transform.rotation);
            attackPoint.gameObject.transform.SetLocalPositionAndRotation(new Vector3(-attackPoint.gameObject.transform.localPosition.x, attackPoint.gameObject.transform.localPosition.y), attackPoint.gameObject.transform.localRotation);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt animation
        animator.SetTrigger("Damage");

        if (currentHealth <= 0)
        {
            // Die animation

            // Disable enemy
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("GAME OVER: YOU'RE DEAD");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
