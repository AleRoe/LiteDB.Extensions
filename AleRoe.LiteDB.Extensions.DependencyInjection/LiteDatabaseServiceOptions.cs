﻿using System;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace AleRoe.LiteDB.Extensions.DependencyInjection
{
    /// <summary>
    /// An options class for configuring LiteDb services
    /// </summary>
    public class LiteDatabaseServiceOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LiteDatabaseServiceOptions"/> class.
        /// </summary>
        public LiteDatabaseServiceOptions(){}

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteDatabaseServiceOptions"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public LiteDatabaseServiceOptions(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            
            this.ConnectionString = new ConnectionString(connectionString);
        }

        /// <summary>
        /// The <see cref="ConnectionString"/> connection string for the LiteDb database
        /// </summary>
        public ConnectionString ConnectionString { get; set; } = new ();

        /// <summary>
        /// The <see cref="BsonMapper"/> class used to convert entities to and from BsonDocument
        /// </summary>
        public BsonMapper Mapper { get; set; } = BsonMapper.Global;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }
    }
}