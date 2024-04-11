using Aspirations.ApiService.Storage.Models;
using Aspirations.ApiService.Storage.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Aspirations.ApiService.Storage.Repositories
{
    public class JsTransformsRepository : IJsTransformsRepository
    {
        private readonly ApiContext _context;

        public JsTransformsRepository(ApiContext context)
        {
            _context = context;

            if (_context.JsTransforms.Any()) return;

            _context.JsTransforms.AddRange(JsonConvert.DeserializeObject<IEnumerable<JsTransform>>(
                File.ReadAllText("Storage/Data/JsTransforms.json"))?.Select(x => new JsTransform
                {
                    AddedBy = x.AddedBy,
                    AddedOn = x.AddedOn,
                    Name = x.Name,
                    Code = x.Code
                }) ?? []);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<JsTransform>> GetJsTransforms()
        {
            return await _context.JsTransforms.ToListAsync();
        }

        public async Task<JsTransform> AddJsTransform(JsTransform jsTransform)
        {
            var existing = await _context.JsTransforms.Where(x => x.Name == jsTransform.Name || 
            x.Code == jsTransform.Code).FirstOrDefaultAsync();
            if (existing != null)
            {
                jsTransform = await UpdateJsTransform(jsTransform);
            }
            else
            {
                await _context.JsTransforms.AddAsync(jsTransform);
                await _context.SaveChangesAsync();
            }
            if (jsTransform.AddedBy == "ArchBishop84")
                File.WriteAllText("Storage/Data/JsTransforms.json", JsonConvert.SerializeObject(
                    await _context.JsTransforms.ToListAsync()));

            return jsTransform;
        }

        public async Task<JsTransform> UpdateJsTransform(JsTransform jsTransform)
        {
            _context.JsTransforms.Update(jsTransform);
            await _context.SaveChangesAsync();
            return jsTransform;
        }

        public async Task DeleteJsTransform(int id)
        {
            _context.JsTransforms.Remove(
                await _context.JsTransforms.Where(x => x.Id == id).FirstAsync());
            await _context.SaveChangesAsync();
            File.WriteAllText("Storage/Data/JsTransforms.json", JsonConvert.SerializeObject(
                await _context.JsTransforms.ToListAsync()));
        }
    }
}
