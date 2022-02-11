using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity, IAttackableByPlayer
{
    // Player base position offset
    protected const float playerYOffset = -0.9f;

    // Editor variables and linked objects
    [SerializeField] protected RangeCheck hitbox;
    [SerializeField] protected Transform baseTransform;

    [SerializeField] protected float navigationUpdateInterval = 0.5f;
    [SerializeField] protected float anticipationTime = 0.5f;
    [SerializeField] protected float attackRecoveryTime = 1.0f;
    [SerializeField] protected float minAttackInterval = 2.0f;
    [SerializeField] protected float flinchTime = 0.5f;
    [SerializeField] protected float dyingTime = 0.5f;
    [SerializeField] protected float opacityChangeInterval = 0.1f;
    [SerializeField] protected float minAttackDistanceFromPlayer = 0f;
    [SerializeField] protected float maxAttackDistanceFromPlayer = 8f;

    [SerializeField] protected GameObject droppedResource;
    [SerializeField] protected float dropChance = 0.4f;
    [SerializeField] protected float multipleResourceChance = 0.2f;
    [SerializeField] protected int maxResourceDrop = 5;


    [SerializeField] protected Attack.AttackType weakAgainst;
    [SerializeField] protected Attack.AttackType resistantAgainst;
    [SerializeField] protected float weakDamageModifier = 1.5f;
    [SerializeField] protected float resistDamageModifier = 0.7f;

    protected enum State
    {
        Idle,
        Navigating,
        Anticipating,
        Attacking,
        Flinching,
        Dying,
        Dead,
        Special
    }

    // Internal state variables
    protected State currentState = State.Idle;
    protected float scaleFactor = 1;
    protected float nextNavigationUpdateTime = 0;
    protected float nextAnticipationEndTime = 0;
    protected float nextAttackRecoveryEndTime = 0;
    protected float nextPossibleAttackTime = 0;
    protected float nextFlinchEndTime = 0;
    protected float nextSpecialEndTime = 0;
    protected float nextDeathTime = 0;
    protected float nextOpacityChangeTime = 0;
    protected bool isMoving = false;
    protected State lastState;

    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected Rigidbody2D rb;
    protected Transform playerTransform;

    // Animator dictionary
    protected Dictionary<State, string> animations = new Dictionary<State, string>() {
        { State.Idle, "Base Layer.Idle" },
        { State.Navigating, "Base Layer.Navigating" },
        { State.Anticipating, "Base Layer.Anticipating" },
        { State.Attacking, "Base Layer.Attacking" },
        { State.Flinching, "Base Layer.Flinching" },
        { State.Dying, "Base Layer.Flinching" },
        { State.Dead, "Base Layer.Flinching" },
        { State.Special, "Base Layer.Special" }
    };

    // Pathfinding
    protected Pathfinder pathfinder;
    protected List<Vector2> currPath;
    protected int pathingIndex = 0;
    [SerializeField] protected float pathingTolerance = 5f;

    // Default attack
    protected Attack defaultAttack;

    protected virtual void Start()
    {
        transform.localScale = Vector3.one;

        // Get components
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        scaleFactor = transform.localScale.x;

        playerTransform = null;
        currPath = new List<Vector2>();

        SetDefaultAttack();
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                StateIdle();
                break;

            case State.Navigating:
                StateNavigating();
                break;

            case State.Anticipating:
                StateAnticipating();
                break;

            case State.Attacking:
                StateAttacking();
                break;

            case State.Flinching:
                StateFlinching();
                break;

            case State.Dying:
                StateDying();
                break;

            case State.Dead:
                StateDead();
                break;

            case State.Special:
                StateSpecial();
                break;
        }
        if (lastState != currentState) {
            animator.Play(animations[currentState], 0, 0);
            lastState = currentState;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isMoving && currPath.Count > pathingIndex)
        {
            Vector2 walkPath = (currPath[pathingIndex] - new Vector2(baseTransform.position.x, baseTransform.position.y));
            if (walkPath.magnitude <= pathingTolerance * scaleFactor)
            {
                pathingIndex++;
                if (currPath.Count > pathingIndex)
                {
                    walkPath = (currPath[pathingIndex] - new Vector2(baseTransform.position.x, baseTransform.position.y));
                }
                else
                {
                    walkPath = Vector2.zero;
                }
            }
            // Set velocity
            rb.velocity = walkPath.normalized * GetMoveSpeed();
            // Set sprite renderer sorting order based on new position
            //spriteRenderer.sortingOrder = (int)(-baseTransform.position.y * 10/scaleFactor);
            // Set enemy facing based on walk direction
            if (walkPath.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (walkPath.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (playerTransform == null)
            {
                playerTransform = collision.transform;
            }
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (playerTransform == null)
            {
                playerTransform = collision.transform;
            }
        }
    }
    
    protected virtual void SetDefaultAttack()
    {
        // Default attack
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Other,
            Effect = Attack.AttackEffect.None
        };
    }

    protected virtual void StateIdle()
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
            else
            {
                currentState = State.Navigating;
            }
        }
    }

    protected virtual void StateNavigating()
    {
        if (playerTransform != null)
        {
            if (nextNavigationUpdateTime <= Time.time && pathfinder != null)
            {
                Vector2 targetPosition = GetPlayerPosition();
                currPath = pathfinder.FindPath(baseTransform.position, targetPosition);
                pathingIndex = 0;
                nextNavigationUpdateTime = Time.time + navigationUpdateInterval;
            }

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

    protected virtual void StateAnticipating()
    {
        isMoving = false;
        if (nextAnticipationEndTime <= Time.time)
        {
            if (hitbox != null && hitbox.PlayerIsInRange())
            {
                IAttackableByEnemy player = playerTransform.gameObject.GetComponent<IAttackableByEnemy>();
                if (player != null)
                {
                    player.ReceiveAttack(defaultAttack);
                }
            }
            nextAttackRecoveryEndTime = Time.time + attackRecoveryTime;
            nextPossibleAttackTime = Time.time + minAttackInterval;
            currentState = State.Attacking;
        }
    }

    protected virtual void StateAttacking()
    {
        isMoving = false;
        if (nextAttackRecoveryEndTime <= Time.time)
        {
            currentState = State.Navigating;
        }
    }

    protected virtual void StateFlinching()
    {
        isMoving = false;
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
        if (nextFlinchEndTime <= Time.time)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            currentState = State.Navigating;
        }
    }

    protected virtual void StateDying()
    {
        isMoving = false;
        if (nextOpacityChangeTime <= Time.time)
        {
            float newOpacity = Mathf.Max(spriteRenderer.color.a - 0.2f, 0);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newOpacity);
            nextOpacityChangeTime = Time.time + opacityChangeInterval;
        }
        if (nextDeathTime <= Time.time)
        {
            if (droppedResource != null)
            {
                if (Random.value <= dropChance)
                {
                    int numDrops = 1;
                    if (Random.value <= multipleResourceChance) 
                    {
                        numDrops = Random.Range(1, maxResourceDrop);
                    }

                    for (int i = 0; i < numDrops; i++)
                    {
                        Vector3 resourcePositionModifier = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);

                        GameObject drop = Instantiate(droppedResource, baseTransform.position + resourcePositionModifier, Quaternion.identity);
                        drop.transform.SetParent(transform.parent);
                    }
                }
            }
            currentState = State.Dead;
        }
    }

    protected virtual void StateDead()
    {
        isMoving = false;
        Destroy(this.gameObject, 0.5f);
        enabled = false;
    }

    protected virtual void StateSpecial()
    {
        
    }

    // Register the pathfinder object to use for the enemy's present room
    public virtual void RegisterPathfinder(Pathfinder pathfinder)
    {
        this.pathfinder = pathfinder;
    }

    // Receive attack and compare against weakness and resistance types
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
                currentState = State.Flinching;
                nextOpacityChangeTime = Time.time + opacityChangeInterval;
                if (GetCurrHealth() <= 0)
                {
                    // If no remaining health, die
                    Die();
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

    protected virtual void Die() {
        nextDeathTime = Time.time + dyingTime;
        currentState = State.Dying;
        nextOpacityChangeTime = Time.time + opacityChangeInterval;

        // Decrement number of enemies (for Anto)
        // EnemyCounter.numOfEnemies--;
    }

    protected virtual Vector3 GetPlayerPosition()
    {
        if (playerTransform != null)
        {
            return new Vector3(playerTransform.position.x, playerTransform.position.y + playerYOffset, playerTransform.position.z);
        }
        else
        {
            return Vector3.zero;
        }
    }

    public virtual Vector3 GetPosition()
    {
        return baseTransform.position;
    }
}
