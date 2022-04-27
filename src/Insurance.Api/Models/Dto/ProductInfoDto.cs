namespace Insurance.Api.Models.Dto
{
    public class ProductInfoDto
    {
        public int ProductId { get; set; }
        public string ProductTypeName { get; set; }
        public bool ProductTypeHasInsurance { get; set; }
        public float SalesPrice { get; set; }
        public int ProductTypeId { get; set; }
    }
}