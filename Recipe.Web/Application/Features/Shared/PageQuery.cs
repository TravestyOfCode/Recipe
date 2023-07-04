using System.Linq;

namespace Recipe.Web.Application.Features.Shared;

public enum SortOrder { ASC, DESC }

public class PageQuery
{
    public int? Page { get; set; }

    public int? PerPage { get; set; }

    public string SortBy { get; set; }

    public SortOrder SortOrder { get; set; }
}

public class PageQueryValidator : AbstractValidator<PageQuery>
{
    public PageQueryValidator()
    {
        RuleFor(p => p.Page)
            .GreaterThan(0);

        RuleFor(p => p.PerPage)
            .GreaterThan(0);
    }
}

public record PageQueryResult(int? Page, int? PerPage, string SortBy, SortOrder SortOrder, double TotalCount)
{
    public double TotalPages
    {
        get
        {
            if (PerPage == null || PerPage.Value == 0 || TotalCount == 0)
            {
                return 0;
            }

            return Math.Ceiling(TotalCount / PerPage.Value);
        }
    }
}

public static class PageQueryExtensions
{
    public static IQueryable<T> AsPageQuery<T>(this IQueryable<T> query, PageQuery page)
    {
        if (query == null || page == null)
            return query;

        IQueryable<T> result = query;

        if (!string.IsNullOrWhiteSpace(page.SortBy))
        {
            if (page.SortOrder == SortOrder.ASC)
            {
                result = result.OrderBy(p => EF.Property<object>(p!, page.SortBy));
            }
            else
            {
                result = result.OrderByDescending(p => EF.Property<object>(p!, page.SortBy));
            }
        }

        if (page.Page != null)
        {
            page.PerPage ??= 10;

            result = result.Skip((page.Page.Value - 1) * page.PerPage.Value).Take(page.PerPage.Value);
        }

        return result;
    }
}

