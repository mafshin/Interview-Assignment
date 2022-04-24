using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Insurance.Api.Models
{
    public class ShoppingCart
    {
        public IList<OrderItem> OrderItems { get; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public Product Product { get; set; }
        public float Quantity { get; set; }
    }
}