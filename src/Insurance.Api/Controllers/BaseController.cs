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
        /// Initializes a new instance of <see cref="BaseController{T}"/>.
        /// </summary>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <param name="logger">Logger instance.</param>
        protected BaseController(IOptions<AppConfiguration> appConfiguration, ILogger<T> logger)
        {
            AppConfiguration = appConfiguration;
            Logger = logger;
        }

        /// <summary>
        /// Application Configuration.
        /// </summary>
        /// <value></value>
        protected IOptions<AppConfiguration> AppConfiguration { get; }

        /// <summary>
        /// Logger instance.
        /// </summary>
        protected ILogger<T> Logger { get; }
    }
}