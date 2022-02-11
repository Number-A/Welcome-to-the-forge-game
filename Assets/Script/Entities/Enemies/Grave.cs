using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : Enemy, IAttackableByPlayer
{
    [SerializeField] protected GameObject creep;
    [SerializeField] protected int maxNumCreeps = 5;
    protected List<GameObject> spawnedCreeps = new List<GameObject>();

    protected override void StateIdle()
    {
        isMoving = false;
        if (playerTransform != null)
        {
            currentState = State.Anticipating;
        }
    }

    protected override void StateAnticipating()
    {
        isMoving = false;
        if (nextPossibleAttackTime <= Time.time)
        {
            if (creep != null)
            {
                for(int i = spawnedCreeps.Count - 1; i >= 0; i--)
                {
                    if (spawnedCreeps[i] == null)
                    {
                        spawnedCreeps.RemoveAt(i);
                    }
                }

                if (spawnedCreeps.Count <= maxNumCreeps)
                {
                    GameObject newCreep = Instantiate(creep, baseTransform.position, Quaternion.identity);
                    newCreep.transform.SetParent(transform.parent);
                    AudioManagerScript.instance.Play("ZombieSpawn");
                    // Register Pathfinder
                    Enemy enemyScript = newCreep.GetComponent<Enemy>();
                    if (enemyScript != null)
                    {
                        enemyScript.RegisterPathfinder(pathfinder);
                    }
                    spawnedCreeps.Add(newCreep);
                    nextPossibleAttackTime = Time.time + minAttackInterval;
                }
            }
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

        StateAnticipating();

        if (nextFlinchEndTime <= Time.time)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            currentState = State.Anticipating;
        }
    }
}
