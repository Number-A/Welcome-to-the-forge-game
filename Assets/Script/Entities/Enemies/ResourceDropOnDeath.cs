using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDropOnDeath : MonoBehaviour
{
    [SerializeField]
    private enum EnemyType {Bone, SpiritEssence, Stone, Vine };

    [SerializeField]
    private EnemyType type;

    [SerializeField]
    private GameObject[] resources = new GameObject[InventoryManager.GetNumResources()];

    [SerializeField]
    private int minResourceDrops = 1;

    [SerializeField]
    private int maxResourceDrops = 3;

    [SerializeField]
    private float resourceDropRange = 1;

    private void OnDestroy()
    {
        int numResources = Random.Range(minResourceDrops, maxResourceDrops);
        float xPos = Random.Range(-resourceDropRange, resourceDropRange);
        float yPos = Random.Range(-resourceDropRange, resourceDropRange);

        Instantiate(resources[(int)type], new Vector3(this.transform.position.x + xPos, this.transform.position.y + yPos, this.transform.position.z), Quaternion.identity);
    }
}
