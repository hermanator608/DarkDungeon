using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject magic;
    [SerializeField] private Transform magicSpawnPoint;
    private GameObject magicInstance;

    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    private float nextAttackTime = 0f;

    public int speed = 5;
    public Transform attackPoint;
    public float attackRate = 2f;
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;
    private int currentHealth;
    public int maxHealth = 2;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();

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
            MagicAttack();
        }
    }

    private void MagicAttack()
    {

        magicInstance = Instantiate(magic, magicSpawnPoint.position, transform.rotation);
    }

    private void MeleeAttack()
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

    // TODO: Update this to use mouse angle?
    private void HandleFlip()
    {

        if (movement.x < 0 && !spriteRenderer.flipX)
        {
            spriteRenderer.flipX = true;
            attackPoint.gameObject.transform.SetLocalPositionAndRotation(new Vector3(-attackPoint.gameObject.transform.localPosition.x, attackPoint.gameObject.transform.localPosition.y), attackPoint.gameObject.transform.localRotation);
        }
        else if (movement.x > 0 && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false;
            attackPoint.gameObject.transform.SetLocalPositionAndRotation(new Vector3(-attackPoint.gameObject.transform.localPosition.x, attackPoint.gameObject.transform.localPosition.y), attackPoint.gameObject.transform.localRotation);
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) 
            return;

        currentHealth -= damage;

        // Play hurt animation
        animator.SetTrigger("Damage");

        Debug.Log("Took damage, current health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("GAME OVER: YOU'RE DEAD");
        StartCoroutine(DeathRotate());
        Invoke("RestartGame", 5f); 
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // TODO: Replace with animation?
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

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
