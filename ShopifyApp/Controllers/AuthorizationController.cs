using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShopifyApp.Helpers;
using ShopifyApp.Models;
using ShopifySharp;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShopifyApp.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthorizationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("login/{shop?}", Name = "login")]
        public async Task<IActionResult> New(string shop)
        {
            if (shop.IsPresent())
            {
                var shopDomain = GetShopDomain(shop);
                return await Authenticate(shopDomain);
            }

            ViewBag.AppName = _configuration.GetValue("AppName", "###AppName MISSING###");
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Create(string shop)
        {
            var shopDomain = GetShopDomain(shop);
            return await Authenticate(shopDomain);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code = null, string shop = null)
        {
            var token = await AuthorizationService.Authorize(code, shop, _configuration["ShopifyApiKey"], _configuration["ShopifyApiSecret"]);
            if (token.IsPresent())
            {
                await LoginShop(shop, token);
                return await GetRedirect();
            }

            TempData["error"] = Constants.InvalidShopUrl;
            return RedirectToRoute("login");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToRoute("login");
        }

        private async Task<IActionResult> Authenticate(string shopDomain)
        {
            if (!shopDomain.IsPresent())
            {
                TempData["error"] = Constants.InvalidShopUrl;
                return await GetRedirect();
            }

            string appUrl = Request.Host.Host != "localhost"
                ? Request.Host.Value
                : _configuration.GetValue<string>("AppUrl");
            string callbackUrl = new Uri(new Uri(appUrl), "callback").ToString();
            string shopifyUrl = $"https://{shopDomain}";
            string shopifyApiKey = _configuration.GetValue<string>("ShopifyApiKey");

            var authorizationUrl = AuthorizationService.BuildAuthorizationUrl(
                Constants.Scopes,
                shopifyUrl,
                shopifyApiKey,
                callbackUrl);
            return Redirect(authorizationUrl.ToString());
        }

        private async Task<ActionResult> GetRedirect()
        {
            var session = HttpContext.Session;
            await session.LoadAsync();
            var returnTo = session.GetString(Constants.ReturnTo);
            if (returnTo.IsPresent())
            {
                session.Remove(Constants.ReturnTo);
                return LocalRedirect(returnTo);
            }

            return RedirectToRoute("home");
        }

        public static string GetShopDomain(string shopDomain)
        {
            var name = shopDomain.Trim();
            name = Regex.Replace(name, "^https?://", "");
            if (!Regex.IsMatch(name, $@"\.{Constants.MyShopifyDomain}"))
                name = $"{name}.{Constants.MyShopifyDomain}";

            try
            {
                var uri = new Uri($"http://{name}");
                return Regex.IsMatch(uri.Host, $@"^[a-z0-9][a-z0-9-]*[a-z0-9]\.{Constants.MyShopifyDomain}$")
                    ? uri.Host
                    : null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private async Task LoginShop(string shopDomain, string token)
        {
            Models.Shop shop = null;
            using (var context = new ShopifyContext())
            {
                shop = await context.Shops.SingleOrDefaultAsync(s => s.Domain == shopDomain);
                if (shop == null)
                {
                    shop = new Models.Shop(shopDomain, token);
                    context.Shops.Add(shop);
                }
                else
                {
                    shop.Token = token;
                }

                await context.SaveChangesAsync();
            }

            var session = HttpContext.Session;
            await session.LoadAsync();
            session.SetInt32(Constants.ShopId, shop.Id);
            await session.CommitAsync();
        }
    }
}