namespace Recipe.Web.Data;

public class Recipe
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public AppUser User { get; set; }

    public string Title { get; set; }

    public string Categories { get; set; }

    public string Description { get; set; }

    public List<Ingredient> Ingredients { get; set; }

    public string Instructions { get; set; }
}
