using UnityEngine;

//interface used to allows us to have different types of pathing for enemies 
//(ex: ranged enemies run away from the player instead of going for them directly)
public interface IEnemyPathingType
{
    //returns the world position of the tile the Enemy wants to reach
    public Vector2Int GetTargetPos();
}
