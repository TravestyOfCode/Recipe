using Recipe.Web.Application.Features.UnitOfMeasure;

namespace Recipe.Web.Application.Features.Ingredient;

public class IngredientModel
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public decimal Quantity { get; set; }

    public string Product { get; set; }

    public int? UnitOfMeasureId { get; set; }

    public UnitOfMeasureModel UnitOfMeasure { get; set; }
}
