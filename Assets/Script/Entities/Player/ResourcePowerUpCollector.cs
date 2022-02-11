using UnityEngine;

public class ResourcePowerUpCollector : MonoBehaviour
{
    private Player player;
    private PlayerMovement playerMovement;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        ResourcePickup pickup = col.gameObject.GetComponent<ResourcePickup>();

        if (pickup != null) 
        {
            // chance that the collected resource is doubled
            if (Random.value < player.GetDoubleCollectionChance())
                InventoryManager.AddResource(pickup.GetResourceType(), 2);
            else
                InventoryManager.AddResource(pickup.GetResourceType(), 1);
            Destroy(col.gameObject);
        }
        else
        {
            PowerUp powerUp = col.gameObject.GetComponent<PowerUp>();
            
            if (powerUp != null)
            {
                if (powerUp is HealPlayer && player.GetCurrHealth() >= player.GetMaxHealth())
                    return;  // prevent wasting the power up

                // Update power up parent so it doesn't get deactivated as rooms are toggled on and off
                powerUp.transform.SetParent(transform.parent);
                powerUp.Collect(player, playerMovement);
                // Do not delete the power up since it's script needs to still be in the scene for 
                // the logic of the power up to continue working (deleting of the power up game object 
                // is the responsability of the power up itself)
            }
        }
    }
}
