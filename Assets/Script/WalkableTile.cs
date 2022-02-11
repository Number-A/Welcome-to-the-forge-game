using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkableTile : MonoBehaviour
{
    [SerializeField]
    private Tilemap obstacles;
    [SerializeField]
    private Tilemap walls;
    
    public bool isWalkable(Vector2Int coord)
    {
        return obstacles.GetTile(new Vector3Int(coord.x, coord.y,0))==null&&walls.GetTile(new Vector3Int(coord.x, coord.y,0))==null;
    }
} 
