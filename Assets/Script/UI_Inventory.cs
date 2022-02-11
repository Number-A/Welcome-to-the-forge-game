using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField]
    private int recipeIndex;
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        IsCraftable();
    }

    private void IsCraftable() 
    {
        if (Crafting.CanAfford(Crafting.GetRecipes()[recipeIndex]))
        {
            button.interactable = true;
        }
        else 
        {
            button.interactable = false;
        }
    }

    public void CraftWeapon1() 
    {
        Crafting.CraftItemByIndex(0);
    }
    public void CraftWeapon2() 
    {
        Crafting.CraftItemByIndex(1);
    }
    public void CraftWeapon3() 
    {
        Crafting.CraftItemByIndex(2);
    }
    public void CraftWeapon4() 
    {
        Crafting.CraftItemByIndex(3);
    }



}
