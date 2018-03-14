using ShopifySharp.Enums;

namespace ShopifyApp
{
    public static class Constants
    {
        // Errors
        public const string LoggedOut = "Successfully logged out";
        public const string CouldNotLogIn = "Could not log in to Shopify store";
        public const string InvalidShopUrl = "Invalid shop domain";

        // Session variables
        public const string ReturnTo = "return_to";
        public const string ShopId = "shop_id";

        public const string MyShopifyDomain = "myshopify.com";

        public static AuthorizationScope[] Scopes =>
            new AuthorizationScope[] {
                AuthorizationScope.ReadProducts,
                AuthorizationScope.WriteProducts
            };
    }
}
