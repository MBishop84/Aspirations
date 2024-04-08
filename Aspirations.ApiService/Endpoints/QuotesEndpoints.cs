using Aspirations.ApiService.Endpoints.Models;
using Aspirations.ApiService.Storage.Models;
using Aspirations.ApiService.Storage.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Aspirations.ApiService.Endpoints
{
    public static class QuotesEndpoints
    {
        private record NewAuthor(string Name);

        private record NewQuote(string Text, NewAuthor Author);

        public static void MapQuotesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/quotes");

            group.MapGet("random", GetRandomQuoteAsync);
            group.MapGet("counts", GetQuoteCountsAsync);
            group.MapPost("create", CreateQuoteAsync);
            group.MapPost("writeAll", WriteToFileAsync);
        }

        private static async Task<Results<Ok<Quote>,NotFound<string>>> GetRandomQuoteAsync
            (IQuoteRepository quoteRepository)
        {
            try
            {
                return TypedResults.Ok(await quoteRepository.GetRandomQuoteAsync());
            }
            catch (Exception ex)
            {
                return TypedResults.NotFound(ex.Message);
            }
        }

        private static async Task<Results<Ok<CountsResponse>, NotFound<string>>> GetQuoteCountsAsync(IQuoteRepository quoteRepository)
        {
            try
            {
                return TypedResults.Ok(await quoteRepository.GetCountsAsync());
            }
            catch (Exception ex)
            {
                return TypedResults.NotFound(ex.Message);
            }
        }

        private static async Task<Results<Ok<Quote>,NotFound<string>>> CreateQuoteAsync
            (IQuoteRepository quoteRepository, NewQuote newQuote)
        {
            try
            {
                var author = await quoteRepository.AddAuthorAsync(new Author { Name = newQuote.Author.Name });

                var quote = new Quote
                {
                    Text = newQuote.Text,
                    AuthorId = author.Id
                };

                return TypedResults.Ok(await quoteRepository.CreateQuoteAsync(quote));
            }
            catch (Exception ex)
            {
                return TypedResults.NotFound(ex.Message);
            }
        }

        private static async Task WriteToFileAsync(IQuoteRepository quoteRepository)
        {
            await quoteRepository.WriteToFileAsync();
        }
    }
}
