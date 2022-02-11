using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoEAppendage : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private float attackInterval = 3.0f;
    [SerializeField] Transform projectileSpawnPoint;

    private Transform playerTransform;
    private float nextAttackTime = 0;
    private bool attacking = false;
    private Attack defaultAttack = new Attack() {
        Value = 3,
        Type = Attack.AttackType.Piercing,
        Effect = Attack.AttackEffect.None
    };

    private void Update()
    {
        if (attacking && nextAttackTime <= Time.time)
        {
            if (projectile != null && playerTransform != null)
            {
                nextAttackTime = Time.time + attackInterval;
                if (projectileSpawnPoint == null)
                {
                    projectileSpawnPoint = transform;
                }
                GameObject newProjectile = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);
                newProjectile.transform.SetParent(transform.parent);
                Projectile projectileScript = newProjectile.GetComponent<Projectile>();
                projectileScript.SetAttack(defaultAttack);
                projectileScript.SetDirection(new Vector2(playerTransform.position.x - projectileSpawnPoint.position.x, playerTransform.position.y - projectileSpawnPoint.position.y));
            }
        }
    }

    public void StartAttacking()
    {
        attacking = true;
    }

    public void StopAttacking()
    {
        attacking = false;
    }

    public void SetAttack(Attack newAttack)
    {
        defaultAttack = newAttack;
    }

    public void SetPlayerTransform(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }
}
