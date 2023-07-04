namespace Recipe.Web.Application.Features.Product;

public class ProductModel
{
    public int Id { get; set; }

    public string Name { get; set; }
}

public class ProductListModel
{
    public PageQueryResult PageResult { get; set; }

    public IList<ProductModel> Products { get; set; }

    public ProductListModel(IList<ProductModel> products, PageQuery query, double totalCount)
    {
        PageResult = new PageQueryResult(query.Page, query.PerPage, query.SortBy, query.SortOrder, totalCount);

        Products = products;
    }
}
