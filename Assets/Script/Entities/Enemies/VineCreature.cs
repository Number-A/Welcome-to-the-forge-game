using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineCreature : Enemy, IAttackableByPlayer
{
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected GameObject specialProjectile;
    [SerializeField] protected Transform projectileSpawnPoint;
    [SerializeField] protected Transform specialProjectileSpawnPoint;
    [SerializeField] protected int numAttacksBetweenSpecial = 2;
    [SerializeField] protected float entangleDuration = 4.0f;
    [SerializeField] protected float specialRecoveryTime = 2.0f;

    protected int numAttacks = 0;

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Piercing,
            Effect = Attack.AttackEffect.None
        };
    }

    protected override void StateNavigating()
    {
        if (playerTransform != null)
        {
            Vector2 directionToPlayer = GetPlayerPosition() - baseTransform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            if (nextNavigationUpdateTime <= Time.time && pathfinder != null)
            {
                Vector2 targetPosition = GetPlayerPosition();
                // If too close to player, try to retreat in opposite direction
                if (distanceToPlayer <= minAttackDistanceFromPlayer * scaleFactor)
                {
                    targetPosition = baseTransform.position + (baseTransform.position - GetPlayerPosition()).normalized * (minAttackDistanceFromPlayer * scaleFactor - distanceToPlayer) * 2;
                }
                currPath = pathfinder.FindPath(baseTransform.position, targetPosition);
                if (currPath.Count == 0)
                {
                    targetPosition = pathfinder.GetRandomUnobstructedTilePosition();
                    currPath = pathfinder.FindPath(baseTransform.position, targetPosition);
                }
                pathingIndex = 0;
                nextNavigationUpdateTime = Time.time + navigationUpdateInterval;
            }

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
                    currentState = State.Idle;
                }
            }
            else
            {
                if (currPath.Count > pathingIndex)
                {
                    isMoving = true;
                }
                else
                {
                    isMoving = false;
                }
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
            if (numAttacks >= numAttacksBetweenSpecial)
            {
                // Entangling vine attack
                numAttacks = 0;
                if (specialProjectile != null)
                {
                    if (specialProjectileSpawnPoint == null)
                    {
                        specialProjectileSpawnPoint = transform;
                    }
                    GameObject newProjectile = Instantiate(specialProjectile, specialProjectileSpawnPoint.position, Quaternion.identity);
                    newProjectile.transform.SetParent(transform.parent);
                    Projectile projectileScript = newProjectile.GetComponent<Projectile>();
                    projectileScript.SetAttack(new Attack() { 
                        Value = entangleDuration,
                        Type = Attack.AttackType.Effect,
                        Effect = Attack.AttackEffect.Entangle
                    });
                    projectileScript.SetDirection(new Vector2(playerTransform.position.x - projectileSpawnPoint.position.x, playerTransform.position.y - projectileSpawnPoint.position.y));
                    EntanglingVine entanglingVine = projectileScript as EntanglingVine;
                    if (entanglingVine != null)
                    {
                        entanglingVine.SetPlayerTransform(playerTransform);
                    }
                }
                nextSpecialEndTime = Time.time + specialRecoveryTime;
                nextPossibleAttackTime = Time.time + minAttackInterval;
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
            else
            {
                // Thorn attack
                if (projectile != null)
                {
                    if (projectileSpawnPoint == null)
                    {
                        projectileSpawnPoint = transform;
                    }
                    GameObject newProjectile = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);
                    newProjectile.transform.SetParent(transform.parent);
                    Projectile projectileScript = newProjectile.GetComponent<Projectile>();
                    projectileScript.SetAttack(defaultAttack);
                    projectileScript.SetDirection(new Vector2(playerTransform.position.x - projectileSpawnPoint.position.x, playerTransform.position.y - projectileSpawnPoint.position.y));
                }
                numAttacks++;
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
    }

    protected override void StateSpecial()
    {
        isMoving = false;
        if (nextSpecialEndTime <= Time.time)
        {
            currentState = State.Navigating;
        }
    }
}
