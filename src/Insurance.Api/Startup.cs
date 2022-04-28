using System;
using System.Net.Http;
using System.Net.Mime;
using Insurance.Api.Business;
using Insurance.Api.Cache;
using Insurance.Api.Clients;
using Insurance.Api.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Insurance.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Insurance.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.Configure<AppConfiguration>(Configuration);

            var appConfiguration =  services.BuildServiceProvider().GetService<IOptions<AppConfiguration>>();

            var httpClientBuilder = services.AddHttpClient("ProductApiClient", (isp, client) =>
            {
                appConfiguration = isp.GetService<IOptions<AppConfiguration>>();

                client.BaseAddress = new Uri(appConfiguration.Value.ProductApi);
            });
            
            AddFaultTolerancePolicy(httpClientBuilder, appConfiguration);

            services.AddSingleton<IBusinessRules, BusinessRules>();
            services.AddSingleton<IProductApiClient, ProductApiClient>();
            services.AddSingleton<IInsuranceDataAccess, InsuranceDataAccess>();

            if (appConfiguration.Value.ResponseCachingEnabled)
            {
                CacheOptions catchOptions = new CacheOptions()
                {
                    DefaultExpirationInMilliseconds = appConfiguration.Value.ResponseCacheExpirationInMilliseconds
                };
                services.AddSingleton(catchOptions);
                services.AddMemoryCache();
                services.AddSingleton<ICache, MemoryCache>();
            }

            services.AddSwaggerGen();
        }

        private static void AddFaultTolerancePolicy(IHttpClientBuilder httpClientBuilder, IOptions<AppConfiguration>? appConfiguration)
        {
            if (appConfiguration.Value.FaultTolerance.RetryPolicyEnabled)
            {
                var retryPolicy = GetRetryPolicy(appConfiguration);
                httpClientBuilder.AddPolicyHandler(retryPolicy);
            }

            if (appConfiguration.Value.FaultTolerance.CircuitBreakerEnabled)
            {
                var circuitBreaker = GetCircuitBreakerPolicy(appConfiguration);
                httpClientBuilder.AddPolicyHandler(circuitBreaker);
            }
        }

        private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(
            IOptions<AppConfiguration>? appConfiguration)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(appConfiguration.Value.FaultTolerance.HandledEventsAllowedBeforeBreaking, 
                    TimeSpan.FromSeconds(appConfiguration.Value.FaultTolerance.DurationOfBreakInSeconds));
        }

        private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(
            IOptions<AppConfiguration>? appConfiguration)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1),
                retryCount: appConfiguration.Value.FaultTolerance.RetryCount);

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(delay);
            return retryPolicy;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(exceptionHandlerApp =>
                {
                    exceptionHandlerApp.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                        context.Response.ContentType = MediaTypeNames.Text.Plain;

                        await context.Response.WriteAsync("An error has occured");
                    });
                });
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}