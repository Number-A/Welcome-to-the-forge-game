using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedArmourBoss : Enemy, IAttackableByPlayer
{
    [SerializeField] protected GameObject[] creeps;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected float fireballDamage = 4.0f;
    [SerializeField] protected int numAttacksBeforeSpecial = 4;
    [SerializeField] protected float summoningDuration = 3.0f;

    private GameObject winScreen;

    protected int numAttacks = 0;
    protected List<GameObject> summons = new List<GameObject>();
    protected List<GameObject> projectiles = new List<GameObject>();

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Slashing,
            Effect = Attack.AttackEffect.None
        };
    }

    protected override void Start()
    {
        base.Start();
        winScreen = PrefabLoader.GetPrefabByName("WinScreen");
    }

    protected override void Update()
    {
        if (projectile != null && (int)((GetMaxHealth() - GetCurrHealth()) / (GetMaxHealth() / 5)) > projectiles.Count && currentState != State.Dying && currentState != State.Dead)
        {
            Vector2 projectileSpawnPoint = pathfinder.GetRandomUnobstructedTilePosition();
            GameObject newProjectile = Instantiate(projectile, projectileSpawnPoint, Quaternion.identity);
            newProjectile.transform.SetParent(transform.parent);
            projectiles.Add(newProjectile);
            BlueFireball bFireballScript = newProjectile.GetComponent<BlueFireball>();
            bFireballScript.SetAttack(new Attack()
            {
                Value = fireballDamage,
                Type = Attack.AttackType.Magic,
                Effect = Attack.AttackEffect.None
            });
            bFireballScript.SetDirection(new Vector2((playerTransform.position.x - projectileSpawnPoint.x)/Mathf.Abs(playerTransform.position.x - projectileSpawnPoint.x), (playerTransform.position.y - projectileSpawnPoint.y)/Mathf.Abs(playerTransform.position.y - projectileSpawnPoint.y)));
            bFireballScript.SetCorners(pathfinder.GetLowerLeftCornerPos(), pathfinder.GetUpperRightCornerPos());
        }

        base.Update();
    }

    protected virtual void FlashIfDamaged()
    {
        if (nextFlinchEndTime > Time.time)
        {
            if (nextOpacityChangeTime <= Time.time)
            {
                if (spriteRenderer.color.a == 1)
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.2f);
                }
                else
                {
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
                }
                nextOpacityChangeTime = Time.time + opacityChangeInterval;
            }
        }
        else
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        }
    }

    protected override void StateIdle()
    {
        FlashIfDamaged();
        base.StateIdle();
    }

    protected override void StateNavigating()
    {
        FlashIfDamaged();
        base.StateNavigating();
    }

    protected override void StateAnticipating()
    {
        FlashIfDamaged();
        if (nextAnticipationEndTime <= Time.time)
        {
            if (numAttacks >= numAttacksBeforeSpecial)
            {
                // Spawn minions
                for (int i = 0; i < creeps.Length; i++)
                {
                    Vector2 position = pathfinder.GetRandomUnobstructedTilePosition();
                    GameObject newCreep = Instantiate(creeps[i], position, Quaternion.identity);
                    newCreep.transform.SetParent(transform.parent);
                    summons.Add(newCreep);
                    Enemy enemyScript = newCreep.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        enemyScript.RegisterPathfinder(pathfinder);
                    }
                }
                numAttacks = 0;
                nextSpecialEndTime = Time.time + summoningDuration;
                currentState = State.Special;
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

    protected override void StateAttacking()
    {
        FlashIfDamaged();
        base.StateAttacking();
    }

    protected override void StateSpecial()
    {
        FlashIfDamaged();
        isMoving = false;
        if (nextSpecialEndTime <= Time.time)
        {
            currentState = State.Navigating;
        }
    }

    public override bool ReceiveAttack(Attack attack)
    {
        if (currentState != State.Special && currentState != State.Dying && currentState != State.Dead)
        {
            float modifier = 1.0f;
            if (weakAgainst == resistantAgainst)
            {

            }
            else if (attack.Type == resistantAgainst)
            {
                modifier = resistDamageModifier;
            }
            else if (attack.Type == weakAgainst)
            {
                modifier = weakDamageModifier;
            }
            if (TakeDamage(attack.Value * modifier))
            {
                // If damaged, flinch
                nextFlinchEndTime = Time.time + flinchTime;
                nextOpacityChangeTime = Time.time + opacityChangeInterval;
                if (GetCurrHealth() <= 0)
                {
                    // If no remaining health, die
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
                    Die();
                    winScreen.SetActive(true);
                    AreaDoorController.SetBossDefeated(1);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    protected override void Die()
    {
        AreaDoorController.SetBossDefeated(1);
        for (int i = summons.Count - 1; i >= 0; i--)
        {
            if (summons[i] != null)
            {
                Destroy(summons[i]);
            }
            summons.RemoveAt(i);
        }

        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            if (projectiles[i] != null)
            {
                Destroy(projectiles[i]);
            }
            projectiles.RemoveAt(i);
        }

        base.Die();
    }
}
