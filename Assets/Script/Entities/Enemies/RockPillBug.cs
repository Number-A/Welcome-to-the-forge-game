using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPillBug : Enemy, IAttackableByPlayer
{
    [SerializeField] protected float attackDistance = 100f;
    [SerializeField] protected float lungeTime = 2.0f;
    [SerializeField] protected float vulnerableTime = 4.0f;
    [SerializeField] protected float bounceForce = 20f;
    [SerializeField] protected float bounceTime = 0.3f;

    protected float nextLungeTimeEnd = 0;
    protected float nextVulnerabilityEndTime = 0;
    protected bool bouncing = false;

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Blunt,
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

    protected override void StateAnticipating()
    {
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

    protected override void StateFlinching()
    {
        isMoving = false;
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

        if (nextVulnerabilityEndTime <= Time.time)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            spriteRenderer.transform.localScale = new Vector3(spriteRenderer.transform.localScale.x, Mathf.Abs(spriteRenderer.transform.localScale.y), spriteRenderer.transform.localScale.z);
            currentState = State.Idle;
        }
    }

    protected override void StateSpecial()
    {
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

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == State.Special)
        {
            if (collision.gameObject.tag != "Player")
            {
                rb.AddForce(collision.contacts[0].normal * bounceForce, ForceMode2D.Impulse);
                AudioManagerScript.instance.Play("PillBugCrash");
                bouncing = true;
                Invoke("StopBounce", bounceTime);
                currentState = State.Flinching;
                nextVulnerabilityEndTime = Time.time + vulnerableTime;
                spriteRenderer.transform.localScale = new Vector3(spriteRenderer.transform.localScale.x, -Mathf.Abs(spriteRenderer.transform.localScale.y), spriteRenderer.transform.localScale.z);
            }
        }
    }

    public override bool ReceiveAttack(Attack attack)
    {
        if (currentState == State.Flinching)
        {
            return base.ReceiveAttack(attack);
        }
        else
        {
            return false;
        }
    }

    protected override void FixedUpdate()
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
        else if (!bouncing)
        {
            rb.velocity = Vector2.zero;
        }
    }

    protected virtual void StopBounce()
    {
        bouncing = false;
    }
}
