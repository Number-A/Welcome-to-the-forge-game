using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElongateButton : MonoBehaviour
{
    private bool coroutineAllowed;
    // Start is called before the first frame update
    void Start()
    {
        coroutineAllowed = true;
    }

    private void OnMouseOver()
    {
        if(coroutineAllowed){
            Elongate();
        }
    }

    private void Elongate(){
        coroutineAllowed = false;

        transform.localScale = new Vector3(
        (Mathf.Lerp(transform.localScale.x, transform.localScale.x+1, Mathf.SmoothStep(0f,1f,0.5f))),
        (Mathf.Lerp(transform.localScale.y, transform.localScale.y + 1, Mathf.SmoothStep(0f, 1f, 0.5f))),
        (Mathf.Lerp(transform.localScale.z, transform.localScale.z + 1, Mathf.SmoothStep(0f, 1f, 0.5f)))
        );
    }
}
