using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy, IAttackableByPlayer
{
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
            currentState = State.Navigating;
        }
    }

    protected override void StateNavigating()
    {
        if (playerTransform != null)
        {
            if (nextNavigationUpdateTime <= Time.time)
            {
                Vector2 targetPosition = GetPlayerPosition();
                currPath.Clear();
                currPath.Add(targetPosition);
                pathingIndex = 0;
                nextNavigationUpdateTime = Time.time + navigationUpdateInterval;
            }

            if (hitbox != null && hitbox.PlayerIsInRange())
            {
                IAttackableByEnemy player = playerTransform.gameObject.GetComponent<IAttackableByEnemy>();
                if (player != null)
                {
                    player.ReceiveAttack(defaultAttack);
                }
            }
            
            if (currPath.Count > pathingIndex)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = false;
        }
    }

    protected override void StateFlinching()
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

        StateNavigating();
        
        if (nextFlinchEndTime <= Time.time)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            currentState = State.Navigating;
        }
    }
}
