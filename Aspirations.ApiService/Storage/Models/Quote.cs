namespace Aspirations.ApiService.Storage.Models
{
    public class Quote
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
