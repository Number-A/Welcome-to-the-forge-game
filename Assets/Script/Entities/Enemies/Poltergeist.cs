using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poltergeist : Enemy, IAttackableByPlayer
{
    [SerializeField] protected GameObject flame;
    protected Vector2 targetPosition = Vector2.zero;

    protected override void SetDefaultAttack()
    {
        defaultAttack = new Attack()
        {
            Value = GetAttack(),
            Type = Attack.AttackType.Magic,
            Effect = Attack.AttackEffect.None
        };
    }

    protected override void StateNavigating()
    {
        if (playerTransform != null)
        {
            if (pathfinder != null && currPath.Count <= pathingIndex)
            {
                targetPosition = pathfinder.GetRandomUnobstructedTilePosition();
                currPath.Clear();
                currPath.Add(targetPosition);
                pathingIndex = 0;
            }

            Vector2 directionToPlayer = GetPlayerPosition() - baseTransform.position;
            if (nextPossibleAttackTime <= Time.time)
            {
                nextAnticipationEndTime = Time.time + anticipationTime;
                currentState = State.Anticipating;
            }
            else
            {
                print(currPath.Count);
                print(pathingIndex);
                isMoving = true;
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
            if (flame != null)
            {
                Vector2 flameSpawnLocation = transform.position;
                if (pathfinder != null)
                {
                    flameSpawnLocation = pathfinder.GetRandomUnobstructedTilePosition();
                }
                GameObject newFlame = Instantiate(flame, flameSpawnLocation, Quaternion.identity);
                newFlame.transform.SetParent(transform.parent);
                Flame projectileScript = newFlame.GetComponent<Flame>();
                projectileScript.SetAttack(defaultAttack);
                if (flameSpawnLocation.x < baseTransform.position.x)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (flameSpawnLocation.x > baseTransform.position.x)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
            nextAttackRecoveryEndTime = Time.time + attackRecoveryTime;
            nextPossibleAttackTime = Time.time + minAttackInterval;
            currentState = State.Attacking;
        }
    }
}
