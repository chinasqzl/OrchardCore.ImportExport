using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "ImportExport",
    Author = "Zhao.Liang",
    Version = "1.0.0",
    Description = "导入导出",
	Dependencies = new[] { "OrchardCore.Contents" },
	Category = "Content"
)]
