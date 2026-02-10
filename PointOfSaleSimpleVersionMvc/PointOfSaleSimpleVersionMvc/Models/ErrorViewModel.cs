namespace PointOfSaleSimpleVersionMvc.Models
{
    public class ErrorViewModela
    {
        public string? RequestIda { get; set; }

        public bool ShowRequestIda => !string.IsNullOrEmpty(RequestIda);
    }
}
