using UnityEngine;

public class MeleePathing : MonoBehaviour, IEnemyPathingType
{
    [SerializeField]
    private Transform playerTransform;

    public Vector2Int GetTargetPos()
    {
        return new Vector2Int((int)playerTransform.position.x, (int)playerTransform.position.y);
    }
}
