using System;
using System.Collections.Generic;
using System.Linq;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.Security.Permissions;

namespace OrchardCore.ImportExport.Security
{
    public class ImportExportPermissionsHelper
    {
        private static readonly Permission ImportContent = new Permission("Import_{0}", "Import {0}");
        private static readonly Permission ExportContent = new Permission("Export_{0}", "Export {0}");

        public static readonly Dictionary<string, Permission> PermissionTemplates = new Dictionary<string, Permission>
        {
            { "Import", ImportContent },
            { "Export", ExportContent }
        };

        public static Dictionary<ValueTuple<string, string>, Permission> PermissionsByType = new Dictionary<ValueTuple<string, string>, Permission>();


        /// <summary>
        /// Returns a dynamic permission for a content type, based on a global content permission template
        /// </summary>
        public static Permission ConvertToDynamicPermission(Permission permission)
        {
            if (PermissionTemplates.TryGetValue(permission.Name, out var result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Generates a permission dynamically for a content type, without a display name or category
        /// </summary>
        public static Permission CreateDynamicPermission(Permission template, string contentType)
        {
            var key = new ValueTuple<string, string>(template.Name, contentType);

            if (PermissionsByType.TryGetValue(key, out var permission))
            {
                return permission;
            }

            permission = new Permission(
                String.Format(template.Name, contentType),
                String.Format(template.Description, contentType),
                (template.ImpliedBy ?? new Permission[0]).Select(t => CreateDynamicPermission(t, contentType))
            );

            var localPermissions = new Dictionary<ValueTuple<string, string>, Permission>(PermissionsByType);
            localPermissions[key] = permission;
            PermissionsByType = localPermissions;

            return permission;
        }

        /// <summary>
        /// Generates a permission dynamically for a content type
        /// </summary>
        public static Permission CreateDynamicPermission(Permission template, ContentTypeDefinition typeDefinition)
        {
            return new Permission(
                String.Format(template.Name, typeDefinition.Name),
                String.Format(template.Description, typeDefinition.DisplayName),
                (template.ImpliedBy ?? Array.Empty<Permission>()).Select(t => CreateDynamicPermission(t, typeDefinition))
            )
            {
                Category = typeDefinition.DisplayName
            };
        }


    }
}
