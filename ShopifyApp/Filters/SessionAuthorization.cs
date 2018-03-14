using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopifyApp.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Filters
{
    public class SessionAuthorization : AuthorizationHandler<SessionAuthorization>, IAuthorizationRequirement
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SessionAuthorization requirement)
        {
            if (context.Resource is AuthorizationFilterContext mvcContext)
            {
                var session = mvcContext.HttpContext.Session;
                await session.LoadAsync();
                var exists = session.Keys.Contains(Constants.ShopId);
                if (!exists)
                {
                    mvcContext.Result = new LoginRedirectResult();
                }

                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
