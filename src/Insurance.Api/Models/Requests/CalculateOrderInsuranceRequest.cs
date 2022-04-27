using System.Collections.Generic;

namespace Insurance.Api.Models.Requests
{
    /// <summary>
    /// Request model for calculating order insurance
    /// </summary>
    public class CalculateOrderInsuranceRequest
    {
        /// <summary>
        /// List of order items.
        /// </summary>
        public IEnumerable<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// An order item in insurance calculation for an order.
        /// </summary>
        public class OrderItem
        {
            public int ProductId { get; set; }
            public float Quantity { get; set; }
        }
    }
}