namespace ShopifyApp.Models
{
    public class Shop
    {
        public Shop() { }

        public Shop(string domain, string token)
        {
            Domain = domain;
            Token = token;
        }

        public int Id { get; set; }
        public string Domain { get; set; }
        public string Token { get; set; }
    }
}
