using System;
using Fluid;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Contents.ViewModels;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.ImportExport.Drivers;
using OrchardCore.ImportExport.Security;
using OrchardCore.ImportExport.Services;
using OrchardCore.ImportExport.Settings;
using OrchardCore.ImportExport.ViewModels;
using OrchardCore.Modules;
using OrchardCore.Security.Permissions;

namespace OrchardCore.ImportExport
{
    public class Startup : StartupBase {
		public override void ConfigureServices(IServiceCollection services) {
			services.AddScoped<ImportExportDocumentManager>();
			services.AddScoped<ImportExportService>();
			services.AddScoped<IDisplayDriver<ContentOptionsViewModel>, ContentOptionsDisplayDriver>();
			services.AddScoped<IContentTypeDefinitionDisplayDriver, ImportExportContentTypeDefinitionDriver>();

			services.AddScoped<IPermissionProvider, ImportExportPermissions>();

			services.Configure<TemplateOptions>(o =>
			{
				o.MemberAccessStrategy.Register<ImportViewModel>();
				o.MemberAccessStrategy.Register<ExportViewModel>();
			});

            services.Configure<MvcOptions>((options) =>
            {
                options.Filters.Add(typeof(ContentImportFilter));
            });
        }

		public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
		{
			routes.MapAreaControllerRoute(
				name: "AdminImportContents",
				areaName: "OrchardCore.ImportExport",
				pattern: "Admin/Contents/{contentTypeId}/Import",
				defaults: new { controller = "Admin", action = "Import" }
			);

		}
	}
}
