using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeHolder : MonoBehaviour
{

    [SerializeField]
    private Image recipeSprite;

    [SerializeField]
    private ResourceSlot[] resources;

    private CraftableItem recipe;

    [SerializeField]
    private Sprite boneSprite;
    [SerializeField]
    private Sprite spiritEssenceSprite;
    [SerializeField]
    private Sprite stoneSprite;
    [SerializeField]
    private Sprite vineSprite;

    [SerializeField]
    private Button craftingButton;

    [SerializeField]
    private TextMeshProUGUI infoText;

    private Dictionary<Enums.ResourceType, int> recipeCost;

    [SerializeField]
    private float armourSize = 30f;

    public void SetRecipeSprite(CraftableItem item){
        recipeSprite.sprite = item.GetSprite();
        
        if (item.GetItemType().ToString() == "Armour") 
        {
           recipeSprite.color = ArmourColorManager.GetArmourColor(((Armour)item).GetArmourType());

            var tempColor = recipeSprite.color;

            tempColor.a = 1f;

            recipeSprite.color = tempColor;

            recipeSprite.rectTransform.sizeDelta = new Vector2(armourSize, armourSize);
        }
    }

    public void SetRecipe(CraftableItem item)
    {
        recipe = item;

        SetRecipeSprite(recipe);
        SetRecipeCost(recipe.GetCosts());

        if(recipe.GetItemType().ToString()=="Weapon"){
            infoText.SetText(recipe.GetName()+"\n"+((Weapon)recipe).GetWeaponType()+" (Lvl"+recipe.GetLevel()+")");
        }
        else if (recipe.GetItemType().ToString() == "Armour")
        {
            infoText.SetText(recipe.GetName() + "\n" + ((Armour)recipe).GetArmourType() + " (Lvl" + recipe.GetLevel() + ")");
        }

        UpdateButtonState();
    }

    private void SetRecipeCost(Dictionary<Enums.ResourceType,int> cost){
        int resourceSlotIndex = 0;
        recipeCost = cost;
        foreach(Enums.ResourceType type in Enum.GetValues(typeof(Enums.ResourceType))){
             if(cost.ContainsKey(type)){
                resources[resourceSlotIndex].SetActive(true);
                resources[resourceSlotIndex].sprite.sprite = GetSpriteByResource(type);
                resources[resourceSlotIndex].text.SetText("{0}", cost[type]);
                resourceSlotIndex++;
             }

        }

        for(int i=resourceSlotIndex;i<resources.Length;i++){
            resources[i].SetActive(false);
        }
    }

    public void OnCraftClick(){
        foreach(var pair in recipe.GetCosts()){
            InventoryManager.RemoveResource(pair.Key, pair.Value);
        }
        InventoryManager.AddItemToInventory(recipe.Clone());

        if(!Crafting.CanAfford(recipe)){
            CraftingList.BuildUI();
        }


    }

    private Sprite GetSpriteByResource(Enums.ResourceType type)
    {
        switch (type){
            case Enums.ResourceType.Bone:
                return boneSprite;
            case Enums.ResourceType.SpiritEssence:
                return spiritEssenceSprite;
            case Enums.ResourceType.Stone:
                return stoneSprite;
            case Enums.ResourceType.Vine:
                return vineSprite;
        }
        return null;
    }


    //Allow the button to be clicked if enough resources
    public void UpdateButtonState(){
        if(!Crafting.CanAfford(recipe)){
            craftingButton.interactable = false;
        }else{
            craftingButton.interactable = true;
        }
    }

    [Serializable]
    public struct ResourceSlot {
        public Image sprite;
        public TextMeshProUGUI text;


        public void SetActive(bool status){
            sprite.enabled = status;
            text.enabled = status;
        }
    }


}
