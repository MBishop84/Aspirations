namespace Aspirations.ApiService.Storage.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Quote> Quotes { get; set; }
    }
}
