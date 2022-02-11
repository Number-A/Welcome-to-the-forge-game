using UnityEngine;
using System.Collections;


// Abstract class implementing the shared behavior between power ups and providing an 
// interface for the rest of the power up framework
[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public abstract class PowerUp : MonoBehaviour
{
    private BoxCollider2D collider;
    private SpriteRenderer spriteRenderer;

    private bool isCollected = false;
    
    // Duration in seconds of the effect of the power up. 0 means power up is always active.
    [SerializeField]
    protected float activeTime = 0;

    protected bool addToPowerupScreen = true;

    protected void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Collect(Player p, PlayerMovement movement)
    {
        RemoveFromScene();
        if (isCollected)
        {
            return;
        }
        isCollected = true;
        AudioManagerScript.instance.Play("Power");
        ApplyPowerUp(p, movement);
    }

    /**
     * Used to remove an active power up when another is picked up or when the time expires
     */
    public void Remove(Player p, PlayerMovement m)
    {
        DeactivateEffect(p, m);
        Destroy(gameObject);
    }

    // Abstract method meant to be implemented by child classes. Gets called whenever 
    // the player picks up the power up. The Player and PlayerMovement objects are passed 
    // as arguments to allow power ups to interact with the player components
    protected abstract void ActivateEffect(Player p, PlayerMovement movement);

    // Abstract method meant to be implemented by child classes. Gets called when
    // the active time of the power up expires for power ups with limited duration.
    protected abstract void DeactivateEffect(Player p, PlayerMovement movement);

    private void ApplyPowerUp(Player p, PlayerMovement m) {
        ActivateEffect(p, m);

        if (addToPowerupScreen)
        {
            p.AddPowerUp(this);
        }
        
        if (activeTime > 0) 
        {
            StartCoroutine(CountDown(p, m));
        }
    }

    // Deactivates the sprite and collider of the power so the power up is effectively removed from the scene
    private void RemoveFromScene()
    {
        spriteRenderer.enabled = false;
        if(collider!=null)
        {
            collider.enabled = false;
        }
    }

    private IEnumerator CountDown(Player p, PlayerMovement movement)
    {
        yield return new WaitForSeconds(activeTime);
        Remove(p, movement);
        Destroy(gameObject);
    }
}
