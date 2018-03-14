using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ShopifyApp.Helpers
{
    public static class MiscExtentions
    {
        // Mimicking https://apidock.com/rails/Object/present%3F
        public static bool IsPresent(this string value)
        {
            return !IsBlank(value);
        }

        // Mimicking https://apidock.com/rails/Object/blank%3F
        public static bool IsBlank(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static IActionResult LoginRedirect(this ControllerBase controller)
        {
            var queryShop = controller.Request.Query["shop"].ToString();
            if (!queryShop.IsPresent())
                return controller.RedirectToRoute("login");

            controller.HttpContext.Session.SetString(Constants.ReturnTo, controller.Request.GetEncodedUrl());
            return controller.RedirectToRoute("login", new { shop = queryShop });
        }
    }
}
