using System;
using System.Collections.Generic;
using OrchardCore.Data.Documents;

namespace OrchardCore.ImportExport.Models
{
    public class ImportExportsDocument : Document
	{
		/// <summary>
        /// key: <ContentType>_<Import|Export>
        /// </summary>
		public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	}
}
