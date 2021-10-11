using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OrchardCore.Contents.ViewModels;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.ImportExport.Security;
using OrchardCore.ImportExport.Services;
using System;
using System.Collections.Generic;

namespace OrchardCore.ImportExport.Drivers
{
	public class ContentOptionsDisplayDriver : DisplayDriver<ContentOptionsViewModel>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ImportExportDocumentManager _documentManager;
        private readonly IAuthorizationService _authorizationService;
        public ContentOptionsDisplayDriver(
            IHttpContextAccessor httpContextAccessor,
            ImportExportDocumentManager documentManager,
            IAuthorizationService authorizationService
        ){
            _httpContextAccessor = httpContextAccessor;
            _documentManager = documentManager;
            _authorizationService = authorizationService;
        }

        // Maintain the Options prefix for compatability with binding.
        protected override void BuildPrefix(ContentOptionsViewModel model, string htmlFieldPrefix)
        {
            Prefix = "Options";
        }

        public override IDisplayResult Edit(ContentOptionsViewModel model)
        {
            var results = new List<IDisplayResult>();
            var user = _httpContextAccessor.HttpContext.User;

            var curRouteValues = _httpContextAccessor.HttpContext.Request.RouteValues;
            var area = Convert.ToString(curRouteValues["area"]);
            var controller = Convert.ToString(curRouteValues["controller"]);
            var action = Convert.ToString(curRouteValues["action"]);

			if (!string.IsNullOrWhiteSpace(model.SelectedContentType))
			{
                
                results.Add(Initialize<ContentOptionsViewModel>("ContentsAdminList__Import", m => {
                    var routeValues = new RouteValueDictionary {
                        {"Area", "OrchardCore.ImportExport"},
				        {"Controller", "Admin"},
				        {"Action", "Import"},
				        {"contentTypeId", model.SelectedContentType},
                    };
                    m.RouteValues = routeValues;
                    m.SelectedContentType = model.SelectedContentType;
                }).RenderWhen(async () => {
                    var exists = await _documentManager.Exists($"{model.SelectedContentType}_Import");
                    var importPermission = ImportExportPermissionsHelper.CreateDynamicPermission(ImportExportPermissionsHelper.PermissionTemplates["Import"], model.SelectedContentType);
                    var hasImportPermission = await _authorizationService.AuthorizeAsync(user, importPermission);
                    return exists && hasImportPermission;
                }).Location("Actions:10.1"));
                results.Add(Initialize<ContentOptionsViewModel>("ContentsAdminList__Export", m=> {
                    var routeValues = new RouteValueDictionary {
                        {"Area", "OrchardCore.ImportExport"},
				        {"Controller", "Admin"},
				        {"Action", "Export"},
				        {"contentTypeId", model.SelectedContentType},
                    };

                    //添加对ListPart.ListContentItemId的约束
                    if ((String.Equals("OrchardCore.ContentNavigation", area, StringComparison.OrdinalIgnoreCase)
                    && String.Equals("Admin", controller, StringComparison.OrdinalIgnoreCase)
                    && String.Equals("Display", action, StringComparison.OrdinalIgnoreCase)
                    ))
                    {
                        var contentItemId = Convert.ToString(curRouteValues["contentItemId"]);
                        routeValues.Add("ListPart.ListContentItemId", contentItemId);
                    }

                    foreach(var query in _httpContextAccessor.HttpContext.Request.Query)
                    {
                        routeValues.Add(query.Key, query.Value);
                    }
                    m.RouteValues = routeValues;
                    m.SelectedContentType = model.SelectedContentType;
                }).RenderWhen(async () => {
                    var exists = await _documentManager.Exists($"{model.SelectedContentType}_Export");
                    var exportPermission = ImportExportPermissionsHelper.CreateDynamicPermission(ImportExportPermissionsHelper.PermissionTemplates["Export"], model.SelectedContentType);
                    var hasExportPermission = await _authorizationService.AuthorizeAsync(user, exportPermission);
                    return exists && hasExportPermission;
                }).Location("Actions:10.2"));
            }
            return Combine(results);
        }
    }
}
