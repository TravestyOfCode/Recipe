namespace Recipe.Web.Application.Features.Category;

public class CategoryModel
{
    public int Id { get; set; }

    public string Name { get; set; }
}

public class CategoryListModel
{
    public PageQueryResult PageResult { get; set; }

    public IList<CategoryModel> Categories { get; set; }

    public CategoryListModel(IList<CategoryModel> categories, PageQuery query, double totalCount)
    {
        PageResult = new PageQueryResult(query.Page, query.PerPage, query.SortBy, query.SortOrder, totalCount);

        Categories = categories;
    }
}
