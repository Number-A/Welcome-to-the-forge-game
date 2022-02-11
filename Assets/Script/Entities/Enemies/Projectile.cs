using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Retrieve the player collider to ignore collisions with it (for arrows shot by the player only)
    protected static Collider2D PlayerCollider = null;

    [SerializeField] protected float travelSpeed = 80f;
    [SerializeField] protected float maxTravelDistance = 800.0f;
    [SerializeField] protected Vector2 direction = new Vector2(1, 0);
    [SerializeField] protected float stopTime = 0.5f;

    protected Attack defaultAttack = new Attack();

    protected CircleCollider2D hitbox;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected float currentTravelDistance = 0;

    [SerializeField]
    protected CircleCollider2D collider;

    public virtual float GetTravelSpeed()
    {
        return travelSpeed;
    }
    public virtual void SetTravelSpeed(float newTravelSpeed)
    {
        travelSpeed = newTravelSpeed;
    }

    public virtual Attack GetAttack()
    {
        return defaultAttack;
    }
    public virtual void SetAttack(Attack newAttack)
    {
        defaultAttack = newAttack;

        // If the projectile is shot by the player ignore collisions with the player
        if (defaultAttack.Type == Attack.AttackType.Bow)
        {
            Physics2D.IgnoreCollision(PlayerCollider, collider);
        }
    }

    public virtual float GetMaxTravelDistance()
    {
        return maxTravelDistance;
    }
    public virtual void SetMaxTravelDistance(float newMaxTravelDistance)
    {
        maxTravelDistance = newMaxTravelDistance;
    }

    public virtual Vector2 GetDirection()
    {
        return direction;
    }
    public virtual void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    protected void Awake()
    {
        // Cache the player collider if it has not already been cached
        if (PlayerCollider == null)
        {
            PlayerCollider = FindObjectOfType<Player>().GetComponent<BoxCollider2D>();
        }
    }

    protected virtual void Start()
    {
        transform.localScale = Vector3.one;

        // Normalize direction vector
        direction = direction.normalized;

        // Set projectile rotation
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Get components
        hitbox = GetComponentInChildren<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void FixedUpdate()
    {
        if (travelSpeed != 0 && currentTravelDistance < maxTravelDistance)
        {
            // Move projectile
            Vector2 prevPosition2D = new Vector2(rb.position.x, rb.position.y);
            rb.position = Vector2.MoveTowards(prevPosition2D, prevPosition2D + direction, travelSpeed * Time.fixedDeltaTime);

            // Update travel distance
            currentTravelDistance += (new Vector2(rb.position.x, rb.position.y) - prevPosition2D).magnitude;
        }
        else if (currentTravelDistance >= maxTravelDistance)
        {
            // Destroy projectile if max travel distance reached
            Destroy(this.gameObject, stopTime);
        }
    }

    // Used to handle collision with the player
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision against Player (only apply if the projectile was not shot by the player)
        if (collision.gameObject.CompareTag("Player") && defaultAttack.Type != Attack.AttackType.Bow)
        {
            IAttackableByEnemy player = collision.gameObject.GetComponent<IAttackableByEnemy>();
            if (player != null)
            {
                player.ReceiveAttack(defaultAttack);
            }
        }

        StopAndDestroyProjectile();
    }

    // Used to handle collision with the enemies
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        IAttackableByPlayer target = collision.gameObject.GetComponentInParent<IAttackableByPlayer>();

        if (target != null && collision.gameObject.name == "Hurtbox"
            && !collision.gameObject.CompareTag("Player") && defaultAttack.Type == Attack.AttackType.Bow)
        {
            target.ReceiveAttack(defaultAttack);
            StopAndDestroyProjectile();
        }
    }

    protected virtual void StopAndDestroyProjectile()
    {
        // Stop, disable hitbox and destroy projectile
        rb.bodyType = RigidbodyType2D.Static;
        travelSpeed = 0;
        hitbox.enabled = false;
        Destroy(this.gameObject, stopTime);
    }
}
