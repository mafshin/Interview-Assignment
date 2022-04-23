using System;
using System.Collections.Generic;
using Insurance.Api.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Xunit;

namespace Insurance.Tests
{
    public class InsuranceTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public InsuranceTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        public static IEnumerable<object[]> GetTestData()
        {
            return new List<object[]>
            {
                new TestScenario(1, 1000, "CalculateInsurance_GivenSalesPriceBetween500And2000Euros_ShouldAddThousandEurosToInsuranceCost").ToObjectArray(),

                new TestScenario(2, 500, "CalculateInsurance_GivenSalesPriceLessThan500AndProductTypeBeingLaptop_ShouldInsuranceCostBeFiveHundred").ToObjectArray(),

                new TestScenario(3, 0, "CalculateInsurance_GivenProductTypeHasNoInsurance_ShouldInsuranceCostBeZero").ToObjectArray(),

                new TestScenario(4, 0, "CalculateInsurance_GivenSalesPriceLessThan500Euros_ShouldInsuranceCostBeZero").ToObjectArray(),

                new TestScenario(5, 2000, "CalculateInsurance_GivenSalesPriceGreaterThan2000Euros_ShouldInsuranceCostBe2000").ToObjectArray(),

                new TestScenario(6, 2500, "CalculateInsurance_GivenSalesPriceGreaterThan2000EurosAndProductTypeBeingLaptop_ShouldInsuranceCostBe2500").ToObjectArray(),
            };
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void CalculateInsuranceTests(TestScenario scenario)
        {
            float expectedInsuranceValue = scenario.ExpectedInsurance;

            var dto = new HomeController.InsuranceDto
            {
                ProductId = scenario.ProductId
            };
            var sut = new HomeController();

            var result = sut.CalculateInsurance(dto);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }

        public class TestScenario
        {
            public TestScenario(int productId, float expectedInsurance, string testName)
            {
                ProductId = productId;
                ExpectedInsurance = expectedInsurance;
                TestName = testName;
            }
            public int ProductId { get; set; }

            public float ExpectedInsurance { get; set; }
            public string TestName { get; }

            public override string ToString() => TestName;

            public object[] ToObjectArray() => new object[] { this };
        }
    }

    public class ControllerTestFixture : IDisposable
    {
        private readonly IHost _host;

        public ControllerTestFixture()
        {
            _host = new HostBuilder()
                   .ConfigureWebHostDefaults(
                        b => b.UseUrls("http://localhost:5002")
                              .UseStartup<ControllerTestStartup>()
                    )
                   .Build();

            _host.Start();
        }

        public void Dispose() => _host.Dispose();
    }

    public class ControllerTestStartup
    {
        public void Configure(IApplicationBuilder app)
        {
            var sampleProducts = new Dictionary<int, object>()
            {
                { 1, new
                    {
                        id = 1,
                        name = "Test Product",
                        productTypeId = 1,
                        salesPrice = 750
                    }
                },
                { 2, new
                    {
                        id = 2,
                        name = "Test Product 2",
                        productTypeId = 2, // laptop
                        salesPrice = 250
                    }
                },
                { 3, new
                    {
                        id = 3,
                        name = "Test Product 3",
                        productTypeId = 3, // smartphone
                        salesPrice = 600
                    }
                },
                { 4, new
                    {
                        id = 4,
                        name = "Test Product 4",
                        productTypeId = 1,
                        salesPrice = 200
                    }
                },
                { 5, new
                 {
                        id = 5,
                        name = "Test Product 5",
                        productTypeId = 1,
                        salesPrice = 3000
                 }
                },
                { 6, new
                 {
                        id = 6,
                        name = "Test Product 6",
                        productTypeId = 2, // laptop
                        salesPrice = 4000
                 }
                }
            };
            app.UseRouting();
            app.UseEndpoints(
                ep =>
                {
                    ep.MapGet(
                        "products/{id:int}",
                        context =>
                        {
                            int productId = int.Parse((string)context.Request.RouteValues["id"]);
                            var product = sampleProducts[productId];
                            return context.Response.WriteAsync(JsonConvert.SerializeObject(product));
                        }
                    );
                    ep.MapGet(
                        "product_types",
                        context =>
                        {
                            var productTypes = new[]
                                               {
                                                   new
                                                   {
                                                       id = 1,
                                                       name = "Test type",
                                                       canBeInsured = true
                                                   },
                                                   new {
                                                       id = 2,
                                                       name = "Laptops",
                                                       canBeInsured = true
                                                   },
                                                   new {
                                                       id = 3,
                                                       name = "Smartphones",
                                                       canBeInsured = false
                                                   }
                                               };
                            return context.Response.WriteAsync(JsonConvert.SerializeObject(productTypes));
                        }
                    );
                }
            );
        }

    }
}