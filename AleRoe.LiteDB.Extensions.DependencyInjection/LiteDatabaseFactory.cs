using System;
using LiteDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace AleRoe.LiteDB.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides a factory object for creating a <c>LiteDatabase</c> instance.
    /// </summary>
    /// <seealso cref="ILiteDatabaseFactory" />
    internal class LiteDatabaseFactory : ILiteDatabaseFactory
    {
        private readonly LiteDatabaseServiceOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteDatabaseFactory"/> class.
        /// </summary>
        /// <param name="options">The <see cref="IOptions{LiteDatabaseServiceOptions}"/> instance used to initialize the <see cref="LiteDatabase"/>.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public LiteDatabaseFactory(IOptions<LiteDatabaseServiceOptions> options)
        {
            this.options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc/>
        public LiteDatabase Create()
        {
            options.Logger?.LogInformation($"Using database {options.ConnectionString.Filename}");
            return new LiteDatabase(options.ConnectionString, options.Mapper);
        }
    }
}