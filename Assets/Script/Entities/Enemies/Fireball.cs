using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Projectile
{
    [SerializeField] protected float trackTime = 3.0f;
    [SerializeField] protected float rotationSpeed = 3.0f;

    protected Transform playerTransform;
    protected float trackEndTime = 0;
    protected float spriteRotationSpeed = 5.0f;

    protected override void Start()
    {
        base.Start();

        trackEndTime = Time.time + trackTime;
    }

    protected override void FixedUpdate()
    {
        // Rotate fireball sprite
        spriteRenderer.gameObject.transform.Rotate(0, 0, 90 * rotationSpeed * Time.fixedDeltaTime);

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
}
