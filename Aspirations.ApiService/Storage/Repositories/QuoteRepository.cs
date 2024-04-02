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

            _context.Quotes.AttachRange(quotes.Select(x =>
                 new Quote
                 {
                     Text = x.Text,
                     Author = new Author
                     {
                         Name = x.Author.Name
                     }
                 }
            ));
            _context.SaveChanges();
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
