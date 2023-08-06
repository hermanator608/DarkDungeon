using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    private Animator animator;
    private AIPath aiPath;

    private float nextAttackTime = 0f;

    public float attackRange;
    public LayerMask playerLayer;
    public int speed = 5;
    public Transform attackPoint;
    public float attackRate = 1f;
    public int attackDamage = 50;
    public bool isMelee = true;

    public bool isFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
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
            Debug.Log("Turning right - " + aiPath.velocity);
            transform.localScale = new Vector3(1f, 1f, 1f);
            attackPoint.gameObject.transform.localPosition = new Vector3(-attackPoint.gameObject.transform.localPosition.x, attackPoint.gameObject.transform.localPosition.y);
            isFacingRight = true;
        }
        else if (aiPath.velocity.x <= -0.01f && isFacingRight)
        {
            Debug.Log("Turning left - " + aiPath.velocity);
            transform.localScale = new Vector3(-1f, 1f, 1f);
            attackPoint.gameObject.transform.localPosition = new Vector3(-attackPoint.gameObject.transform.localPosition.x, attackPoint.gameObject.transform.localPosition.y);
            Debug.Log("New transform - " + attackPoint.gameObject.transform.localPosition);
            isFacingRight = false;
        }

        // Attach if at end of path
        if (aiPath.reachedEndOfPath && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    void Attack()
    {
        nextAttackTime = Time.time + 1f / attackRate;
        int randomAttack = Random.Range(0, 2);

        if (randomAttack == 0)
        {
            animator.SetTrigger("Jab");
        } else
        {
            animator.SetTrigger("Slice");
        }

        if (isMelee)
        {
            // Detect Hit
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

            // Damage them
            foreach (Collider2D hit in hits)
            {
                Debug.Log("Enemy hit " + hit.name);
                hit.GetComponent<Player>().TakeDamage(attackDamage);
            }
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
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
