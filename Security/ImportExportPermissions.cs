using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Security.Permissions;

namespace OrchardCore.ImportExport.Security
{
    public class ImportExportPermissions : IPermissionProvider
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public ImportExportPermissions(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            // manage rights only for Securable types
            var securableTypes = _contentDefinitionManager.ListTypeDefinitions()
                .Where(ctd => ctd.GetSettings<ContentTypeSettings>().Securable);

            var result = new List<Permission>();

            foreach (var typeDefinition in securableTypes)
            {
                foreach (var permissionTemplate in ImportExportPermissionsHelper.PermissionTemplates.Values)
                {
                    result.Add(ImportExportPermissionsHelper.CreateDynamicPermission(permissionTemplate, typeDefinition));
                }
            }

            return Task.FromResult(result.AsEnumerable());
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return Enumerable.Empty<PermissionStereotype>();
        }
    }
}
