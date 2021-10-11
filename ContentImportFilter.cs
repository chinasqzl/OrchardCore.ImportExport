using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Layout;
using OrchardCore.DisplayManagement.Shapes;

namespace OrchardCore.ImportExport
{
    public class ContentImportFilter: IAsyncResultFilter
    {
        private readonly ILayoutAccessor _layoutAccessor;
        private readonly IShapeFactory _shapeFactory;

        public ContentImportFilter(
            ILayoutAccessor layoutAccessor,
            IShapeFactory shapeFactory)
        {
            _layoutAccessor = layoutAccessor;
            _shapeFactory = shapeFactory;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            // Should only run on the front-end (or optionally also on the admin) for a full view.
            if ((context.Result is ViewResult || context.Result is PageResult))
            {
                var area = Convert.ToString(context.RouteData.Values["area"]);
                var controller = Convert.ToString(context.RouteData.Values["controller"]);
                var action = Convert.ToString(context.RouteData.Values["action"]);

                var routeValues = new Dictionary<string,string> {
							{"Area", "OrchardCore.ImportExport"},
							{"Controller", "Admin"},
							{"Action", "Import"}
						};

                var contentType = "";
                if ((String.Equals("OrchardCore.Contents", area, StringComparison.OrdinalIgnoreCase)
                    && String.Equals("Admin", controller, StringComparison.OrdinalIgnoreCase)
                    && String.Equals("List", action, StringComparison.OrdinalIgnoreCase)
                    ))
                {
                    contentType = Convert.ToString(context.RouteData.Values["contentTypeId"]);
                }
                if ((String.Equals("OrchardCore.ContentNavigation", area, StringComparison.OrdinalIgnoreCase)
                    && String.Equals("Admin", controller, StringComparison.OrdinalIgnoreCase)
                    && String.Equals("Display", action, StringComparison.OrdinalIgnoreCase)
                    ))
                {
                    var contentItemId = Convert.ToString(context.RouteData.Values["contentItemId"]);
                    var groupId = Convert.ToString(context.RouteData.Values["groupId"]);
                    if (groupId.StartsWith("List-"))
                    {
                        contentType = groupId.Substring("List-".Length);
                        routeValues.Add("ListPart.ContainerId", contentItemId);
                    }
                }

                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    routeValues.Add("ContentTypeId", contentType);
                    var layout = await _layoutAccessor.GetLayoutAsync();
                    var tabsZone = layout.Zones["Footer"];

                    if (tabsZone is Shape shape)
                    {
                        await shape.AddAsync(await _shapeFactory.CreateAsync("ContentsAdminList__ImportForm",
                            Arguments.From(new { Route = routeValues })));
                    }
                }
            }

            await next.Invoke();
        }
    }
}
