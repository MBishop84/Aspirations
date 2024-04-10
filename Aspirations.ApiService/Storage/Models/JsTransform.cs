namespace Aspirations.ApiService.Storage.Models
{
    public class JsTransform
    {
        public int Id { get; set; }
        public string? AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
