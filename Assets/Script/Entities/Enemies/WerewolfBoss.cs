using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WerewolfBoss : Enemy, IAttackableByPlayer
{

    [SerializeField] protected GameObject[] creeps;
    [SerializeField] protected float enhancedAttackValue = 5.0f;
    [SerializeField] protected float enhancedAttackRecoveryTime = 1.0f;
    [SerializeField] protected float enhancedAnticipationTime = 0.5f;
    [SerializeField] protected float enhancedMinAttackInterval = 4.0f;
    [SerializeField] protected int numAttacksBeforeSpecial = 4;
    [SerializeField] protected float howlDuration = 4.0f;
    [SerializeField] protected float howlConfuseDuration = 10.0f;

    protected int numAttacks = 0;
    protected bool enhanced = false;
    protected List<GameObject> summons = new List<GameObject>();

    [SerializeField]
    private float forgeReturnDelay = 3.0f;
    private GameObject winScreen;

    protected override void Start()
    {
        // Fetch the winScreen object in the scene so it can be displayed when the player wins
        winScreen = PrefabLoader.GetPrefabByName("WinScreen");

        base.Start();
        numAttacks = numAttacksBeforeSpecial; // Start the battle by howling
        animations = new Dictionary<State, string>() {
            { State.Idle, "Base Layer.Idle" },
            { State.Navigating, "Base Layer.Navigating" },
            { State.Anticipating, "Base Layer.Anticipating" },
            { State.Attacking, "Base Layer.Attacking" },
            { State.Flinching, "Base Layer.Flinching" },
            { State.Dying, "Base Layer.Dying" },
            { State.Dead, "Base Layer.Dying" },
            { State.Special, "Base Layer.Special" }
        };
    }

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Slashing,
            Effect = Attack.AttackEffect.None
        };
    }
    
    protected override void Update() {
        if (GetCurrHealth() <= GetMaxHealth() / 2 && !enhanced)
        {
            enhanced = true;
            spriteRenderer.color = new Color(1, 0.25f, 0.25f, spriteRenderer.color.a);
            attackRecoveryTime = enhancedAttackRecoveryTime;
            anticipationTime = enhancedAnticipationTime;
            minAttackInterval = enhancedMinAttackInterval;
            defaultAttack = new Attack()
            {
                Value = enhancedAttackValue,
                Type = defaultAttack.Type,
                Effect = defaultAttack.Effect
            };
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
                // Confuse player
                IAttackableByEnemy player = playerTransform.gameObject.GetComponent<IAttackableByEnemy>();
                if (player != null)
                {
                    player.ReceiveAttack(new Attack()
                    {
                        Value = howlConfuseDuration,
                        Type = Attack.AttackType.Effect,
                        Effect = Attack.AttackEffect.Confuse
                    });
                }
                numAttacks = 0;
                nextSpecialEndTime = Time.time + howlDuration;
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

                    winScreen.SetActive(true);

                    Die();
                    AreaDoorController.SetBossDefeated(0);
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
        for (int i = summons.Count - 1; i >= 0; i--)
        {
            if (summons[i] != null)
            {
                Destroy(summons[i]);
            }
            summons.RemoveAt(i);
        }

        base.Die();
    }
}
