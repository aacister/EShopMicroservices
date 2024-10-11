using Microsoft.AspNetCore.Http.Features;

namespace Basket.API.Models
{
    public class ShoppingCart
    {
        public string UserName { get; set; } = default!;
        public List<ShoppingCartitem> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);

        public ShoppingCart(string username)
        {
            UserName = username;
        }

        //Required for Mapping
        public ShoppingCart()
        {

        }
    }
}
