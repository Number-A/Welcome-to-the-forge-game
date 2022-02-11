using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy, IAttackableByPlayer
{
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected Transform projectileSpawnPoints;
    [SerializeField] protected Transform angleProjectileSpawnPoints;
    
    protected bool isAtAngle = false;
    protected Transform currentProjectileSpawnPoints;

    protected Dictionary<State, string> unangledAnimations = new Dictionary<State, string>() {
        { State.Idle, "Base Layer.Idle" },
        { State.Navigating, "Base Layer.Navigating" },
        { State.Anticipating, "Base Layer.Anticipating" },
        { State.Attacking, "Base Layer.Attacking" },
        { State.Flinching, "Base Layer.Flinching" },
        { State.Dying, "Base Layer.Flinching" },
        { State.Dead, "Base Layer.Flinching" },
        { State.Special, "Base Layer.Special" }
    };

    protected Dictionary<State, string> atAngleAnimations = new Dictionary<State, string>() {
        { State.Idle, "Base Layer.Idle2" },
        { State.Navigating, "Base Layer.Navigating2" },
        { State.Anticipating, "Base Layer.Anticipating2" },
        { State.Attacking, "Base Layer.Attacking2" },
        { State.Flinching, "Base Layer.Flinching2" },
        { State.Dying, "Base Layer.Flinching2" },
        { State.Dead, "Base Layer.Flinching2" },
        { State.Special, "Base Layer.Special2" }
    };

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Piercing,
            Effect = Attack.AttackEffect.None
        };
    }

    protected override void Start()
    {
        base.Start();

        if (isAtAngle && angleProjectileSpawnPoints != null)
        {
            currentProjectileSpawnPoints = angleProjectileSpawnPoints;
        }
        else if (!isAtAngle && projectileSpawnPoints != null)
        {
            currentProjectileSpawnPoints = projectileSpawnPoints;
        }
    }

    protected override void Update()
    {
        if (isAtAngle)
        {
            animations = atAngleAnimations;
        }
        else
        {
            animations = unangledAnimations;
        }

        base.Update();
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
                }
            }
        }
    }

    protected override void StateAnticipating()
    {
        isMoving = false;
        if (nextAnticipationEndTime <= Time.time)
        {
            // Spawn projectiles
            if (projectile != null)
            {
                if (currentProjectileSpawnPoints == null)
                {
                    currentProjectileSpawnPoints = transform;
                }
                foreach (Transform child in currentProjectileSpawnPoints)
                {
                    GameObject newProjectile = Instantiate(projectile, child.position, Quaternion.identity);
                    newProjectile.transform.SetParent(transform.parent);
                    Projectile projectileScript = newProjectile.GetComponent<Projectile>();
                    projectileScript.SetAttack(defaultAttack);
                    projectileScript.SetDirection(new Vector2(child.position.x - transform.position.x, child.position.y - transform.position.y).normalized);
                }
            }
            nextAttackRecoveryEndTime = Time.time + attackRecoveryTime;
            nextPossibleAttackTime = Time.time + minAttackInterval;
            currentState = State.Attacking;
        }
    }

    protected override void StateAttacking()
    {
        if (nextAttackRecoveryEndTime <= Time.time)
        {
            currentState = State.Navigating;
            isAtAngle = !isAtAngle;
            if (isAtAngle && angleProjectileSpawnPoints != null)
            {
                currentProjectileSpawnPoints = angleProjectileSpawnPoints;
            }
            else if (!isAtAngle && projectileSpawnPoints != null)
            {
                currentProjectileSpawnPoints = projectileSpawnPoints;
            }
        }

        base.StateAttacking();
    }
}
