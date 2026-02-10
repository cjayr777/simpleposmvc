namespace Pos.Model;

public class ProductType
{
    public int ProductTypeId { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public int SortOrder { get; set; }
    public int StatId { get; set; }


    public ProductType()
    {
        ProductTypeId = 0;
        Name = "";
        Details = "";
        SortOrder = 0;
        StatId = 0;
    }

}
