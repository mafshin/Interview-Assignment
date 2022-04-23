using Insurance.Api.Controllers;

namespace Insurance.Api
{
    public class BusinessRules 
    {
        public static float CalculateInsuranceValue(HomeController.InsuranceDto toInsure)
        {
            float insurance = 0f;

            if (toInsure.SalesPrice < 500)
                insurance = 0;
            else
            {
                if (toInsure.SalesPrice > 500 && toInsure.SalesPrice < 2000)
                    if (toInsure.ProductTypeHasInsurance)
                        insurance += 1000;
                if (toInsure.SalesPrice >= 2000)
                    if (toInsure.ProductTypeHasInsurance)
                        insurance += 2000;
            }

            if ((toInsure.ProductTypeName == "Laptops" || toInsure.ProductTypeName == "Smartphones") && toInsure.ProductTypeHasInsurance)
                insurance += 500;

            return insurance;
        }
    }
}