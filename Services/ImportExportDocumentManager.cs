using System.Threading.Tasks;
using OrchardCore.Documents;
using OrchardCore.ImportExport.Models;

namespace OrchardCore.ImportExport.Services
{
    public class ImportExportDocumentManager
    {
        private readonly IDocumentManager<ImportExportsDocument> _documentManager;
        public ImportExportDocumentManager(IDocumentManager<ImportExportsDocument> documentManager)
        {
            _documentManager = documentManager;
        }

        public async Task<string> GetIdentifierAsync() => (await GetDocumentAsync()).Identifier;
        /// <summary>
        /// Loads the queries document from the store for updating and that should not be cached.
        /// </summary>
        public Task<ImportExportsDocument> LoadDocumentAsync() => _documentManager.GetOrCreateMutableAsync();

        /// <summary>
        /// Gets the queries document from the cache for sharing and that should not be updated.
        /// </summary>
        public Task<ImportExportsDocument> GetDocumentAsync() => _documentManager.GetOrCreateImmutableAsync();

        public async Task<string> GetAsync(string key)
        {
            var document = await LoadDocumentAsync();
            if (document.Data.ContainsKey(key))
            {
                return document.Data[key];
            }
            else
            {
                return null;
            }
        }

        public async Task CreateAsync(string key, string value)
        {
            var document = await LoadDocumentAsync();
            document.Data.Add(key, value);
            await _documentManager.UpdateAsync(document);
        }

        public async Task RemoveAsync(string key)
        {
            var document = await LoadDocumentAsync();
            document.Data.Remove(key);
            await _documentManager.UpdateAsync(document);
        }
        public async Task UpdateAsync(string key, string value)
        {
            var document = await LoadDocumentAsync();
            document.Data[key] = value;
            await _documentManager.UpdateAsync(document);
        }

        public async Task CreateOrUpdateAsync(string key,string value)
        {
            var document = await LoadDocumentAsync();
            if (document.Data.ContainsKey(key))
            {
                document.Data[key] = value;
            }
            else
            {
                document.Data.Add(key, value);
            }
            await _documentManager.UpdateAsync(document);
        }

        public async Task<bool> Exists(string key)
        {
            var data = await this.GetAsync(key);
            return data != null;
        }
    }
}
