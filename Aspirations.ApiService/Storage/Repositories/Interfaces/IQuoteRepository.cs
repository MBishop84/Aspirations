using Aspirations.ApiService.Endpoints.Models;
using Aspirations.ApiService.Storage.Models;

namespace Aspirations.ApiService.Storage.Repositories.Interfaces
{
    public interface IQuoteRepository
    {
        Task<Quote> GetRandomQuoteAsync();
        Task<Quote> CreateQuoteAsync(Quote quote);
        Task WriteToFileAsync();
        Task<Author> AddAuthorAsync(Author author);
        Task<CountsResponse> GetCountsAsync();
    }
}
