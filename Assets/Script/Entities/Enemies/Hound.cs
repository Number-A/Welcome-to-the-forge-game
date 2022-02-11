using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hound : Enemy, IAttackableByPlayer
{
    [SerializeField] protected float attackDistance = 100f;
    [SerializeField] protected float lungeTime = 2.0f;

    protected float nextLungeTimeEnd = 0;

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Slashing,
            Effect = Attack.AttackEffect.None
        };
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
}
