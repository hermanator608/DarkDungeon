using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private int currentHealth;
    private Animator animator;
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private bool isAttacking = false;
    private bool isFacingRight = true;
    private bool startedAttacking = false;

    public int maxHealth = 2;
    public float attackRange;
    public LayerMask playerLayer;
    public int speed = 5;
    public Transform attackPoint;
    public float attackRate = 1f;
    public int attackDamage = 1;
    public bool isMelee = true;
    public int attackMinDistance = 4;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        attackPoint.SetParent(gameObject.transform);
    }

    void FixedUpdate()
    {
        // Calculate the distance between AI and the player
        if (aiPath.remainingDistance >= attackMinDistance && !startedAttacking)
        {
            aiPath.canMove = false;
            return;
        }
        else
        {
            aiPath.canMove = true;
            startedAttacking = true;
        }

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
            attackPoint.transform.localScale = new Vector3(-1f, 1f, 1f);
            isFacingRight = true;
        }
        else if (aiPath.velocity.x <= -0.01f && isFacingRight)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            attackPoint.transform.localScale = new Vector3(-1f, 1f, 1f);
            isFacingRight = false;
        }

        // Attach if at end of path
        if (aiPath.reachedEndOfPath && !isAttacking)
        {
            StartCoroutine(AttackWithDelay());
        }
    }

    private IEnumerator AttackWithDelay()
    {
        // Set isAttacking to prevent multiple attacks before this one is completed
        isAttacking = true;

        // Call the Attack() function
        Attack();

        // Wait for a specific delay before attacking
        yield return new WaitForSeconds(attackRate);

        // Reset isAttacking after the attack is complete
        isAttacking = false;
    }

    void Attack()
    {
        Debug.Log("Enemy Attacking");
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

            Debug.Log("Enemy hit " + hits);
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
            Die();
        }
    }

    private IEnumerator DeathRotate()
    {
        yield return new WaitForSeconds(.5f);

        animator.enabled = false;
        float totalRotation = 0f;

        while (totalRotation < 90f)
        {
            float rotationAmount = 500 * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationAmount);
            totalRotation += rotationAmount;

            yield return null;
        }
    }

    private void Die()
    {
        StartCoroutine(DeathRotate());
        aiPath.enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        Gizmos.DrawWireSphere(transform.position, attackMinDistance);
    }
}
