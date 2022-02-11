using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootofEvil : Enemy, IAttackableByPlayer
{
    [SerializeField] protected GameObject[] creeps;
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected float appendageThornAttack = 3.0f;
    [SerializeField] protected float startingAttackInterval = 5.0f;
    [SerializeField] protected int startingNumVines = 3;
    [SerializeField] protected float vineStartInterval = 0.4f;
    [SerializeField] protected GameObject lowerLeftAppendage;
    [SerializeField] protected GameObject lowerRightAppendage;
    [SerializeField] protected GameObject upperLeftAppendage;
    [SerializeField] protected GameObject upperRightAppendage;

    private GameObject winScreen;

    protected List<GameObject> appendages = new List<GameObject>();
    protected List<GameObject> summons = new List<GameObject>();
    protected int numSummons = 0;
    protected bool initialized = false;
    protected enum Direction { Up, Down, Left, Right }
    float minX;
    float maxX;
    float minY;
    float maxY;

    protected override void Start()
    {
        base.Start();
        winScreen = PrefabLoader.GetPrefabByName("WinScreen");
    }

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Blunt,
            Effect = Attack.AttackEffect.None
        };
    }

    protected override void Update()
    {
        if (!initialized)
        {
            appendages.Add(lowerLeftAppendage);
            appendages.Add(lowerRightAppendage);
            appendages.Add(upperLeftAppendage);
            appendages.Add(upperRightAppendage);

            for (int i = 0; i < appendages.Count; i++)
            {
                appendages[i].transform.SetParent(transform.parent);
            }

            if (pathfinder != null)
            {
                lowerLeftAppendage.transform.position = pathfinder.GetLowerLeftCornerPos() + new Vector2(10, 10);
                lowerRightAppendage.transform.position = pathfinder.GetLowerRightCornerPos() + new Vector2(-10, 10);
                upperLeftAppendage.transform.position = pathfinder.GetUpperLeftCornerPos() + new Vector2(10, -10);
                upperRightAppendage.transform.position = pathfinder.GetUpperRightCornerPos() + new Vector2(-10, -10);
                minX = lowerLeftAppendage.transform.position.x;
                minY = lowerLeftAppendage.transform.position.y;
                maxX = upperRightAppendage.transform.position.x;
                maxY = upperRightAppendage.transform.position.y;
            }

            initialized = true;
        }

        // Spawn minions
        if (creeps != null && (int)((GetMaxHealth() - GetCurrHealth()) / (GetMaxHealth() / 4)) > numSummons && currentState != State.Dying && currentState != State.Dead)
        {
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
            numSummons++;
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
        if (initialized && playerTransform != null)
        {
            for (int i = 0; i < appendages.Count; i++)
            {
                RoEAppendage appendageScript = appendages[i].GetComponent<RoEAppendage>();
                if (appendageScript != null)
                {
                    appendageScript.SetPlayerTransform(playerTransform);
                    appendageScript.SetAttack(new Attack() { 
                        Value = appendageThornAttack,
                        Type = Attack.AttackType.Piercing,
                        Effect = Attack.AttackEffect.None
                    });
                    appendageScript.StartAttacking();
                }
            }
            currentState = State.Special;
        }
    }

    protected override void StateSpecial()
    {
        FlashIfDamaged();
        if (nextPossibleAttackTime <= Time.time && projectile != null && playerTransform != null)
        {
            float attackInterval = (startingAttackInterval - minAttackInterval) * (GetMaxHealth() - GetCurrHealth()) / GetMaxHealth() + minAttackInterval;
            nextPossibleAttackTime = Time.time + attackInterval;

            int numVines = startingNumVines + (int)((GetMaxHealth() - GetCurrHealth()) / (GetMaxHealth() / 4));
            float startInterval = attackInterval;

            for (int i = 0; i < numVines; i++)
            {
                Direction dir = (Direction)Random.Range(0, System.Enum.GetNames(typeof(Direction)).Length);
                Vector2 spawnPos = Vector2.zero;
                Vector2 direction = Vector2.zero;
                switch (dir)
                {
                    case Direction.Up:
                    {
                        if (i == 0)
                        {
                            spawnPos = new Vector2(playerTransform.position.x, minY);
                        }
                        else
                        {
                            spawnPos = new Vector2(Random.Range(minX, maxX), minY);
                        }
                        direction = new Vector2(0, 1);
                        break;
                    }
                    case Direction.Down:
                    {
                        if (i == 0)
                        {
                            spawnPos = new Vector2(playerTransform.position.x, maxY);
                        }
                        else
                        {
                            spawnPos = new Vector2(Random.Range(minX, maxX), maxY);
                        }
                        direction = new Vector2(0, -1);
                        break;
                    }
                    case Direction.Left:
                    {
                        if (i == 0)
                        {
                            spawnPos = new Vector2(maxX, playerTransform.position.y);
                        }
                        else
                        {
                            spawnPos = new Vector2(maxX, Random.Range(minY, maxY));
                        }
                        direction = new Vector2(-1, 0);
                        break;
                    }
                    case Direction.Right:
                    {
                        if (i == 0)
                        {
                            spawnPos = new Vector2(minX, playerTransform.position.y);
                        }
                        else
                        {
                            spawnPos = new Vector2(minX, Random.Range(minY, maxY));
                        }
                        direction = new Vector2(1, 0);
                        break;
                    }
                }
                GameObject newProjectile = Instantiate(projectile, spawnPos, Quaternion.identity);
                newProjectile.transform.SetParent(transform.parent);
                RedVine projectileScript = newProjectile.GetComponent<RedVine>();
                if (projectileScript != null)
                {
                    projectileScript.SetAttack(defaultAttack);
                    projectileScript.SetDirection(direction);
                    StartCoroutine(StartProjectile(projectileScript, startInterval));
                    startInterval += attackInterval;
                }
            }
        }
    }

    public override bool ReceiveAttack(Attack attack)
    {
        if (currentState != State.Dying && currentState != State.Dead)
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
                    AreaDoorController.SetBossDefeated(2);
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

    protected virtual IEnumerator StartProjectile(RedVine projectile, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        projectile.StartMoving();
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

        for (int i = appendages.Count - 1; i >= 0; i--)
        {
            if (appendages[i] != null)
            {
                Destroy(appendages[i]);
            }
            appendages.RemoveAt(i);
        }

        base.Die();
    }
}
