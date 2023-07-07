using Recipe.Web.Application.Features.Recipe;

namespace Recipe.Web.ViewModels.Recipe;

public class RecipeEditViewModel
{
    public RecipeModel Recipe { get; set; }

    public Dictionary<int, string> UnitOfMeasures { get; set; }
}
