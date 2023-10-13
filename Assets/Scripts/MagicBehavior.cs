using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBehavior : MonoBehaviour
{
    [SerializeField] private float normalMagicSpeed = 15f;
    [SerializeField] private float destroyTime = 3f;
    [SerializeField] private LayerMask whatDestroysBullet;
    [SerializeField] private int normalMagicDamage = 1;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        SetDestroyTime();

        SetStraightVelocity();
    }

    private void SetStraightVelocity()
    {
        rb.velocity = transform.right * normalMagicSpeed;
    }

    private void SetDestroyTime()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        // Bitwise operations to determine we are within layer mask - eww?
        if ((whatDestroysBullet.value & (1 << collider2D.gameObject.layer)) > 0)
        {
            // Damage Enemy
            IDamageable iDamageable = collider2D.gameObject.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.TakeDamage(normalMagicDamage);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if ((whatDestroysBullet.value & (1 << collider2D.gameObject.layer)) > 0)
        {
            Destroy(gameObject);
        }
    }
}
