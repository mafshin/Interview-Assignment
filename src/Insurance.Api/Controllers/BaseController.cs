using Insurance.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Insurance.Api.Controllers
{
    /// Base class for Api Controllers
    public abstract class BaseController<T> : Controller 
    {
        /// <summary>
        /// Initializes a new instance of <see ref="BaseController">.
        /// </summary>
        /// <param name="appConfiguration">The application configuration.</param>
        public  BaseController(IOptions<AppConfiguration> appConfiguration, ILogger<T> logger)
        {
            AppConfiguration = appConfiguration;
            Logger = logger;
        }

        /// <summary>
        /// Application Configuration.
        /// </summary>
        /// <value></value>
        public IOptions<AppConfiguration> AppConfiguration { get; }
        public ILogger<T> Logger { get; }
    }
}