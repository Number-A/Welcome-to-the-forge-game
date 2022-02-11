using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IAttackable
{
    [SerializeField]
    private float maxHealth = 20.0f;

    [SerializeField]
    private float currHealth;
    
    [SerializeField]
    private float attack;
    [SerializeField]
    private float defense;

    [SerializeField]
    private float moveSpeed = 100.0f;

    // Provides the listeners with the new health of the entity. might want to replace it with Chi's system once it is implemented
    public event Action<float> onDamaged; 

    public Entity()
    {
        currHealth = maxHealth;
    }

    //getters

    public Vector3 GetPosition() { return transform.position; }

    public float GetMaxHealth() { return maxHealth; }
    public float GetCurrHealth() { return currHealth; }

    public virtual float GetAttack() { return attack; }
    
    public virtual float GetDefense() { return defense; }

    public virtual float GetMoveSpeed() { return moveSpeed; }

    // Returns true if the damage taken > 0
    public virtual bool TakeDamage(float damage)
    {
        if(this.gameObject.tag!="Player")
        {
            if(!Player.hasWeapon)
            {
                //Plays a random sound when the player melee attacks an enemy
                int soundIndex1 = UnityEngine.Random.Range(1, 5);

                    switch (soundIndex1)
                    {
                        case 1:
                            AudioManagerScript.instance.Play("Punch1");
                            break;
                        case 2:
                            AudioManagerScript.instance.Play("Punch2");
                            break;
                        case 3:
                            AudioManagerScript.instance.Play("Punch3");
                            break;
                        case 4:
                            AudioManagerScript.instance.Play("Punch4");
                            break;
                        default:
                            Debug.LogError("Invalid hurt sound index returned: " + soundIndex1);
                            break;
                    }
                
            }
            else{
            //Plays a random sound when the player swings
            int soundIndex2 = UnityEngine.Random.Range(1, 6);

                    switch (soundIndex2)
                    {
                        case 1:
                            AudioManagerScript.instance.Play("Swing1");
                            break;
                        case 2:
                            AudioManagerScript.instance.Play("Swing2");
                            break;
                        case 3:
                            AudioManagerScript.instance.Play("Swing3");
                            break;
                        case 4:
                            AudioManagerScript.instance.Play("Swing4");
                            break;
                        case 5:
                            AudioManagerScript.instance.Play("Swing5");
                            break;
                        default:
                            Debug.LogError("Invalid hurt sound index returned: " + soundIndex2);
                            break;

                    }
            }
        }
        //might want to change the math here
        float actualDamage = damage - GetDefense();
        if (actualDamage < 0.0f)
        {
            actualDamage = UnityEngine.Random.Range(1, 3);
        }
        currHealth -= actualDamage;

        if (currHealth <= 0.0f)
        {
            currHealth = 0.0f;
            if (onDamaged != null)
            {
                onDamaged(currHealth);
            }
            return true;
        }

        if (onDamaged != null)
        {
            onDamaged(currHealth);
        }
        return true;
    }

    public abstract bool ReceiveAttack(Attack atk);

    // Used when instantiating enemies who have been damaged previously when loading a previously saved dungeon
    public void SetCurrentHealth(float healthVal)
    {
        if (healthVal < 0)
        {
            return;
        }

        this.currHealth = healthVal;
    }

    public void SetMaxHealth(float healthVal)
    {
        if (healthVal > 0)
        {
            maxHealth = healthVal;
        }
    }
}
