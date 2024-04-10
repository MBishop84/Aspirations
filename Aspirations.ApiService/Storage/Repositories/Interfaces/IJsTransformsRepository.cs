using Aspirations.ApiService.Storage.Models;

namespace Aspirations.ApiService.Storage.Repositories.Interfaces
{
    public interface IJsTransformsRepository
    {
        Task<IEnumerable<JsTransform>> GetJsTransforms();
        Task<JsTransform> AddJsTransform(JsTransform jsTransform);
        Task<JsTransform> UpdateJsTransform(JsTransform jsTransform);
        Task DeleteJsTransform(int id);
    }
}
