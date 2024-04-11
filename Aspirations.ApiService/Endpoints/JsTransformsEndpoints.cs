using Aspirations.ApiService.Storage.Models;
using Aspirations.ApiService.Storage.Repositories.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Aspirations.ApiService.Endpoints
{
    public static class JsTransformsEndpoints
    {
        public static void MapJsTransformsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/js_transforms");

            group.MapGet("all", GetAllJsTransformsAsync);
            group.MapPost("add", AddJsTransform);
            group.MapDelete("delete/{id}", DeleteJsTransform);
        }

        private static async Task<Results<Ok<IEnumerable<JsTransform>>, NotFound<string>>> GetAllJsTransformsAsync(IJsTransformsRepository jsTransformsRepository)
        {
            try
            {
                return TypedResults.Ok(await jsTransformsRepository.GetJsTransforms());
            }
            catch (Exception ex)
            {
                return TypedResults.NotFound(ex.Message);
            }
        }

        private static async Task<Results<Ok<JsTransform>, NotFound<string>>> AddJsTransform(
            IJsTransformsRepository jsTransformsRepository, JsTransform jsTransform)
        {
            try
            {
                return TypedResults.Ok(await jsTransformsRepository.AddJsTransform(
                    new JsTransform
                    {
                        AddedBy = string.IsNullOrEmpty(jsTransform.AddedBy) 
                        ? "Unknown" : jsTransform.AddedBy,
                        AddedOn = DateTime.Now,
                        Name = jsTransform.Name,
                        Code = jsTransform.Code
                    }));
            }
            catch (Exception ex)
            {
                return TypedResults.NotFound(ex.Message);
            }
        }

        private static async Task<Results<Ok, NotFound<string>>> DeleteJsTransform(IJsTransformsRepository jsTransformsRepository, int id)
        {
            try
            {
                await jsTransformsRepository.DeleteJsTransform(id);
                return TypedResults.Ok();
            }
            catch (Exception ex)
            {
                return TypedResults.NotFound(ex.Message);
            }
        }
    }
}
