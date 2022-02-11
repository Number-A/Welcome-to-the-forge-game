using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : Entity, IAttackableByEnemy
{
    [SerializeField]
    private HUD playerHUD;

    private Weapon currentWeapon;
    private Armour currentArmour;

    private float attackModifier = 1.0f;
    private float defenseModifier = 1.0f;
    private float speedModifier = 1.0f;

    private float invincibleTimer = 0f;
    private float invincibilityDuration = 0.6f;

    private float flashTimer = 0f;
    private float flashDuration = 0.1f;
    private bool isVisible = true;
    // chance that collecting one resource adds two resources.
    private float doubleCollectionChance = 0.0f;

    [SerializeField]
    private PlayerMovement playerController;
    [SerializeField]
    private GameObject deathScreen;

    private GameObject playerSprite;
    public static bool hasWeapon;

    private List<PowerUpCounter> powerUpCollected = new List<PowerUpCounter>();

    private void Start()
    {
        deathScreen.SetActive(false);
        playerHUD.SetupHealth(GetCurrHealth(), GetMaxHealth());
        playerSprite = transform.Find("PlayerSprite").gameObject;
    }


    private void Update() {
        if(GetCurrentWeapon()==null)
        {
            hasWeapon=false;
        }
        else{
            hasWeapon=true;
        }
        if (invincibleTimer > 0) {
            invincibleTimer -= Time.deltaTime;

            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0) {
                isVisible = !isVisible;
                playerSprite.SetActive(isVisible);
                flashTimer = flashDuration;
            }
        } else if (isVisible == false) {
            isVisible = true;
            playerSprite.SetActive(isVisible);
        }
    }

    public void AddPowerUp(PowerUp p)
    {
        int currNum = -1;
        for (int i = 0; i < powerUpCollected.Count; i++)
        {
            if (p.GetType() == powerUpCollected[i].power.GetType())
            {
                PowerUpCounter counter = powerUpCollected[i];
                counter.numCollected++;
                currNum = counter.numCollected;
                break;
            }
        }

        if (currNum == -1)
        {
            powerUpCollected.Add(new PowerUpCounter(p, 1));
            currNum = 1;
        }

        PauseMenu.SetPowerUp(p, currNum);
    }

    public void LoadPlayerData(PlayerData data)
    {
        if (data.inventory.equippedWeaponIndex != -1) 
        {
            SetCurrentWeapon(InventoryManager.GetWeaponAtIndex(data.inventory.equippedWeaponIndex));
        }
        else
        {
            SetCurrentWeapon(null);
        }

        if (data.inventory.equippedArmourIndex != -1)
        {
            SetCurrentArmour(InventoryManager.GetArmourAtIndex(data.inventory.equippedArmourIndex));
        }
        else
        {
            SetCurrentArmour(null);
        }

        SetCurrentHealth(data.playerCurrHealth);
        SetMaxHealth(data.playerMaxHealth);
    }

    public Weapon GetCurrentWeapon() { return currentWeapon; }
    public Armour GetCurrentArmour() { return currentArmour; }
    public void SetCurrentWeapon(Weapon w)
    {
        currentWeapon = w;
        playerController.SetWeapon(w);
    }
    
    public void SetCurrentArmour(Armour a)
    {
        currentArmour = a;
        playerController.SetArmour(a);
    }
    
    public override float GetAttack()
    {
        float attack = base.GetAttack();
        if (currentWeapon != null)
        {
            attack += currentWeapon.GetAttack();
        }
        return attack * attackModifier;
    }
    
    public override float GetDefense()
    {
        if (currentArmour != null)
        {
            return (base.GetDefense() + currentArmour.GetDefense()) * defenseModifier;
        }
        return base.GetDefense() * defenseModifier;
    }

    public override bool TakeDamage(float damage)
    {
        if (invincibleTimer > 0)
        {
            return false;
        }
        bool baseResult = base.TakeDamage(damage);
        playerHUD.UpdateHealth(GetCurrHealth(), GetMaxHealth());


        if (GetCurrHealth() <= 0.0f)
        {
            AudioManagerScript.instance.Play("DeathSound");
            Die();
            return baseResult;
        }

        invincibleTimer = invincibilityDuration;

        // Play a random sound when the player is hurt
        int soundIndex = Random.Range(1, 5);

        switch (soundIndex)
        {
            case 1:
                AudioManagerScript.instance.Play("HurtSound1");
                break;
            case 2:
                AudioManagerScript.instance.Play("HurtSound2");
                break;
            case 3:
                AudioManagerScript.instance.Play("HurtSound3");
                break;
            case 4:
                AudioManagerScript.instance.Play("HurtSound4");
                break;
            default:
                Debug.LogError("Invalid hurt sound index returned: " + soundIndex);
                break;

        }
        return baseResult;
    }

    public override float GetMoveSpeed()
    {
        return base.GetMoveSpeed() * speedModifier;
    }

    public override bool ReceiveAttack(Attack attack)
    {
        if (attack.Type == Attack.AttackType.Effect)
        {
            if (attack.Effect == Attack.AttackEffect.Entangle)
            {
                playerController.Entangle(attack.Value);
            }
            else if (attack.Effect == Attack.AttackEffect.Confuse)
            {
                playerController.Confuse(attack.Value);
            }
            return false;
        }
        if (invincibleTimer > 0) {
            return false;
        }
        TakeDamage(attack.Value);
        return GetCurrHealth() <= 0;
    }

    /**
     * Add the given modifier value to the player's attack modifier.
     * To add 15% to the player's attack, pass 0.15f as the modifier to add.
     * To reduce the player's attack by 50%, pass -0.5f as the modifier to add.
     * Modifiers are additive: if the player currently has a 50% attack reduction
     * as modifier, adding 15% will put the overall modifier to 0.65f.
     */
    public void AddToAttackModifier(float modifier)
    {
        attackModifier += modifier;
    }

    public void RemoveFromAttackModifier(float modifier)
    {
        attackModifier -= modifier;
    }

    public void ResetAttackModifier()
    {
        attackModifier = 1.0f;
    }

    public void AddToDefenseModifier(float modifier)
    {
        defenseModifier += modifier;
    }

    public void RemoveFromDefenseModifier(float modifier)
    {
        defenseModifier -= modifier;
    }

    public void ResetDefenseModifier()
    {
        defenseModifier = 1.0f;
    }

    public void AddToSpeedModifier(float modifier)
    {
        speedModifier += modifier;
    }

    public void RemoveFromSpeedModifier(float modifier)
    {
        speedModifier -= modifier;
    }

    public void ResetSpeedModifier()
    {
        speedModifier = 1.0f;
    }

    public void AddToDoubleCollectionChance(float chance)
    {
        doubleCollectionChance += chance;
    }

    public void RemoveFromDoubleCollectionChance(float chance)
    {
        doubleCollectionChance -= chance;
    }

    public float GetDoubleCollectionChance() { return doubleCollectionChance; }

    public void Heal(float hp)
    {
       if (GetCurrHealth() + hp > GetMaxHealth())
            SetCurrentHealth(GetMaxHealth());
        else
            SetCurrentHealth(GetCurrHealth() + hp);
        playerHUD.UpdateHealth(GetCurrHealth(), GetMaxHealth());
    }

    private class PowerUpCounter
    {
        public PowerUp power;
        public int numCollected;

        public PowerUpCounter(PowerUp p, int num)
        {
            power = p;
            numCollected = num;
        }
    }

    private void Die()
    {
        AudioManagerScript.instance.Play("DeathSound");
        deathScreen.SetActive(true);
        playerHUD.gameObject.SetActive(false);
        playerController.enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
