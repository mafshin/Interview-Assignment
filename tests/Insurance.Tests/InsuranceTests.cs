using System;
using System.Collections.Generic;
using System.IO;
using Insurance.Api.Controllers;
using Insurance.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using System.Net.Http;
using Moq;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Insurance.Api;
using Insurance.Api.Business;
using Insurance.Api.Clients;
using Insurance.Api.Data;
using Insurance.Api.Models.Requests;
using Insurance.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Insurance.Tests
{
    public class InsuranceTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IOptions<AppConfiguration> configs;
        private readonly ILogger<HomeController> logger;

        public InsuranceTests(ControllerTestFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _testOutputHelper = testOutputHelper;

            configs = GetConfiguration();

            logger = new TestLogger<HomeController>();
        }

        public class ProductTestScenario : TestScenario<(int ProductId, float ExpectedInsurance)>
        {
            public ProductTestScenario(int ProductId, float ExpectedInsurance, string testName) :
                base((ProductId, ExpectedInsurance), testName)
            {
            }
        }

        public static IEnumerable<object[]> GetProductTestData()
        {
            return new List<object[]>
            {
                new ProductTestScenario(ProductId: 1, ExpectedInsurance: 1000,
                        "CalculateInsurance_GivenSalesPriceBetween500And2000Euros_ShouldAddThousandEurosToInsuranceCost")
                    .ToObjectArray(),

                new ProductTestScenario(ProductId: 2, ExpectedInsurance: 500,
                        "CalculateInsurance_GivenSalesPriceLessThan500AndProductTypeBeingLaptop_ShouldInsuranceCostBeFiveHundred")
                    .ToObjectArray(),

                new ProductTestScenario(ProductId: 3, ExpectedInsurance: 0,
                    "CalculateInsurance_GivenProductTypeHasNoInsurance_ShouldInsuranceCostBeZero").ToObjectArray(),

                new ProductTestScenario(ProductId: 4, ExpectedInsurance: 0,
                    "CalculateInsurance_GivenSalesPriceLessThan500Euros_ShouldInsuranceCostBeZero").ToObjectArray(),

                new ProductTestScenario(ProductId: 5, ExpectedInsurance: 2000,
                    "CalculateInsurance_GivenSalesPriceGreaterThan2000Euros_ShouldInsuranceCostBe2000").ToObjectArray(),

                new ProductTestScenario(ProductId: 6, ExpectedInsurance: 2500,
                        "CalculateInsurance_GivenSalesPriceGreaterThan2000EurosAndProductTypeBeingLaptop_ShouldInsuranceCostBe2500")
                    .ToObjectArray(),
            };
        }

        public class OrderTestScenario : TestScenario<(float ExpectedInsurance,
            IEnumerable<(int ProductId, float Quantity)> OrderItems)>
        {
            public OrderTestScenario(float ExpectedInsurance, IEnumerable<(int, float)> OrderItems, string testName)
                : base((ExpectedInsurance, OrderItems), testName)
            {
            }
        }

        public static IEnumerable<object[]> GetOrderTestData()
        {
            return new List<object[]>
            {
                new OrderTestScenario(ExpectedInsurance: 3000, OrderItems: new (int ProductId, float Quantity)[]
                    {
                        (ProductId: 1, Quantity: 3),
                        (ProductId: 3, Quantity: 2)
                    }
                    , "CalculateInsurance_GivenOrder1_ShouldInsuranceBe3000").ToObjectArray(),

                new OrderTestScenario(ExpectedInsurance: 11000, OrderItems: new (int ProductId, float Quantity)[]
                    {
                        (ProductId: 2, Quantity: 10),
                        (ProductId: 5, Quantity: 3)
                    }
                    , "CalculateInsurance_GivenOrder2_ShouldInsuranceBe11000").ToObjectArray(),

                new OrderTestScenario(ExpectedInsurance: 5500, OrderItems: new (int ProductId, float Quantity)[]
                    {
                        (ProductId: 1, Quantity: 3),
                        (ProductId: 3, Quantity: 2),
                        (ProductId: 7, Quantity: 2),
                    }
                    , "CalculateInsurance_GivenOrder3WithInsurableDigitalCamera_ShouldInsuranceBe5500").ToObjectArray(),

                new OrderTestScenario(ExpectedInsurance: 3000, OrderItems: new (int ProductId, float Quantity)[]
                        {
                            (ProductId: 1, Quantity: 3),
                            (ProductId: 3, Quantity: 2),
                            (ProductId: 8, Quantity: 2),
                        }
                        , "CalculateInsurance_GivenOrder4WithNonInsurableDigitalCamera_ShouldInsuranceBe3000")
                    .ToObjectArray(),

                new OrderTestScenario(ExpectedInsurance: 5500, OrderItems: new (int ProductId, float Quantity)[]
                        {
                            (ProductId: 1, Quantity: 3),
                            (ProductId: 3, Quantity: 2),
                            (ProductId: 7, Quantity: 2),
                            (ProductId: 8, Quantity: 2),
                        }
                        , "CalculateInsurance_GivenOrder5WithInsurableAndNonInsurableDigitalCamera_ShouldInsuranceBe5500")
                    .ToObjectArray(),
            };
        }

        public static IEnumerable<object[]> GetOrderTestDataWithSurcharges()
        {
            return new List<object[]>
            {
                new OrderTestScenario(ExpectedInsurance: 3900, OrderItems: new (int ProductId, float Quantity)[]
                    {
                        (ProductId: 1, Quantity: 3),
                        (ProductId: 3, Quantity: 2)
                    }
                    , "CalculateInsurance_GivenOrder1WithSurcharge_ShouldInsuranceBe3900").ToObjectArray(),

                new OrderTestScenario(ExpectedInsurance: 13800, OrderItems: new (int ProductId, float Quantity)[]
                    {
                        (ProductId: 2, Quantity: 10),
                        (ProductId: 5, Quantity: 3)
                    }
                    , "CalculateInsurance_GivenOrder2WithSurcharge_ShouldInsuranceBe13900").ToObjectArray(),

                new OrderTestScenario(ExpectedInsurance: 7400, OrderItems: new (int ProductId, float Quantity)[]
                    {
                        (ProductId: 1, Quantity: 3),
                        (ProductId: 3, Quantity: 2),
                        (ProductId: 7, Quantity: 2),
                    }
                    , "CalculateInsurance_GivenOrder31WithSurcharge_ShouldInsuranceBe8400").ToObjectArray(),

                new OrderTestScenario(ExpectedInsurance: 3900, OrderItems: new (int ProductId, float Quantity)[]
                    {
                        (ProductId: 1, Quantity: 3),
                        (ProductId: 3, Quantity: 2),
                        (ProductId: 8, Quantity: 2),
                    }
                    , "CalculateInsurance_GivenOrder4WithSurcharge_ShouldInsuranceBe3900").ToObjectArray(),

                new OrderTestScenario(ExpectedInsurance: 7400, OrderItems: new (int ProductId, float Quantity)[]
                    {
                        (ProductId: 1, Quantity: 3),
                        (ProductId: 3, Quantity: 2),
                        (ProductId: 7, Quantity: 2),
                        (ProductId: 8, Quantity: 2),
                    }
                    , "CalculateInsurance_GivenOrder5WithSurcharge_ShouldInsuranceBe8400").ToObjectArray(),
            };
        }

        [Theory]
        [MemberData(nameof(GetProductTestData))]
        public async Task CalculateProductInsuranceTests(
            ProductTestScenario scenario)
        {
            float expectedInsuranceValue = scenario.Model.ExpectedInsurance;

            var dto = new CalculateProductInsuranceRequest()
            {
                ProductId = scenario.Model.ProductId
            };

            var clinet = new HttpClient();
            clinet.BaseAddress = new Uri(this._fixture.ApiUrl);

            Mock<IHttpClientFactory> factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(clinet);

            ProductApiClient productApiClient = new ProductApiClient(factory.Object);
            InsuranceDataAccess insuranceDataAccess = new InsuranceDataAccess();
            BusinessRules businessRules = new BusinessRules(productApiClient, insuranceDataAccess);

            var sut = new HomeController(configs, logger, insuranceDataAccess, businessRules, productApiClient);

            var response = await sut.CalculateProductInsurance(dto);

            var result = (response as OkObjectResult).Value as CalculateProductInsuranceResponse;

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }

        [Theory]
        [MemberData(nameof(GetOrderTestData))]
        public async Task CalculateOrderInsuranceTests(OrderTestScenario testScenario)
        {
            float expectedInsuranceValue = testScenario.Model.ExpectedInsurance;

            var response = await CalculateOrderInsurance(testScenario);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: response.InsuranceValue
            );
        }

        [Theory]
        [MemberData(nameof(GetOrderTestDataWithSurcharges))]
        public async Task CalculateOrderInsuranceWithSurchargeTests(OrderTestScenario testScenario)
        {
            float expectedInsuranceValue = testScenario.Model.ExpectedInsurance;

            var surcharges = new ProductTypeSurcharge[]
            {
                new ProductTypeSurcharge()
                {
                    ProductTypeId = 1,
                    Surcharge = 0.3f,
                },
                new ProductTypeSurcharge()
                {
                    ProductTypeId = 2,
                    Surcharge = 0.2f,
                },
                new ProductTypeSurcharge()
                {
                    ProductTypeId = 3,
                    Surcharge = 0.6f,
                },
                new ProductTypeSurcharge()
                {
                    ProductTypeId = 4,
                    Surcharge = 0.4f,
                }
            };

            var response = await CalculateOrderInsurance(testScenario, surcharges);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: response.InsuranceValue
            );
        }

        [Fact]
        public async Task UpdateProductTypeSurcharge_ConcurrentUsers()
        {
            InsuranceDataAccess insuranceDataAccess = new InsuranceDataAccess();

            var ratesCollection = new float[][]
            {
                new float[] {0.1f, 0.2f, 0.3f},
                new float[] {0.11f, 0.22f, 0.33f}
            };

            var updateTasks = Enumerable.Range(0, 100)
                .Select(x => Task.Run(() => UpdateSurcharge(insuranceDataAccess)));

            // Simulate concurrent updates
            await Task.WhenAll(updateTasks);

            async Task UpdateSurcharge(IInsuranceDataAccess dataAccess)
            {
                var clinet = new HttpClient();
                clinet.BaseAddress = new Uri(this._fixture.ApiUrl);

                Mock<IHttpClientFactory> factory = new Mock<IHttpClientFactory>();
                factory.Setup(x => x.CreateClient(It.IsAny<string>()))
                    .Returns(clinet);

                ProductApiClient productApiClient = new ProductApiClient(factory.Object);
                BusinessRules businessRules = new BusinessRules(productApiClient, dataAccess);

                var sut = new HomeController(configs, logger, dataAccess, businessRules, productApiClient);

                var random = new Random();
                var surcharges = new ProductTypeSurcharge[]
                {
                    new ProductTypeSurcharge()
                    {
                        ProductTypeId = 1,
                        Surcharge = ratesCollection[random.Next(0, 2)][0]
                    },
                    new ProductTypeSurcharge()
                    {
                        ProductTypeId = 2,
                        Surcharge = ratesCollection[random.Next(0, 2)][1]
                    },
                    new ProductTypeSurcharge()
                    {
                        ProductTypeId = 3,
                        Surcharge = ratesCollection[random.Next(0, 2)][2]
                    },
                };

                await sut.SetProductTypeSurcharges(surcharges);

                await Task.Delay(random.Next(5));

                foreach (var surcharge in surcharges)
                {
                    var rate = await insuranceDataAccess.GetSurchargeByProductTypeId(surcharge.ProductTypeId);

                    // Check if surcharge rate is one of supplied values
                    _testOutputHelper.WriteLine($"ProductTypeId: {surcharge.ProductTypeId}, Rate: {rate}");
                    Assert.True(rate == ratesCollection[0][surcharge.ProductTypeId - 1] ||
                                rate == ratesCollection[1][surcharge.ProductTypeId - 1]);
                }
            }
        }

        [Fact]
        public async Task CalculateInsurance_GivenSalesPriceOver1700With40PercentSurcharge_ShouldInsuranceBe1400()
        {
            float expectedInsuranceValue = 1400;

            var dto = new CalculateProductInsuranceRequest()
            {
                ProductId = 7
            };

            var clinet = new HttpClient();
            clinet.BaseAddress = new Uri(this._fixture.ApiUrl);

            Mock<IHttpClientFactory> factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(clinet);

            ProductApiClient productApiClient = new ProductApiClient(factory.Object);
            InsuranceDataAccess insuranceDataAccess = new InsuranceDataAccess();
            BusinessRules businessRules = new BusinessRules(productApiClient, insuranceDataAccess);

            var sut = new HomeController(configs, logger, insuranceDataAccess, businessRules, productApiClient);

            var surcharge = new[]
            {
                new ProductTypeSurcharge()
                {
                    ProductTypeId = 4,
                    Surcharge = 0.4f
                }
            };

            await sut.SetProductTypeSurcharges(surcharge);

            var response = await sut.CalculateProductInsurance(dto);

            var result = (response as OkObjectResult).Value as CalculateProductInsuranceResponse;

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }

        [Fact]
        public async Task UpdateProductTypeSurcharge_GivenProductTypeIdDoesntExist_ShouldReturn422StatusCode()
        {
            InsuranceDataAccess insuranceDataAccess = new InsuranceDataAccess();

            var clinet = new HttpClient();
            clinet.BaseAddress = new Uri(this._fixture.ApiUrl);

            Mock<IHttpClientFactory> factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(clinet);

            ProductApiClient productApiClient = new ProductApiClient(factory.Object);
            BusinessRules businessRules = new BusinessRules(productApiClient, insuranceDataAccess);

            var sut = new HomeController(configs, logger, insuranceDataAccess, businessRules, productApiClient);

            var surcharges = new[]
            {
                new ProductTypeSurcharge()
                {
                    ProductTypeId = 1,
                    Surcharge = 0.2f,
                },
                new ProductTypeSurcharge()
                {
                    ProductTypeId = 100000,
                    Surcharge = 0.2f
                },
            };

            var response = await sut.SetProductTypeSurcharges(surcharges);

            var result = response as IStatusCodeActionResult;

            var actual = result.StatusCode;
            var expected = StatusCodes.Status422UnprocessableEntity;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestStartup()
        {
            var json = @"{

  ""AllowedHosts"": ""*"",
  ""ProductApi"": ""http://localhost:5010"",
  
  ""FaultTolerance"":
  {
    ""RetryPolicyEnabled"": true,
    ""RetryCount"": 3,
    
    ""CircuitBreakerEnabled"": true,
    ""DurationOfBreakInSeconds"": 30,
    ""HandledEventsAllowedBeforeBreaking"": 5
  }
}";
            using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            IHost host = null;
            try
            {
                host = new HostBuilder()
                    .ConfigureAppConfiguration(config => { config.AddJsonStream(memoryStream); })
                    .ConfigureWebHostDefaults(
                        b => b.UseUrls("http://localhost:5010")
                            .UseStartup<Startup>()
                    )
                    .Build();

                host.Start();
            }
            finally
            {
                host?.Dispose();
            }
        }

        [Fact]
        public async Task CalculateProductInsurance_GivenProductIdDoesntExist_ShouldReturn422()
        {
            var dto = new CalculateProductInsuranceRequest()
            {
                ProductId = 10000
            };

            var clinet = new HttpClient();
            clinet.BaseAddress = new Uri(this._fixture.ApiUrl);

            Mock<IHttpClientFactory> factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(clinet);

            ProductApiClient productApiClient = new ProductApiClient(factory.Object);
            InsuranceDataAccess insuranceDataAccess = new InsuranceDataAccess();
            BusinessRules businessRules = new BusinessRules(productApiClient, insuranceDataAccess);

            var sut = new HomeController(configs, logger, insuranceDataAccess, businessRules, productApiClient);

            var response = await sut.CalculateProductInsurance(dto);

            var actual = (response as IStatusCodeActionResult).StatusCode;

            var expected = StatusCodes.Status422UnprocessableEntity;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task CalculateOrderInsurance_GivenProductIdDoesntExist_ShouldReturn422()
        {
            OrderTestScenario testScenario = new OrderTestScenario(0, new (int, float)[]
            {
                (10000, 1)
            }, "OrderInsuranceWithInvalidProductId");
            
            var response = await CalculateOrderInsuranceResponse(testScenario, null);

            var actual = (response as IStatusCodeActionResult).StatusCode;

            var expected = StatusCodes.Status422UnprocessableEntity;

            Assert.Equal(expected, actual);
        }

        private async Task<CalculateOrderInsuranceResponse> CalculateOrderInsurance(
            OrderTestScenario testScenario, ProductTypeSurcharge[] productTypeSurcharges = null)
        {
            var response = await CalculateOrderInsuranceResponse(testScenario, productTypeSurcharges);

            var result = (response as OkObjectResult).Value as CalculateOrderInsuranceResponse;

            return result;
        }

        private async Task<IActionResult> CalculateOrderInsuranceResponse(OrderTestScenario testScenario,
            ProductTypeSurcharge[] productTypeSurcharges)
        {
            var dto = new CalculateOrderInsuranceRequest()
            {
                OrderItems = testScenario.Model.OrderItems.Select(x => new CalculateOrderInsuranceRequest.OrderItem()
                {
                    ProductId = x.ProductId, Quantity = x.Quantity
                })
            };

            var clinet = new HttpClient();
            clinet.BaseAddress = new Uri(this._fixture.ApiUrl);

            Mock<IHttpClientFactory> factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(clinet);

            ProductApiClient productApiClient = new ProductApiClient(factory.Object);
            InsuranceDataAccess insuranceDataAccess = new InsuranceDataAccess();
            BusinessRules businessRules = new BusinessRules(productApiClient, insuranceDataAccess);

            var sut = new HomeController(configs, logger, insuranceDataAccess, businessRules, productApiClient);

            if (productTypeSurcharges != null)
            {
                await sut.SetProductTypeSurcharges(productTypeSurcharges);
            }

            var response = await sut.CalculateOrderInsurance(dto);
            return response;
        }

        public IOptions<AppConfiguration> GetConfiguration()
        {
            return Options.Create<AppConfiguration>(new AppConfiguration()
            {
                ProductApi = _fixture.ApiUrl
            });
        }
    }

    public class ControllerTestFixture : IDisposable
    {
        private readonly IHost _host;

        public string ApiUrl { get; } = "http://localhost:5003";

        public ControllerTestFixture()
        {
            _host = new HostBuilder()
                .ConfigureWebHostDefaults(
                    b => b.UseUrls(ApiUrl)
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
                {
                    1, new
                    {
                        id = 1,
                        name = "Test Product",
                        productTypeId = 1,
                        salesPrice = 750
                    }
                },
                {
                    2, new
                    {
                        id = 2,
                        name = "Test Product 2",
                        productTypeId = 2, // laptop
                        salesPrice = 250
                    }
                },
                {
                    3, new
                    {
                        id = 3,
                        name = "Test Product 3",
                        productTypeId = 3, // smartphone
                        salesPrice = 600
                    }
                },
                {
                    4, new
                    {
                        id = 4,
                        name = "Test Product 4",
                        productTypeId = 1,
                        salesPrice = 200
                    }
                },
                {
                    5, new
                    {
                        id = 5,
                        name = "Test Product 5",
                        productTypeId = 1,
                        salesPrice = 3000
                    }
                },
                {
                    6, new
                    {
                        id = 6,
                        name = "Test Product 6",
                        productTypeId = 2, // laptop
                        salesPrice = 4000
                    }
                },
                {
                    7, new
                    {
                        id = 7,
                        name = "Nikon Z6",
                        productTypeId = 4, // digital camera
                        salesPrice = 1700
                    }
                },
                {
                    8, new
                    {
                        id = 8,
                        name = "Canon R7 (Upcoming !)",
                        productTypeId = 5, // digital camera
                        salesPrice = 2200
                    }
                }
            };

            var productTypes = new[]
            {
                new
                {
                    id = 1,
                    name = "Test type",
                    canBeInsured = true
                },
                new
                {
                    id = 2,
                    name = "Laptops",
                    canBeInsured = true
                },
                new
                {
                    id = 3,
                    name = "Smartphones",
                    canBeInsured = false
                },
                new
                {
                    id = 4,
                    name = "Digital cameras",
                    canBeInsured = true
                },
                new
                {
                    id = 5,
                    name = "Digital cameras",
                    canBeInsured = false
                },
            };
            app.UseRouting();
            app.UseEndpoints(
                ep =>
                {
                    ep.MapGet(
                        "products/{id:int}",
                        context =>
                        {
                            int productId = int.Parse((string) context.Request.RouteValues["id"]);
                            var product = sampleProducts[productId];
                            return context.Response.WriteAsync(JsonConvert.SerializeObject(product));
                        }
                    );
                    ep.MapGet(
                        "product_types",
                        context => { return context.Response.WriteAsync(JsonConvert.SerializeObject(productTypes)); }
                    );
                    ep.MapGet(
                        "product_types/{id:int}",
                        context =>
                        {
                            int productTypeId = int.Parse((string) context.Request.RouteValues["id"]);
                            var productType = productTypes.FirstOrDefault(x => x.id == productTypeId);
                            return context.Response.WriteAsync(JsonConvert.SerializeObject(productType));
                        }
                    );
                }
            );
        }
    }
}