using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    [SerializeField] private RangeCheck hitbox;
    [SerializeField] private float lifespan = 10.0f;

    private Attack defaultAttack = new Attack() { 
        Value = 2,
        Type = Attack.AttackType.Magic,
        Effect = Attack.AttackEffect.None
    };

    private SpriteRenderer spriteRenderer;
    private ParticleSystem particles;

    public void SetAttack(Attack newAttack)
    {
        defaultAttack = newAttack;
    }

    private void Start()
    {
        transform.localScale = Vector3.one;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        particles = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0)
        {
            if (hitbox != null)
            {
                hitbox.gameObject.SetActive(false);
            }
            spriteRenderer.gameObject.SetActive(false);
            particles.Stop();
            Destroy(this.gameObject, 1);
        }
        else if (hitbox != null && hitbox.PlayerIsInRange())
        {
            hitbox.GetPlayerTransform().gameObject.GetComponent<IAttackableByEnemy>().ReceiveAttack(defaultAttack);
        }
    }
}
