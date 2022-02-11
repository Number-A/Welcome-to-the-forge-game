using UnityEngine;
using TMPro;

public class CraftingList : MonoBehaviour
{
    [SerializeField]
    private GameObject recipeHolderPrefab;

    [SerializeField]
    private Transform craftingListUI;

    [SerializeField]
    private TextMeshProUGUI seeAffordableRecipesButtonText;

    private bool craftableOnly = false;
    private static CraftingList Instance;
    private static bool seeAffordableRecipes = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BuildUI();
    }

    //temp
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearUI();
        }
    }
    /////////////

    private void AddRecipeToUI(CraftableItem recipe)
    {
        GameObject recipeHolderObject = Instantiate(recipeHolderPrefab, transform);

        RecipeHolder holder = recipeHolderObject.GetComponent<RecipeHolder>();
        holder.SetRecipe(recipe);
    }

    public static void BuildUI()
    {
        if (Instance == null)
        {
            return;
        }

        Instance.ClearUI();
        foreach(CraftableItem recipe in Crafting.GetRecipes()){
            
            if (recipe.IsUnlocked())
            {
                if (!seeAffordableRecipes || Crafting.CanAfford(recipe))
                {
                    Instance.AddRecipeToUI(recipe);
                }
            }

            /*
            if(recipe.IsUnlocked() && !seeAffordableRecipes) 
            {
                if(!Instance.craftableOnly || Crafting.CanAfford(recipe)){
                    Instance.AddRecipeToUI(recipe);
                
                }
            }else if(recipe.IsUnlocked() && seeAffordableRecipes)
            {
                if (!Instance.craftableOnly && Crafting.CanAfford(recipe))
                {
                    Instance.AddRecipeToUI(recipe);

                }
            }*/
        }
    }

    private void ClearUI()
    {
        for(int i = 0; i < craftingListUI.childCount; i++)
        {
            Destroy(craftingListUI.GetChild(i).gameObject);
        }
    }

    public void SeeAffordableRecipesUI(){
        seeAffordableRecipes = !seeAffordableRecipes;
        BuildUI();
        if(seeAffordableRecipes)
        {
            seeAffordableRecipesButtonText.SetText("All\nRecipes");
        }else{
            seeAffordableRecipesButtonText.SetText("Craftable\nRecipes");
        }
        
    }
}
