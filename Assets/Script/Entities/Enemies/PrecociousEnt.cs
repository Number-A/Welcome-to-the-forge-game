using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecociousEnt : Enemy, IAttackableByPlayer
{
    [SerializeField] protected float pollinationInterval = 5.0f;
    [SerializeField] protected float pollinationDuration = 1.5f;
    [SerializeField] protected float pollinationHitboxDuration = 1.0f;
    [SerializeField] protected float confuseEffectDuration = 5.0f;
    [SerializeField] protected ParticleSystem pollinationParticles;
    [SerializeField] protected RangeCheck pollinationHitbox;

    protected float nextPollinationEndTime = 0;
    protected float nextPollinationTime = 0;

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Blunt,
            Effect = Attack.AttackEffect.None
        };
    }

    protected virtual void Pollinate()
    {
        if (nextPollinationEndTime > Time.time && nextPollinationEndTime - pollinationHitboxDuration <= Time.time)
        {
            if (pollinationHitbox != null && pollinationHitbox.PlayerIsInRange())
            {
                IAttackableByEnemy player = playerTransform.gameObject.GetComponent<IAttackableByEnemy>();
                if (player != null)
                {
                    player.ReceiveAttack(new Attack()
                    {
                        Value = confuseEffectDuration,
                        Type = Attack.AttackType.Effect,
                        Effect = Attack.AttackEffect.Confuse
                    });
                }
            }
        }
        else if (nextPollinationEndTime <= Time.time)
        {
            if (pollinationParticles.isPlaying)
            {
                pollinationParticles.Stop();
            }
        }

        if (nextPollinationTime <= Time.time)
        {
            if (pollinationParticles.isStopped)
            {
                pollinationParticles.Play();
            }
            nextPollinationEndTime = Time.time + pollinationDuration;
            nextPollinationTime = Time.time + pollinationInterval;
        }
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
        Pollinate();
        FlashIfDamaged();
        base.StateIdle();
    }

    protected override void StateNavigating()
    {
        Pollinate();
        FlashIfDamaged();
        base.StateNavigating();
    }

    protected override void StateAnticipating()
    {
        Pollinate();
        FlashIfDamaged();
        base.StateAnticipating();
    }

    protected override void StateAttacking()
    {
        Pollinate();
        FlashIfDamaged();
        base.StateAttacking();
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
}
