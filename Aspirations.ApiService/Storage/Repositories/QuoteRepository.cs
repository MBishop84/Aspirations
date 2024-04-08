using Aspirations.ApiService.Endpoints.Models;
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
            _context.Authors.AddRange(authors);
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

        public async Task<Quote> CreateQuoteAsync(Quote quote)
        {
            var newQuote = await _context.Quotes
                .Include(a => a.Author)
                .FirstOrDefaultAsync(q => q.Text == quote.Text);

            if (newQuote != null)
                return newQuote;

            newQuote = quote;

            await _context.Quotes.AddAsync(newQuote);
            await _context.SaveChangesAsync();
            return newQuote;
        }

        public async Task<Author> AddAuthorAsync(Author newAuthor)
        {
            var existingAuthor = await _context.Authors
                .FirstOrDefaultAsync(a => a.Name == newAuthor.Name);

            if (existingAuthor != null)
                return existingAuthor;

            await _context.Authors.AddAsync(newAuthor);
            await _context.SaveChangesAsync();
            return newAuthor;
        }

        public async Task WriteToFileAsync()
        {
            await File.WriteAllTextAsync(
                "Storage/Data/quotes.json",
                JsonConvert.SerializeObject(
                    await _context.Quotes
                        .Include(a => a.Author)
                        .ToListAsync()
                    , Formatting.Indented));
        }

        public async Task<CountsResponse> GetCountsAsync()
        {
            return new CountsResponse
            {
                Authors = await _context.Authors.CountAsync(),
                Quotes = await _context.Quotes.CountAsync()
            };
        }
    }
}
