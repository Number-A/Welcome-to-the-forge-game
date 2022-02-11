using UnityEngine;

public class RangeCheck : MonoBehaviour
{
    private bool m_playerIsInRange = false;
    private Transform playerTransform;

    public bool PlayerIsInRange() { 
        return m_playerIsInRange; 
    }

    public Transform GetPlayerTransform() { 
        return playerTransform; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTransform = collision.gameObject.transform;
            m_playerIsInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_playerIsInRange = false;
        }
    }
}
