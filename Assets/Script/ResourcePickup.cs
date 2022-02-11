using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    [SerializeField]
    private Enums.ResourceType type;

    public Enums.ResourceType GetResourceType() { return type; }
}
