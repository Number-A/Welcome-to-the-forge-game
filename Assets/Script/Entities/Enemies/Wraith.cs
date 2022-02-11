using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wraith : Enemy, IAttackableByPlayer
{
    [SerializeField] protected float attackDistance = 100f;
    [SerializeField] protected float lungeTime = 2.0f;

    protected float nextLungeTimeEnd = 0;

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Magic,
            Effect = Attack.AttackEffect.None
        };
    }

    protected override void StateIdle()
    {
        // Set transparent
        if (spriteRenderer.color.a > 0.3f)
        {
            float opacity = Mathf.Clamp(spriteRenderer.color.a - 1.0f * Time.deltaTime, 0, 1);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
        }

        base.StateIdle();
    }

    protected override void StateNavigating()
    {
        // Set transparent
        if (spriteRenderer.color.a > 0.3f)
        {
            float opacity = Mathf.Clamp(spriteRenderer.color.a - 1.0f * Time.deltaTime, 0, 1);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
        }

        if (playerTransform != null)
        {
            Vector2 directionToPlayer = GetPlayerPosition() - baseTransform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            Vector2 targetPosition;

            if (distanceToPlayer <= maxAttackDistanceFromPlayer * scaleFactor && distanceToPlayer >= minAttackDistanceFromPlayer * scaleFactor)
            {
                if (nextPossibleAttackTime <= Time.time)
                {
                    nextAnticipationEndTime = Time.time + anticipationTime;
                    currentState = State.Anticipating;
                    if (directionToPlayer.x < 0)
                    {
                        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                    else if (directionToPlayer.x > 0)
                    {
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                }
                else
                {
                    // Circle around player
                    float angleToPlayer = Mathf.Atan2(-directionToPlayer.y, -directionToPlayer.x);
                    // Add fixed amount of rotation
                    angleToPlayer += 0.2f;
                    Vector2 newRelativePosition = new Vector2(distanceToPlayer * Mathf.Cos(angleToPlayer), distanceToPlayer * Mathf.Sin(angleToPlayer));
                    targetPosition = new Vector2(newRelativePosition.x + GetPlayerPosition().x, newRelativePosition.y + GetPlayerPosition().y);
                    currPath.Clear();
                    currPath.Add(targetPosition);
                    pathingIndex = 0;
                    isMoving = true;
                }
            }
            else
            {
                targetPosition = GetPlayerPosition();
                currPath.Clear();
                currPath.Add(targetPosition);
                pathingIndex = 0;
                isMoving = true;
            }
        }
        else
        {
            currentState = State.Idle;
        }
    }

    protected override void StateAnticipating()
    {
        // Set opaque
        if (spriteRenderer.color.a < 1.0f)
        {
            float opacity = Mathf.Clamp(spriteRenderer.color.a + 1.0f * Time.deltaTime, 0, 1);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
        }

        isMoving = false;
        if (nextAnticipationEndTime <= Time.time)
        {
            Vector2 targetPosition = baseTransform.position + (GetPlayerPosition() - baseTransform.position).normalized * attackDistance;
            currPath.Clear();
            currPath.Add(targetPosition);
            pathingIndex = 0;
            nextPossibleAttackTime = Time.time + minAttackInterval;
            nextLungeTimeEnd = Time.time + lungeTime;
            if (GetPlayerPosition().x < baseTransform.position.x)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (GetPlayerPosition().x > baseTransform.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            currentState = State.Special;
        }
    }

    protected override void StateAttacking()
    {
        // Set opaque
        if (spriteRenderer.color.a < 1.0f)
        {
            float opacity = Mathf.Clamp(spriteRenderer.color.a + 1.0f * Time.deltaTime, 0, 1);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
        }

        base.StateAttacking();
    }

    protected override void StateSpecial()
    {
        // Set opaque
        if (spriteRenderer.color.a < 1.0f)
        {
            float opacity = Mathf.Clamp(spriteRenderer.color.a + 1.0f * Time.deltaTime, 0, 1);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
        }

        isMoving = true;
        if (hitbox != null && hitbox.PlayerIsInRange())
        {
            IAttackableByEnemy player = playerTransform.gameObject.GetComponent<IAttackableByEnemy>();
            if (player != null)
            {
                player.ReceiveAttack(defaultAttack);
            }
        }
        if (nextLungeTimeEnd <= Time.time || currPath.Count <= pathingIndex)
        {
            nextAttackRecoveryEndTime = Time.time + attackRecoveryTime;
            currentState = State.Attacking;
        }
    }

    public override bool ReceiveAttack(Attack attack)
    {
        if (currentState == State.Idle || currentState == State.Navigating)
        {
            return false;
        }
        else
        {
            return base.ReceiveAttack(attack);
        }
    }
}
