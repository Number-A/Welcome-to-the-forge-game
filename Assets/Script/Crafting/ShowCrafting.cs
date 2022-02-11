using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCrafting : MonoBehaviour
{
    [SerializeField]
    private GameObject CraftingPrefab;
    private bool isCrafting=false;
    private bool islooping=true;

    private void Start()
    {
         if(!InputManager.Contains("Crafting")){
            InputManager.Set("Crafting", KeyCode.Tab);
         }
    }
    private void Update()
    {
        if(isCrafting&&islooping)
        {
            if(InputManager.GetKeyDown("Crafting"))
            {
                CraftingPrefab.SetActive(true);
                islooping=false;
                isCrafting=false;
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name=="Player")
        {
            isCrafting=true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
         if(other.name=="Player")
        {
            isCrafting=false;
            islooping=true;
            CraftingPrefab.SetActive(false);

        }
    }
}
