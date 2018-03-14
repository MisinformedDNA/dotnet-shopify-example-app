using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopifyApp.Models;
using ShopifySharp;
using ShopifySharp.Filters;
using System.Threading.Tasks;

namespace ShopifyApp.Controllers
{
    [Authorize("SessionPolicy")]
    public class HomeController : Controller
    {
        [HttpGet("", Name = "home")]
        public async Task<IActionResult> Index()
        {
            var shop = await GetShop();
            if (shop == null)
                return RedirectToRoute("login");

            ViewBag.Shop = shop;
            ViewBag.Products = await new ProductService(shop.Domain, shop.Token).ListAsync(new ProductFilter { Limit = 10 });
            return View();
        }

        private async Task<Models.Shop> GetShop()
        {
            var session = HttpContext.Session;
            await session.LoadAsync();
            var id = session.GetInt32(Constants.ShopId);
            if (id == null)
                return null;

            using (var context = new ShopifyContext())
            {
                var shop = await context.Shops.FindAsync(id);
                return shop;
            }
        }
    }
}