using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity), typeof(Rigidbody2D))]
public class EnemyPathing : MonoBehaviour
{
    [SerializeField]
    private float pathingRefreshRate = 0.1f;
    [SerializeField]
    private float stopRange = 0.5f;

    private IEnemyPathingType m_pathingObj;
    private float m_currRefreshTimer = 0.0f;

    private bool m_playerInRange = false;
    private List<Vector2Int> m_currPath;
    private int m_currTileIndex = 0; //keeps track of the current tile in the current path

    private Rigidbody2D m_rb;
    private Entity m_entity;

    private RangeCheck m_rangeCheck;

    private void Start()
    {
        m_rangeCheck = GetComponentInChildren<RangeCheck>();
        m_rb = GetComponent<Rigidbody2D>();
        m_entity = GetComponent<Entity>();
        m_pathingObj = GetComponent<IEnemyPathingType>();
        if (m_pathingObj == null)
        {
            Debug.LogError("No IEnemyPathingType object is attached to enemy " + gameObject.name);
        }

        m_currRefreshTimer = pathingRefreshRate;
    }

    private void Update()
    {        
        CheckPlayerInRange();
        if (m_playerInRange)
        {
            if (m_currRefreshTimer >= pathingRefreshRate)
            {
                m_currRefreshTimer = 0.0f;
                UpdatePath();
            }
            else
            {
                m_currRefreshTimer += Time.deltaTime;
            }
        }
        else
        {
            m_currRefreshTimer = pathingRefreshRate;
        }
    }

    private void FixedUpdate()
    {
        if (m_playerInRange && m_currPath != null 
            && stopRange < Vector2.Distance(transform.position, m_currPath[m_currPath.Count - 1]))
        {
            Vector2 walkDir = GetWalkDirection().normalized * m_entity.GetMoveSpeed();
            m_rb.velocity = walkDir;
        }
        else
        {
            m_rb.velocity = Vector2.zero;
        }
    }

    private void UpdatePath()
    {
        Vector2Int currTilePos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        m_currPath = Pathfinding.AStar(currTilePos, m_pathingObj.GetTargetPos());
        m_currTileIndex = 0;
    }

    private Vector2 GetWalkDirection()
    {
        UpdatePositionInPath();
        if (m_currTileIndex == m_currPath.Count - 1)
        {
            return m_currPath[m_currTileIndex] - ((Vector2)transform.position);
        }
        return m_currPath[m_currTileIndex + 1] - ((Vector2)transform.position);
    }

    //updates the current tile in our current path
    private void UpdatePositionInPath()
    {
        float tileDistance = float.MaxValue;
        Vector2 currWorldPos = transform.position;

        for (int i = m_currTileIndex; i < m_currPath.Count; i++)
        {
            float currTileDist = Vector2.Distance(currWorldPos, m_currPath[i]);
            if (currTileDist < tileDistance)
            {
                tileDistance = currTileDist;
            }
            else if (currTileDist > tileDistance)
            {
                m_currTileIndex = i;
                break;
            }
        }
    }

    private void CheckPlayerInRange()
    {
        m_playerInRange = m_rangeCheck.PlayerIsInRange();
    }
}
