using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.ImportExport.Services;
using OrchardCore.ImportExport.ViewModels;

namespace OrchardCore.ImportExport.Settings
{
    public class ImportExportContentTypeDefinitionDriver : ContentTypeDefinitionDisplayDriver
    {
        private readonly ImportExportDocumentManager _documentManager;
        private readonly IStringLocalizer<ImportExportContentTypeDefinitionDriver> S;

        public ImportExportContentTypeDefinitionDriver(
            ImportExportDocumentManager documentManager,
            IStringLocalizer<ImportExportContentTypeDefinitionDriver> localizer)
        {
            _documentManager = documentManager;
            S = localizer;
        }

        public override IDisplayResult Edit(ContentTypeDefinition contentTypeDefinition)
        {
            return Initialize<ImportExportSettingsViewModel>("ImportExportSettings", async model =>
            {
                model.ContentTypeId = contentTypeDefinition.Name;
                model.ExportExist = (await _documentManager.GetAsync($"{contentTypeDefinition.Name}_Export")) != null;
                model.ImportExist = (await _documentManager.GetAsync($"{contentTypeDefinition.Name}_Import")) != null;
            }).Location("Shortcuts");
        }
    }
}
