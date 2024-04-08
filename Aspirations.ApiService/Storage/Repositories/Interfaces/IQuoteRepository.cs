﻿using Aspirations.ApiService.Endpoints.Models;
using Aspirations.ApiService.Storage.Models;

namespace Aspirations.ApiService.Storage.Repositories.Interfaces
{
    public interface IQuoteRepository
    {
        Task<Quote> GetRandomQuoteAsync();
        Task<IEnumerable<Quote>> GetQuotesAsync(int count);
        Task<Quote> CreateQuoteAsync(Quote quote);
        Task<Quote> UpdateQuoteAsync(Quote quote);
        Task DeleteQuoteAsync(string id);
        Task WriteToFileAsync();
        Task<Author> AddAuthorAsync(Author author);
        Task<CountsResponse> GetCountsAsync();
    }
}
