using Aspirations.ApiService.Storage.Models;
using Aspirations.ApiService.Storage.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Aspirations.ApiService.Endpoints
{
    public static class QuotesEndpoints
    {
        public static void MapQuotesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/quotes");

            group.MapGet("random", GetRandomQuoteAsync);
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
    }
}
