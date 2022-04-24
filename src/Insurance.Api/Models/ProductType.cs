namespace Insurance.Api.Models
{
    /// <summary>
    /// Represents type of a product.
    /// </summary>
    public class ProductType
    {
        /// <summary>
        /// Unique id of the product type.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the product type e.g laptop, smartphone.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether products of this type can be insured or not.
        /// </summary>
        public bool CanBeInsured { get; set; }
    }
}