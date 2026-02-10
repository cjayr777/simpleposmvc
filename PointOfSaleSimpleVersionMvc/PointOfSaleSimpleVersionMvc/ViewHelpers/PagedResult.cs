namespace PointOfSaleSimpleVersionMvc.ViewHelpers;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages
    {
        get
        {
            return (int)Math.Ceiling(TotalItems / (double)PageSize);
        }
    }

    public bool HasPrevious
    {
        get
        {
            return PageNumber > 1;
        }
    }

    public bool HasNext
    {
        get
        {
            return PageNumber < TotalPages;
        }
    }
}
