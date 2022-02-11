using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueFireball : MonoBehaviour
{
    [SerializeField] protected float travelSpeed = 100f;
    [SerializeField] protected Vector2 direction = new Vector2(1, 0);
    [SerializeField] protected float rotationSpeed = 3.0f;

    protected Attack defaultAttack = new Attack();

    protected CircleCollider2D hitbox;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    protected float maxX;
    protected float minX;
    protected float maxY;
    protected float minY;

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
        // Rotate fireball sprite
        spriteRenderer.gameObject.transform.Rotate(0, 0, 90 * rotationSpeed * Time.fixedDeltaTime);

        // Set direction
        if (transform.position.x >= maxX || transform.position.x <= minX)
        {
            SetDirection(new Vector2(-direction.x, direction.y));
        }
        if (transform.position.y >= maxY || transform.position.y <= minY)
        {
            SetDirection(new Vector2(direction.x, -direction.y));
        }

        if (travelSpeed != 0)
        {
            // Move projectile
            Vector2 prevPosition2D = new Vector2(rb.position.x, rb.position.y);
            rb.position = Vector2.MoveTowards(prevPosition2D, prevPosition2D + direction, travelSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IAttackableByEnemy player = collision.gameObject.GetComponent<IAttackableByEnemy>();
            if (player != null)
            {
                player.ReceiveAttack(defaultAttack);
            }
        }
    }

    public virtual void SetCorners(Vector2 lowerLeft, Vector2 upperRight)
    {
        minX = lowerLeft.x;
        minY = lowerLeft.y;
        maxX = upperRight.x;
        maxY = upperRight.y;
    }
}
