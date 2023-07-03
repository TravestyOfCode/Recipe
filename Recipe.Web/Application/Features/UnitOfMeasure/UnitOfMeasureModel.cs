namespace Recipe.Web.Application.Features.UnitOfMeasure;

public class UnitOfMeasureModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Abbreviation { get; set; }

    public decimal? ConversionToGramsRatio { get; set; }
}

public class UnitOfMeasureListModel
{
    public PageQueryResult PageResult { get; set; }

    public IList<UnitOfMeasureModel> UnitOfMeasures { get; set; }

    public UnitOfMeasureListModel(IList<UnitOfMeasureModel> unitOfMeasures, PageQuery query, double totalCount)
    {
        PageResult = new PageQueryResult(query.Page, query.PerPage, query.SortBy, query.SortOrder, Math.Ceiling(totalCount / query.PerPage.Value));

        UnitOfMeasures = unitOfMeasures;
    }
}
