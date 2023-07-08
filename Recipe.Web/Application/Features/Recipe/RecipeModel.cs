using Recipe.Web.Application.Features.Category;
using Recipe.Web.Application.Features.Ingredient;

namespace Recipe.Web.Application.Features.Recipe;

public class RecipeModel
{
    public int Id { get; set; }

    public string Title { get; set; }

    // public string Categories { get; set; }

    public List<CategoryModel> Categories { get; set; }

    public string Description { get; set; }

    public List<IngredientModel> Ingredients { get; set; }

    public string Instructions { get; set; }
}

public class RecipeListModel
{
    public PageQueryResult PageResult { get; set; }

    public List<RecipeModel> Recipes { get; set; }

    public RecipeListModel(List<RecipeModel> recipes, PageQuery query, double totalCount)
    {
        PageResult = new PageQueryResult(query.Page, query.PerPage, query.SortBy, query.SortOrder, totalCount);

        Recipes = recipes;
    }
}

