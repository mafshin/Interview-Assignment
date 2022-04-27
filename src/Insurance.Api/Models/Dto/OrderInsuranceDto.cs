using System.Collections.Generic;

namespace Insurance.Api.Models.Dto
{
    public class OrderInsuranceDto
    {
        public IEnumerable<OrderItemDto> OrderItems { get; set; }
    }
}