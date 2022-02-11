using UnityEngine.UI;
using UnityEngine;

public class OnHoverButton : MonoBehaviour
{

    [SerializeField]
    private Transform thisButton;

    private float currentScale;

    public float hoverScale = 1.2f;

    private void Start()
    {
        currentScale = transform.localScale.x;
    }

    public void EnterHover(){
        transform.localScale = new Vector3(currentScale* hoverScale, currentScale * hoverScale, 0);
    }

    public void ExitHover(){
        transform.localScale = new Vector3(currentScale, currentScale, 0);
    }
}
