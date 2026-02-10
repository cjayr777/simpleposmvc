namespace Pos.Model;

public class Role
{
    public int RoleId { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public int SortOrder { get; set; }
    public int StatId { get; set; }


    public Role()
    {
        RoleId = 0;
        Name = "";
        Details = "";
        SortOrder = 0;
        StatId = 0;
    }

}
