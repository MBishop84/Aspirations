using Aspirations.ApiService.Storage.Models;
using Aspirations.ApiService.Storage.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Aspirations.ApiService.Storage.Repositories
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly ApiContext _context;

        public QuoteRepository(ApiContext context)
        {
            _context = context;

            if (_context.Quotes.Any()) return;

            var quotes = JsonConvert.DeserializeObject<IEnumerable<Quote>>(
                File.ReadAllText("Storage/Data/quotes.json"));

            if (quotes == null)
                throw new ArgumentException("Problem loading quotes");

            var authors = AddAuthors(quotes);

            _context.Quotes.AddRange(quotes.Select(q => 
                new Quote
                {
                    Text = q.Text,
                    AuthorId = authors.First(a => a.Name == q.Author.Name).Id
                }
            ));
            _context.SaveChanges();
        }

        private IEnumerable<Author> AddAuthors(IEnumerable<Quote>? quotes)
        {
            var authors = quotes.Select(q => q.Author).Distinct().Select(a => new Author() { Name = a.Name });
            _context.Authors.AttachRange(authors);
            _context.SaveChanges();
            return _context.Authors;
        }

        public async Task<Quote> GetRandomQuoteAsync()
        {
            return await _context.Quotes
                .Include(q => q.Author)
                .OrderBy(q => Guid.NewGuid())
                .FirstAsync();
        }

        public Task<Quote> CreateQuoteAsync(Quote quote)
        {
            throw new NotImplementedException();
        }

        public Task DeleteQuoteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Quote>> GetQuotesAsync(int count)
        {
            throw new NotImplementedException();
        }

        public Task<Quote> UpdateQuoteAsync(Quote quote)
        {
            throw new NotImplementedException();
        }
    }
}
