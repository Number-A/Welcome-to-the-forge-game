using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Player))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowPrefab;

    private Player player;
    private List<IAttackable> damageablesInRange;
    private float attackDuration = 0.0f;
    private float currAtkTimer = 0.0f;
    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Vector2Int facingDir = Vector2Int.up;
    bool isAttacking = false;
    bool isSpriteFlipped = false;

    [SerializeField]
    private GameObject confusionQuestionMark;

    [SerializeField]
    private float bowShootingCooldown = 0.3f;
    private float bowShootingTimer;

    [SerializeField]
    private Animator baseAnimator;
    [SerializeField]
    private Animator armourAnimator;
    [SerializeField]
    private Animator weaponAnimator;

    [SerializeField]
    private RuntimeAnimatorController[] weaponControllers;

    

    private float moveScalar = 1.0f; // Used to invert controls when confused

    private bool entangled = false;
    private bool confused = false;

    private SpriteRenderer armourSpriteRenderer;

    private Camera cam;
    private bool isWalking=false;

    private ArmourColorManager armourColorManager;

    // Unity callbacks
    private void Start()
    {
        bowShootingTimer = bowShootingCooldown;
        cam = Camera.main;

        confusionQuestionMark.SetActive(false);

        //check if input manager keybindings have been set
        if (!InputManager.Contains("Move Left"))
        {
            InputManager.Set("Move Left", KeyCode.A);
            InputManager.Set("Move Right", KeyCode.D);
            InputManager.Set("Move Up", KeyCode.W);
            InputManager.Set("Move Down", KeyCode.S);
            InputManager.Set("Attack", KeyCode.Mouse0);
        }

        damageablesInRange = new List<IAttackable>();
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        AnimationClip[] animations = baseAnimator.runtimeAnimatorController.animationClips;

        int found = 0;
        foreach (AnimationClip anim in animations)
        {
            if (anim.name == "PlayerAnticipate")
            {
                attackDuration += anim.length;
                found++;
                if (found == 2)
                {
                    break;
                }
            }
            else if (anim.name == "PlayerAttack")
            {
                attackDuration += anim.length;
                found++;
                if (found == 2)
                {
                    break;
                }
            }
        }

        armourColorManager = new ArmourColorManager();

        armourSpriteRenderer = armourAnimator.gameObject.GetComponent<SpriteRenderer>();

        //update armour and weapon animators
        SetArmour(player.GetCurrentArmour());
        SetWeapon(player.GetCurrentWeapon());


    }

    public bool Entangle(float duration)
    {
        if (entangled == true)
        {
            return false;
        }
        entangled = true;
        StartCoroutine("RemoveEntangled", duration);
        return true;
    }

    public bool Confuse(float duration)
    {
        if (confused == true)
        {
            return false;
        }
        confused = true;
        confusionQuestionMark.SetActive(true);
        moveScalar = moveScalar * -1.0f;
        StartCoroutine("RemoveConfused", duration);
        return false;
    }

    private void Update()
    {
        if (InventoryMenuController.IsPaused())
        {
            return;
        }

        //get user input 
        GetUserInput();
        if(moveDir!=Vector2.zero&&isWalking==false)
        {
            isWalking=true;
            AudioManagerScript.instance.Play("WalkSound");
        }
        if(moveDir==Vector2.zero&&isWalking==true)
        {
            isWalking=false;
            AudioManagerScript.instance.Stop("WalkSound");
        }

        if (isAttacking)
        {
            if (currAtkTimer < attackDuration)
            {
                currAtkTimer += Time.deltaTime;
            }
            else
            {
                currAtkTimer = 0.0f;
                isAttacking = false;
            }
        }
        else if (InputManager.GetKey("Attack"))
        {
            Attack();
        }

        if (bowShootingTimer < bowShootingCooldown)
        {
            bowShootingTimer += Time.deltaTime;
        }

        UpdateAnimations();
    }


    private void FixedUpdate()
    {
        if (entangled)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            //no need to mutilply by fixed delta time since the rigibody will handle the actual moving
            rb.velocity = moveDir.normalized * player.GetMoveSpeed() * moveScalar;
        }

        UpdateWeaponRotation(); //temporary since this will get managed by the actual sprites used
    }

    // Public Methods

    public bool IsAttacking() { return this.isAttacking; }


    // Private Helpers
    private void UpdateWeaponRotation()
    {
        if (moveDir.x > 0.0f)
        {
            facingDir = Vector2Int.right;
        }
        else if (moveDir.x < 0.0f)
        {
            facingDir = Vector2Int.left;
        }
        else if (moveDir.y > 0.0f)
        {
            facingDir = Vector2Int.up;
        }
        else if (moveDir.y < 0.0f)
        {
            facingDir = Vector2Int.down;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy") {
            IAttackable target = col.gameObject.GetComponentInParent<IAttackable>();

            if (target != null)
            {
                damageablesInRange.Add(target);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            IAttackable target = col.gameObject.GetComponentInParent<IAttackable>();

            if (target != null)
            {
                damageablesInRange.Remove(target);
            }
        }
    }

    //checks that the player is facing the enemy hit
    private bool TargetIsBeingFaced(Vector3 targetPos)
    {
        if (facingDir == Vector2Int.right)
        {
            return transform.position.x < targetPos.x;
        }
        else if (facingDir == Vector2Int.left)
        {
            return transform.position.x > targetPos.x;
        }
        else if (facingDir == Vector2Int.up)
        {
            return transform.position.y < targetPos.y;
        }
        else //facing down
        {
            return transform.position.y > targetPos.y;
        }
    }

    // Attacks all the targets in range that are currently being faced
    private void MeleeAttack()
    {
        isAttacking = true;
        Attack atk = CreateAttackStruct();
        
        //AudioManagerScript.instance.Play("HittingSound");
        for (int i = 0; i < damageablesInRange.Count; i++)
        {
            try
            {
                if (TargetIsBeingFaced(damageablesInRange[i].GetPosition()))
                {
                    //if target dies remove it from the list
                    if (damageablesInRange[i].ReceiveAttack(atk))
                    {
                        damageablesInRange.RemoveAt(i);
                        i--; //decrement so when the loop increments the value of i stays the same
                    }
                }
            }
            catch (MissingReferenceException)
            {
                // Target was deleted or player changed rooms remove the target from the list and continue
                damageablesInRange.RemoveAt(i);
                i--; // Decrement i to not skip any targets
            }
        }
    }

    private void Attack()
    {
        if (player.GetCurrentWeapon() != null && player.GetCurrentWeapon().GetWeaponType() == Enums.WeaponType.Bow)
        {
            RangeAttack();
            AudioManagerScript.instance.Play("Bow");
        }
        else
        {
            MeleeAttack();
            //Plays a grunt noise whn you attack anything.
           int soundIndex = UnityEngine.Random.Range(1, 4);

                switch (soundIndex)
                {
                    case 1:
                        AudioManagerScript.instance.Play("HittingSound1");
                        break;
                    case 2:
                        AudioManagerScript.instance.Play("HittingSound2");
                        break;
                    case 3:
                        AudioManagerScript.instance.Play("HittingSound3");
                        break;
                    default:
                        Debug.LogError("Invalid hurt sound index returned: " + soundIndex);
                        break;
                }
        }
    }

    private void RangeAttack()
    {
        if (bowShootingTimer < bowShootingCooldown)
        {
            return;
        }
        isAttacking = true;
        Vector3 mousePosWorldCoord = cam.ScreenToWorldPoint(Input.mousePosition);

        Vector2 shootDirection = (Vector2)mousePosWorldCoord - (Vector2)transform.position;
        GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity, transform.parent);
        Projectile a = newArrow.GetComponent<Projectile>();
        a.SetDirection(shootDirection.normalized);

        a.SetAttack(CreateAttackStruct());
        bowShootingTimer = 0.0f;
    }

    private void GetUserInput()
    {
        moveDir = Vector2.zero;
        if (InputManager.GetKey("Move Left"))
        {
            moveDir += Vector2.left;
        }

        if (InputManager.GetKey("Move Right"))
        {
            moveDir += Vector2.right;
        }

        if (InputManager.GetKey("Move Up"))
        {
            moveDir += Vector2.up;
        }

        if (InputManager.GetKey("Move Down"))
        {
            moveDir += Vector2.down;
        }
    }

    private IEnumerator RemoveEntangled(float duration)
    {
        yield return new WaitForSeconds(duration);
        entangled = false;
    }

    private IEnumerator RemoveConfused(float duration)
    {
        yield return new WaitForSeconds(duration);
        moveScalar = -1.0f * moveScalar;
        confused = false;
        confusionQuestionMark.SetActive(false);
    }

    private Attack CreateAttackStruct()
    {
        Attack atk = new Attack();
        atk.Value = player.GetAttack();
        atk.Type = GetWeaponAttackType();
        atk.Effect = global::Attack.AttackEffect.None;
        return atk;
    }

    private Attack.AttackType GetWeaponAttackType()
    {
        Weapon currWeapon = player.GetCurrentWeapon();

        if (currWeapon == null)
        {
            return global::Attack.AttackType.Other;
        }

            //Sword, Axe, Mace, Spear, Bow
        switch (currWeapon.GetWeaponType())
        {
            case Enums.WeaponType.Sword:
                return global::Attack.AttackType.Sword;

            case Enums.WeaponType.Axe:
                return global::Attack.AttackType.Axe;

            case Enums.WeaponType.Spear:
                return global::Attack.AttackType.Spear;

            case Enums.WeaponType.Bow:
                return global::Attack.AttackType.Bow;
        }

        Debug.LogError("Unknown Weapon type");
        return global::Attack.AttackType.Other;
    }

    private void UpdateAnimations()
    {
        if ((isSpriteFlipped && moveDir.x > 0.0f) || (!isSpriteFlipped && moveDir.x < 0.0f))
        {
            FlipSprite();
        }

        baseAnimator.SetBool("IsWalking", moveDir != Vector2.zero);
        baseAnimator.SetBool("IsAttacking", isAttacking);

        if (weaponAnimator.gameObject.activeSelf)
        {
            weaponAnimator.SetBool("IsWalking", moveDir != Vector2.zero);
            weaponAnimator.SetBool("IsAttacking", isAttacking);
        }

        if (armourAnimator.gameObject.activeSelf)
        {
            armourAnimator.SetBool("IsWalking", moveDir != Vector2.zero);
            armourAnimator.SetBool("IsAttacking", isAttacking);
        }
    }

    private void FlipSprite()
    {
        isSpriteFlipped = !isSpriteFlipped;
        confusionQuestionMark.transform.localScale = new Vector3(-1.0f * confusionQuestionMark.transform.localScale.x, 
            confusionQuestionMark.transform.localScale.y, confusionQuestionMark.transform.localScale.z);
        transform.localScale = new Vector3(-1.0f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void SetWeapon(Weapon w)
    {
        if (w == null)
        {
            weaponAnimator.gameObject.SetActive(false);
            return;
        }

        weaponAnimator.gameObject.SetActive(true);
        int controllerIndex = ((int)w.GetWeaponType()) * 3 + w.GetLevel() - 1;
        weaponAnimator.runtimeAnimatorController = weaponControllers[controllerIndex];
    }

    public void SetArmour(Armour a)
    {
        if (a == null)
        {
            armourAnimator.gameObject.SetActive(false);
            return;
        }

        armourAnimator.gameObject.SetActive(true);
        armourSpriteRenderer.color = ArmourColorManager.GetArmourColor(a.GetArmourType());
    }
    
}