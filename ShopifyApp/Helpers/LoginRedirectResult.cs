using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace ShopifyApp.Helpers
{
    public class LoginRedirectResult : ActionResult
    {
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;
            if (request.Method == "GET")
            {
                var session = httpContext.Session;
                await session.LoadAsync();
                session.SetString(Constants.ReturnTo, request.GetEncodedPathAndQuery());
                await session.CommitAsync();
            }

            var services = httpContext.RequestServices;
            var urlHelperFactory = services.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(context);

            var url = urlHelper.RouteUrl("login", new { shop = request.Query["shop"].ToString() });
            httpContext.Response.Redirect(url);
        }
    }
}
