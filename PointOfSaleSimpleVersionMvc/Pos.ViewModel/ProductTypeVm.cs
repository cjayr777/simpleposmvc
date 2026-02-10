using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Pos.ViewModel;

public class ProductTypeVm
{
    [DisplayName("Id")]
    public int ProductTypeId { get; set; }

    [DisplayName("Product Type")]
    [Required]
    public string Name { get; set; }

    public string? Details { get; set; }

    [DisplayName("Sort Order")]
    [Range(0, int.MaxValue, ErrorMessage = "This cannot be negative.")]
    public int SortOrder { get; set; }

    [DisplayName("Stat Id")]
    public int StatId { get; set; }

    [DisplayName("Status")]
    public string StatName { get; set; }

    public string? Search { get; set; }

    [DisplayName("Actioner Id")]
    public int ActionerId { get; set; }

    [DisplayName("Actioner Name")]
    public string ActionerName { get; set; }

    public string ProtectedId { get; set; }


    public ProductTypeVm()
    {
        ProductTypeId = 0;
        Name = "";
        Details = "";
        SortOrder = 0;
        StatId = 0;
        StatName = "";
        Search = "";
        ActionerId = 0;
        ActionerName = "";
        ProtectedId = "0";
    }

}
