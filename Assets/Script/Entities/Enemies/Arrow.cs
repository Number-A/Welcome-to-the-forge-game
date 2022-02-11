using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float travelSpeed = 2.5f;
    [SerializeField] private float maxTravelDistance = 100.0f;
    [SerializeField] private Vector2 direction = new Vector2(1, 0);
    [SerializeField] private float stopTime = 0.5f;

    private CircleCollider2D hitbox;
    private Rigidbody2D rb;
    private float currentTravelDistance = 0;

    private Attack atk;

    public void SetAttack(Attack atk)
    {
        this.atk = atk;
    }

    public float GetTravelSpeed()
    {
        return travelSpeed;
    }
    public void SetTravelSpeed(float newTravelSpeed)
    {
        travelSpeed = newTravelSpeed;
    }

    public float GetMaxTravelDistance()
    {
        return maxTravelDistance;
    }
    public void SetMaxTravelDistance(float newMaxTravelDistance)
    {
        maxTravelDistance = newMaxTravelDistance;
    }

    public Vector2 GetDirection()
    {
        return direction;
    }
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
    }

    void Start()
    {
        // Normalize direction vector
        direction = direction.normalized;

        // Set arrow rotation
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // Get components
        hitbox = GetComponentInChildren<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (travelSpeed != 0 && currentTravelDistance < maxTravelDistance)
        {
            // Move arrow
            Vector2 prevPosition2D = new Vector2(rb.position.x, rb.position.y);
            rb.position = Vector2.MoveTowards(prevPosition2D, prevPosition2D + direction, travelSpeed * Time.fixedDeltaTime);

            // Update travel distance
            currentTravelDistance += (new Vector2(rb.position.x, rb.position.y) - prevPosition2D).magnitude;

        }
        else if (currentTravelDistance >= maxTravelDistance)
        {
            // Destroy arrow if max travel distance reached
            Invoke("DestroyArrow", 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IAttackable target = collision.gameObject.GetComponent<IAttackable>();
        
        if (target != null)
        {
            // Only deal damage if a player arrow hit an enemy or if an enemy arrow hit the player
            if (atk.Type != Attack.AttackType.Bow && !collision.gameObject.CompareTag("Player"))
            {
                return;
            }
            // Inflict damage
            target.ReceiveAttack(atk);
        }

        // Stop, disable hitbox and destroy arrow
        rb.bodyType = RigidbodyType2D.Static;
        travelSpeed = 0;
        hitbox.enabled = false;
        Invoke("DestroyArrow", stopTime);
    }

    public void DestroyArrow()
    {
        Destroy(gameObject);
    }
}
