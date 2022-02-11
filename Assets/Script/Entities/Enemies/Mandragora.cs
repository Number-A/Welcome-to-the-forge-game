using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mandragora : Enemy, IAttackableByPlayer
{
    [SerializeField] protected float attackDuration = 0.5f;
    [SerializeField] protected ParticleSystem screamEffect;

    protected float nextSpecialTimeEnd = 0;

    protected override void SetDefaultAttack()
    {
        // Default attack
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Magic,
            Effect = Attack.AttackEffect.None
        };
    }

    protected override void StateIdle()
    {
        isMoving = false;
        if (playerTransform != null)
        {
            Vector2 directionToPlayer = GetPlayerPosition() - baseTransform.position;
            float distanceToPlayer = directionToPlayer.magnitude;
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
            }
        }
    }

    protected override void StateNavigating()
    {
        if (playerTransform != null)
        {
            isMoving = true;
            if (currPath.Count <= pathingIndex)
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
            nextSpecialEndTime = Time.time + attackDuration;
            currentState = State.Special;
            if (screamEffect != null && screamEffect.isStopped)
            {
                screamEffect.Play();
            }
        }
    }

    protected override void StateSpecial()
    {
        isMoving = false;
        if (nextSpecialEndTime <= Time.time)
        {
            if (screamEffect != null && screamEffect.isPlaying)
            {
                screamEffect.Stop();
            }
            nextAttackRecoveryEndTime = Time.time + attackRecoveryTime;
            nextPossibleAttackTime = Time.time + minAttackInterval;
            currentState = State.Attacking;
            if (pathfinder != null)
            {
                Vector2 targetLocation = pathfinder.GetRandomUnobstructedTilePosition();
                currPath.Clear();
                pathingIndex = 0;
                currPath = pathfinder.FindPath(transform.position, targetLocation);
            }
        }
        else
        {
            if (hitbox != null && hitbox.PlayerIsInRange())
            {
                IAttackableByEnemy player = playerTransform.gameObject.GetComponent<IAttackableByEnemy>();
                if (player != null)
                {
                    player.ReceiveAttack(defaultAttack);
                }
            }
        }
    }

    public override bool ReceiveAttack(Attack attack)
    {
        if (currentState == State.Attacking || currentState == State.Navigating)
        {
            return base.ReceiveAttack(attack);
        }
        else
        {
            return false;
        }
    }
}
