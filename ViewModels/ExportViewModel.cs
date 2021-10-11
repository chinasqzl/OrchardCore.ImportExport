using System.Collections.Generic;
using OrchardCore.ContentManagement;

namespace OrchardCore.ImportExport.ViewModels
{
    public class ExportViewModel
    {
        public IEnumerable<ContentItem> ContentItems { get; set; }
    }
}
