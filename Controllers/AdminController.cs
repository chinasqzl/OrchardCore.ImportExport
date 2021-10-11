using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.Contents.Services;
using OrchardCore.Contents.ViewModels;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.ImportExport.Services;
using OrchardCore.ImportExport.ViewModels;
using YesSql.Filters.Query;

namespace OrchardCore.ImportExport.Controllers
{
    [Admin]
    public class AdminController : Controller {
		private readonly IAuthorizationService _authorizationService;
		private readonly ImportExportService _importExportService;
		private readonly ImportExportDocumentManager _documentManager;
		private readonly INotifier _notifier;

		public AdminController(
		  IAuthorizationService authorizationService,
		  ImportExportService importExportService,
		  ImportExportDocumentManager documentManager,
		  INotifier notifier,
		  IHtmlLocalizer<AdminController> localizer,
		  ILogger<AdminController> logger
		) {
			_authorizationService = authorizationService;
			_importExportService = importExportService;
			_documentManager = documentManager;
			_notifier = notifier;
			
			T = localizer;
			Logger = logger;
		}

		public IHtmlLocalizer T { get; }
		public ILogger Logger { get; set; }


		public async Task<IActionResult> Index(string id) {
			var template = await _documentManager.GetAsync(id);
			return View(new ExportTemplateViewModel { Template = template });
		}

		[HttpPost,ActionName("Index")]
		public async Task<IActionResult> IndexPost(string id,string template) {
			var oldTemplate = await _documentManager.GetAsync(id);
            await _documentManager.CreateOrUpdateAsync(id, template);
			return View(new ExportTemplateViewModel { Template = template });
		}


        [HttpPost]
        public async Task<IActionResult> Import(Microsoft.AspNetCore.Http.IFormFile importedFile, string contentTypeId, string returnUrl)
        {
            await _importExportService.ImportAsync(importedFile.OpenReadStream(), contentTypeId,User);
            return Redirect(returnUrl);
        }

        //public async Task<IActionResult> Export(string contentType,bool blank=false) {
        //	string archiveFileName = await _importExportService.ExportAsync(contentType, blank);
        //	return new PhysicalFileResult(archiveFileName, "application/ms-excel") { FileDownloadName = archiveFileName+".xlsx" };
        //}

        [HttpGet]
        public async Task<IActionResult> Export(
			[ModelBinder(BinderType = typeof(ContentItemFilterEngineModelBinder), Name = "q")] QueryFilterResult<ContentItem> queryFilterResult, 
            ContentOptionsViewModel options,
			string contentTypeId = "")
        {
			if (String.IsNullOrWhiteSpace(contentTypeId))
            {
				return Forbid();
            }

			options.SelectedContentType = contentTypeId;
			options.FilterResult = queryFilterResult;
			options.FilterResult.MapFrom(options);
			var result = await _importExportService.ExportAsync(options);
			var archiveFileName = $"{contentTypeId}_{DateTime.Now.ToString("yyyyMMdd")}.csv";
			//var test = new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("text/csv")
			//{
			//	Charset = "utf-8",
			//	Encoding = System.Text.Encoding.UTF8
			//};
			return new FileContentResult(System.Text.Encoding.Default.GetBytes(result), "text/csv") {
				FileDownloadName = archiveFileName
			};
        }
    }
}
