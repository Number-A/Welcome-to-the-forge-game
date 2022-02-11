using UnityEngine;

public class LevelSpikes : MonoBehaviour
{

    [SerializeField]
    private float damageDealt=2;
    
    private Player p = null;

    private void Update()
    {
        if (p != null)
        {
            Attack attack = new Attack()
            {
                Value = damageDealt,
                Type = Attack.AttackType.Piercing,
                Effect = Attack.AttackEffect.None
            };
            p.ReceiveAttack(attack);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.name =="Player")
        {
            p = other.gameObject.GetComponent<Player>();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.name == "Player")
        {
            p = null;
        }
    }
}
