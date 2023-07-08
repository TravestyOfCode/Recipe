namespace Recipe.Web.Application.Features.Recipe;

public class CreateRecipeModel
{
    public string Title { get; set; }

    public string Categories { get; set; }

    public List<string> CategoryList { get; set; }

    public string Description { get; set; }

    public string Instructions { get; set; }

    public Dictionary<int, string> UnitOfMeasures { get; set; }

    public CreateRecipeModel(Dictionary<int, string> unitOfMeasures) => UnitOfMeasures = unitOfMeasures;
}
