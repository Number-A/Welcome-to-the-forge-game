using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warlock : Enemy, IAttackableByPlayer
{
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected Transform projectileSpawnPoint;

    [SerializeField]
    protected CapsuleCollider2D hurtbox;

    protected Vector2 targetPosition;

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Magic,
            Effect = Attack.AttackEffect.None
        };
    }

    protected override void StateNavigating()
    {
        isMoving = false;
        if (playerTransform != null)
        {
            if (pathfinder != null)
            {
                targetPosition = pathfinder.GetRandomUnobstructedTilePosition();
                currentState = State.Special;
            }
            else
            {
                currentState = State.Idle;
            }
        }
        else
        {
            currentState = State.Idle;
        }
    }

    protected override void StateAnticipating()
    {
        isMoving = false;
        if (nextAnticipationEndTime <= Time.time)
        {
            if (projectile != null)
            {
                if (projectileSpawnPoint != null)
                {
                    projectileSpawnPoint = transform;
                }
                GameObject newProjectile = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);
                newProjectile.transform.SetParent(transform.parent);
                Projectile projectileScript = newProjectile.GetComponent<Projectile>();
                projectileScript.SetAttack(defaultAttack);
                projectileScript.SetDirection(new Vector2(playerTransform.position.x - projectileSpawnPoint.position.x, playerTransform.position.y - projectileSpawnPoint.position.y));
                Fireball fireball = projectileScript as Fireball;
                AudioManagerScript.instance.Play("Fireball");
                if (fireball != null)
                {
                    fireball.SetPlayerTransform(playerTransform);
                }
            }
            nextAttackRecoveryEndTime = Time.time + attackRecoveryTime;
            nextPossibleAttackTime = Time.time + minAttackInterval;
            if (GetPlayerPosition().x < baseTransform.position.x)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (GetPlayerPosition().x > baseTransform.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            currentState = State.Attacking;
        }
    }

    protected override void StateSpecial()
    {
        isMoving = false;
        float distanceToTarget = (new Vector2(transform.position.x, transform.position.y) - targetPosition).magnitude;
        // If at target location
        if (distanceToTarget <= pathingTolerance * scaleFactor)
        {
            // Turn opaque
            if (spriteRenderer.color.a < 1.0f)
            {
                float opacity = Mathf.Clamp(spriteRenderer.color.a + 1.0f * Time.deltaTime, 0, 1);
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
                hurtbox.enabled = true;
            }
            else
            {
                nextAnticipationEndTime = Time.time + anticipationTime;
                currentState = State.Anticipating;
            }
        }
        else
        {
            if (spriteRenderer.color.a > 0)
            {
                float opacity = Mathf.Clamp(spriteRenderer.color.a - 1.0f * Time.deltaTime, 0, 1);
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
                hurtbox.enabled = false;
            }
            else
            {
                transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
            }
        }
    }
}
