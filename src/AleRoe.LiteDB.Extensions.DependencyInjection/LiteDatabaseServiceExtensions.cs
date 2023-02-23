using System;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AleRoe.LiteDB.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up LiteDb services in an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    public static class LiteDatabaseServiceExtensions
    {
        public const string LiteDatabaseConnectionStringKey = "LiteDatabase";
        public const string LiteDatabaseLoggerCategory = "LiteDB.LiteDatabase";

        /// <summary>
        ///  Adds a singleton <see cref="LiteDatabase" /> service implementation to the services collection
        ///  using the <c>LiteDatabase</c> connection string supplied in settings.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddLiteDatabase(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            return services
                .AddLiteDbCore();
        }

        /// <summary>
        /// Adds a singleton <see cref="LiteDatabase" /> service implementation to the services collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
        /// <param name="configure">The action used to configure this instance.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddLiteDatabase(this IServiceCollection services, Action<LiteDatabaseServiceOptions> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            
            return services
                .AddLiteDbCore()
                .Configure(configure);
        }

        /// <summary>
        /// Adds a singleton <see cref="LiteDatabase" /> service implementation to the services collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
        /// <param name="options">The <see cref="LiteDatabaseServiceOptions"/> instance used to configure this instance.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddLiteDatabase(this IServiceCollection services, LiteDatabaseServiceOptions options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (options == null) throw new ArgumentNullException(nameof(options));
            
            return services.AddLiteDatabase(configure =>
            {
                configure.ConnectionString = options.ConnectionString;
                configure.Mapper = options.Mapper;
                if (options.Logger != null)
                {
                    configure.Logger = options.Logger;
                }
                
            });
        }

        /// <summary>
        /// Adds a singleton <see cref="LiteDatabase" /> service implementation to the services collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
        /// <param name="connectionString">The connection string used to configure this instance.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddLiteDatabase(this IServiceCollection services, string connectionString)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            return services.AddLiteDatabase(configure =>
            {
                configure.ConnectionString = new ConnectionString(connectionString);
            });
        }

        /// <summary>
        /// Adds a singleton <see cref="LiteDatabase" /> service implementation to the services collection.
        /// </summary>
        /// <typeparam name="T">The <see cref="IConfigureOptions{LiteDatabaseServiceOptions}"/> type that will configure options.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that calls can be chained.</returns>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public static IServiceCollection AddLiteDatabase<T>(this IServiceCollection services) where T : IConfigureOptions<LiteDatabaseServiceOptions>
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            return services
                .AddLiteDbCore()
                .ConfigureOptions(typeof(T));
        }

        /// <summary>
        /// Adds the core services for LiteDatabase.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <exception cref="ArgumentNullException">services</exception>
        private static IServiceCollection AddLiteDbCore(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddOptions();
            services.AddOptions<LiteDatabaseServiceOptions>()
                .Configure<IServiceProvider>((options, provider) =>
                {
                    var factory = provider.GetService<ILoggerFactory>();
                    options.Logger = factory?.CreateLogger(LiteDatabaseLoggerCategory);

                    var configuration = provider.GetService<IConfiguration>();
                    if (configuration != null)
                    {
                        var connectionString = configuration.GetConnectionString(LiteDatabaseConnectionStringKey);
                        if (connectionString != null)
                        {
                            options.ConnectionString = new ConnectionString(connectionString);
                        }
                        
                    }
                });
            services.TryAddTransient<ILiteDatabaseFactory, LiteDatabaseFactory>();
            services.TryAddSingleton<LiteDatabase>(provider =>
            {
                var factory = provider.GetRequiredService<ILiteDatabaseFactory>();
                return factory.Create();
            });

            return services;
        }
    }
}