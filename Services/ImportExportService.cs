using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using OrchardCore.Contents.Services;
using OrchardCore.Contents.ViewModels;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.ImportExport.ViewModels;
using OrchardCore.Liquid;

namespace OrchardCore.ImportExport.Services
{
    public class ImportExportService
    {
        private readonly ImportExportDocumentManager _documentManager;
        private readonly IContentsAdminListQueryService _contentsAdminListQueryService;
        private readonly IUpdateModelAccessor _updateModelAccessor;
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly IContentManager _contentManager;
        private readonly IContentItemIdGenerator _idGenerator;

        public ImportExportService(
            ImportExportDocumentManager documentManager,
            IContentsAdminListQueryService contentsAdminListQueryService,
            IUpdateModelAccessor updateModelAccessor,
            ILiquidTemplateManager liquidTemplateManager,
            IContentManager contentManager,
            IContentItemIdGenerator idGenerator,
            INotifier notifier,
            IHtmlLocalizer<ImportExportService> localizer,
            ILogger<ImportExportService> logger)
        {
            _documentManager = documentManager;
            _contentsAdminListQueryService = contentsAdminListQueryService;
            _updateModelAccessor = updateModelAccessor;
            _liquidTemplateManager = liquidTemplateManager;
            _contentManager = contentManager;
            _idGenerator = idGenerator;
            _notifier = notifier;
            T = localizer;
            Logger = logger;
        }

        private readonly INotifier _notifier;
        public IHtmlLocalizer T { get; }
        public ILogger Logger { get; set; }

        public async Task<string> ExportAsync(ContentOptionsViewModel options)
        {
            var key = $"{options.SelectedContentType}_Export";
			var template = await _documentManager.GetAsync(key);
			if(template == null)
            {

            }

            var query = await _contentsAdminListQueryService.QueryAsync(options, _updateModelAccessor.ModelUpdater);
            var contentItems = await query.ListAsync();

            var model = new ExportViewModel
            {
                ContentItems = contentItems
            };

            var content = await _liquidTemplateManager.RenderStringAsync(template, NullEncoder.Default,model,
                    new Dictionary<string, FluidValue>() { ["Model"] = new ObjectValue(model.ContentItems) });
            return content;
        }

        public async Task ImportAsync(Stream stream, string contentTypeId,ClaimsPrincipal User)
        {
            var key = $"{contentTypeId}_Import";
			var template = await _documentManager.GetAsync(key);
            if(template == null)
            {

            }

            var lines = new List<string[]>();
			var reader = new StreamReader(stream);
			string s;
			while((s= reader.ReadLine())!= null)
            {
                lines.Add(s.Split(','));
            }
			reader.Close();

            var list = new List<IDictionary<string, string>>();
            if (lines.Count > 1)
            {
                for(var i = 1; i < lines.Count; i++)
                {
                    var item = new Dictionary<string, string>();
                    for(var j=0;j< lines[i].Length; j++)
                    {
                        item.Add(lines[0][j], lines[i][j]);
                    }
                    list.Add(item);
                }
            }

            var model = new ImportViewModel
            {
                Lines = list
            };

            var content = await _liquidTemplateManager.RenderStringAsync(template, NullEncoder.Default,model,
                    new Dictionary<string, FluidValue>() { ["Model"] = new ObjectValue(model.Lines) });

            var jsondata = JsonConvert.DeserializeObject<List<ContentItem>>(content);
            jsondata = jsondata.Select(a =>
            {
                a.ContentItemId = a.ContentItemId ?? _idGenerator.GenerateUniqueId(a);
                a.ContentType = a.ContentType ?? contentTypeId;
                a.Owner = a.Owner;
                a.Author = a.Author;
                a.CreatedUtc = System.DateTime.UtcNow;
                a.Published = true;
                a.Latest = true;
                return a;
            }).ToList();
            await _contentManager.ImportAsync(jsondata);
        }
    }
}
