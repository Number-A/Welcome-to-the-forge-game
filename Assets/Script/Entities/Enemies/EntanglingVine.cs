using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntanglingVine : Projectile
{
    [SerializeField] protected float trackTime = 5.0f;
    [SerializeField] protected float rotationSpeed = 3.0f;

    protected Transform playerTransform;
    protected float trackEndTime = 0;

    protected override void Start()
    {
        transform.localScale = Vector3.one;

        // Normalize direction vector
        direction = direction.normalized;

        // Set projectile rotation (not for vine)
        //var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Get components
        hitbox = GetComponentInChildren<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // End track time
        trackEndTime = Time.time + trackTime;
    }

    protected override void FixedUpdate()
    {
        if (travelSpeed != 0 && currentTravelDistance < maxTravelDistance)
        {
            // Rotate towards player if tracking
            if (playerTransform != null && trackEndTime > Time.time)
            {
                direction = (playerTransform.position - transform.position).normalized;
            }
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

    public virtual void SetPlayerTransform(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision against Player (only apply if the projectile was not shot by the player)
        if (collision.gameObject.CompareTag("Player") && defaultAttack.Type != Attack.AttackType.Bow)
        {
            IAttackableByEnemy player = collision.gameObject.GetComponent<IAttackableByEnemy>();
            if (player != null)
            {
                player.ReceiveAttack(defaultAttack);
                stopTime = defaultAttack.Value;
            }
            StopAndDestroyProjectile();
        }
    }
}
