using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pos.ViewModel;

public class RoleVm
{
    [DisplayName("Id")]
    public int RoleId { get; set; }

    [DisplayName("Role")]
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


    public RoleVm()
    {
        RoleId = 0;
        Name = "";
        Details = "";
        SortOrder = 0;
        StatId = 0;
        StatName = "";
        Search = "";
        ActionerId = 0;
        ActionerName = "";
    }

}
