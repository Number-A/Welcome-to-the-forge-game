using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedVine : MonoBehaviour
{
    [SerializeField] protected float travelSpeed = 300f;
    [SerializeField] protected float maxTravelDistance = 800.0f;
    [SerializeField] protected Vector2 direction = new Vector2(1, 0);
    [SerializeField] protected float stopTime = 0.1f;

    protected Attack defaultAttack = new Attack();

    protected bool moving = false;
    protected CircleCollider2D hitbox;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected float currentTravelDistance = 0;

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

    protected virtual void Start()
    {
        transform.localScale = Vector3.one;

        // Normalize direction vector
        direction = direction.normalized;

        // Get components
        hitbox = GetComponentInChildren<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void FixedUpdate()
    {
        if (moving && travelSpeed != 0 && currentTravelDistance < maxTravelDistance)
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
            StopAndDestroyProjectile();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IAttackableByEnemy player = collision.gameObject.GetComponent<IAttackableByEnemy>();
            if (player != null)
            {
                player.ReceiveAttack(defaultAttack);
            }
        }
        else if (collision.gameObject.name == "Walls" || collision.gameObject.name == "Obstacles" || collision.gameObject.name == "Doors") {
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

    public virtual void StartMoving()
    {
        moving = true;
    }

    public virtual void StopMoving()
    {
        moving = false;
    }
}
