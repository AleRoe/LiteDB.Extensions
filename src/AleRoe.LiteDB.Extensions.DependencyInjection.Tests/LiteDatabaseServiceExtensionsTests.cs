using System;
using System.Collections.Generic;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace AleRoe.LiteDB.Extensions.DependencyInjection.Tests
{
    [TestFixture()]
    public class LiteDatabaseServiceExtensionsTests
    {
        private const string ConnectionString = ":memory:";
        
        [Test()]
        public void AddLiteDatabaseTest_Default()
        {
            using (var provider = new ServiceCollection()
                .AddLiteDatabase()
                .AddSingleton<IConfiguration>(Configuration)
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsTrue(database.Mapper.EmptyStringToNull);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_Default_MissingConnectionStringThrows()
        {
            using (var provider = new ServiceCollection()
                .AddLiteDatabase()
                .AddSingleton<IConfiguration>(EmptyConfiguration)
                .BuildServiceProvider())
            {
                Assert.Throws<ArgumentNullException>(() => provider.GetRequiredService<LiteDatabase>());
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithOptions()
        {
            var options = new LiteDatabaseServiceOptions(ConnectionString);
            using (var provider = new ServiceCollection()
                .AddLiteDatabase(options)
                .BuildServiceProvider())
            {
                Assert.DoesNotThrow(() => provider.GetRequiredService<LiteDatabase>());
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithOptionsAndPatch()
        {
            var options = new LiteDatabaseServiceOptions(ConnectionString);
            options.AddDatabasePatch(action => { action.UserVersion = 1; });
            using (var provider = new ServiceCollection()
                .AddLiteDatabase(options)
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.That(database.UserVersion, Is.EqualTo(1));
            }
        }


        [Test()]
        public void AddLiteDatabaseTest_WithOptions_SupportsLogging()
        {
            var options = new LiteDatabaseServiceOptions(ConnectionString);
            using (var provider = new ServiceCollection()
                .AddLiteDatabase(options)
                .AddLogging()
                .BuildServiceProvider())
            {
                Assert.DoesNotThrow(() => provider.GetRequiredService<LiteDatabase>());
                var iOptions = provider.GetRequiredService<IOptions<LiteDatabaseServiceOptions>>();
                Assert.IsNotNull(iOptions.Value.Logger);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithConnectionString()
        {
            using (var provider = new ServiceCollection()
                .AddLiteDatabase(ConnectionString)
                .BuildServiceProvider())
            {
                Assert.DoesNotThrow(() => provider.GetRequiredService<LiteDatabase>());
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithConnectionString_SupportsLogging()
        {
            using (var provider = new ServiceCollection()
                .AddLiteDatabase(ConnectionString)
                .AddLogging()
                .BuildServiceProvider())
            {
                Assert.DoesNotThrow(() => provider.GetRequiredService<LiteDatabase>());
                var options = provider.GetRequiredService<IOptions<LiteDatabaseServiceOptions>>();
                Assert.IsNotNull(options.Value.Logger);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithConfigure()
        {

            using (var provider = new ServiceCollection()
                .AddLiteDatabase(configure =>
                {
                    configure.ConnectionString.Filename = ConnectionString;
                    configure.Mapper.EmptyStringToNull = false;
                })
                .BuildServiceProvider())
            {
                Assert.DoesNotThrow(() => provider.GetRequiredService<LiteDatabase>());
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithPatch()
        {

            using (var provider = new ServiceCollection()
                .AddLiteDatabase(configure =>
                {
                    configure.ConnectionString.Filename = ConnectionString;
                    configure.AddDatabasePatch(action => { action.UserVersion= 1; });
                })
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.That(database.UserVersion, Is.EqualTo(1));
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithPatchExceptionThrows()
        {

            using (var provider = new ServiceCollection()
                .AddLiteDatabase(configure =>
                {
                    configure.ConnectionString.Filename = ConnectionString;
                    configure.AddDatabasePatch(action => { throw new Exception(); });
                })
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.Throws<LiteException>(() => database = provider.GetRequiredService<LiteDatabase>());
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithConfigure_SupportsLogging()
        {

            using (var provider = new ServiceCollection()
                .AddLiteDatabase(configure =>
                {
                    configure.ConnectionString.Filename = ConnectionString;
                    configure.Mapper.EmptyStringToNull = false;
                })
                .AddLogging()
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsFalse(database.Mapper.EmptyStringToNull);
                var options = provider.GetRequiredService<IOptions<LiteDatabaseServiceOptions>>();
                Assert.IsNotNull(options.Value.Logger);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithPostConfigure()
        {
            using (var provider = new ServiceCollection()
                .AddLiteDatabase()
                .AddSingleton<IConfiguration>(Configuration)
                .ConfigureOptions<ConfigureLiteDatabaseServiceOptions>()
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsFalse(database.Mapper.EmptyStringToNull);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithPostConfigure_SupportsLogging()
        {
            using (var provider = new ServiceCollection()
                .AddLiteDatabase()
                .AddLogging()
                .AddSingleton<IConfiguration>(Configuration)
                .ConfigureOptions<ConfigureLiteDatabaseServiceOptions>()
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsFalse(database.Mapper.EmptyStringToNull);
                var options = provider.GetRequiredService<IOptions<LiteDatabaseServiceOptions>>();
                Assert.IsNotNull(options.Value.Logger);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_Default_SupportsLogging()
        {

            using (var provider = new ServiceCollection()
                .AddLiteDatabase()
                .AddSingleton(Configuration)
                .AddLogging()
                .BuildServiceProvider())
            {
                Assert.DoesNotThrow(() => provider.GetRequiredService<LiteDatabase>());
                var options = provider.GetRequiredService<IOptions<LiteDatabaseServiceOptions>>();
                Assert.IsNotNull(options.Value.Logger);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithOptions_EmptyConfiguration()
        {
            var options = new LiteDatabaseServiceOptions(ConnectionString);
            using (var provider = new ServiceCollection()
                .AddSingleton<IConfiguration>(EmptyConfiguration)
                .AddLiteDatabase(options)
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsFalse(database.Mapper.EmptyStringToNull);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithConnectionString_EmptyConfiguration()
        {
            using (var provider = new ServiceCollection()
                .AddSingleton<IConfiguration>(EmptyConfiguration)
                .AddLiteDatabase(ConnectionString)
                .BuildServiceProvider())
            {
                Assert.DoesNotThrow(() => provider.GetRequiredService<LiteDatabase>());
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithConfigure_EmptyConfiguration()
        {
            using (var provider = new ServiceCollection()
                .AddSingleton<IConfiguration>(EmptyConfiguration)
                .AddLiteDatabase(configure =>
                {
                    configure.ConnectionString.Filename = ConnectionString;
                    configure.Mapper.EmptyStringToNull = false;
                })
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsFalse(database.Mapper.EmptyStringToNull);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithPostConfigure_ConnectionString()
        {
            using (var provider = new ServiceCollection()
                .AddLiteDatabase()
                .AddSingleton<IConfiguration>(Configuration)
                .ConfigureOptions<ConfigureLiteDatabaseServiceOptionsConnString>()
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsFalse(database.Mapper.EmptyStringToNull);
            }
        }
        [Test()]
        public void AddLiteDatabaseTest_WithPostConfigure_EmptyConfiguration()
        {
            using (var provider = new ServiceCollection()
                .AddLiteDatabase()
                .AddSingleton<IConfiguration>(EmptyConfiguration)
                .ConfigureOptions<ConfigureLiteDatabaseServiceOptionsConnString>()
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsFalse(database.Mapper.EmptyStringToNull);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_Default_WithMultiplePostConfigure_SupportsLogging()
        {
            using (var provider = new ServiceCollection()
                .AddLiteDatabase()
                .AddSingleton<IConfiguration>(Configuration)
                .AddLogging()
                .ConfigureOptions<ConfigureLiteDatabaseServiceOptions>()
                .ConfigureOptions<ConfigureLiteDatabaseServiceOptions2>()
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsFalse(database.Mapper.EmptyStringToNull);
                Assert.IsTrue(database.Mapper.SerializeNullValues);
                var options = provider.GetRequiredService<IOptions<LiteDatabaseServiceOptions>>();
                Assert.IsNotNull(options.Value.Logger);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithConfigureOption_EmptyConfiguration()
        {
            using (var provider = new ServiceCollection()
                .AddSingleton<IConfiguration>(EmptyConfiguration)
                .AddLiteDatabase<ConfigureLiteDatabaseServiceOptionsConnString>()
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsFalse(database.Mapper.EmptyStringToNull);
            }
        }

        [Test()]
        public void AddLiteDatabaseTest_WithConfigureOptionMultiple_EmptyConfiguration()
        {
            using (var provider = new ServiceCollection()
                .AddSingleton<IConfiguration>(EmptyConfiguration)
                .AddLiteDatabase<ConfigureLiteDatabaseServiceOptionsConnString>()
                .ConfigureOptions<ConfigureLiteDatabaseServiceOptions2>()
                .BuildServiceProvider())
            {
                LiteDatabase database = null;
                Assert.DoesNotThrow(() => database = provider.GetRequiredService<LiteDatabase>());
                Assert.IsFalse(database.Mapper.EmptyStringToNull);
                Assert.IsTrue(database.Mapper.SerializeNullValues);
            }
        }


        internal class ConfigureLiteDatabaseServiceOptionsConnString : IConfigureOptions<LiteDatabaseServiceOptions>
        {
            public void Configure(LiteDatabaseServiceOptions options)
            {
                options.ConnectionString = new ConnectionString(":memory:");
                options.Mapper.EmptyStringToNull = false;
            }
        }

        internal class ConfigureLiteDatabaseServiceOptions : IConfigureOptions<LiteDatabaseServiceOptions>
        {
            public void Configure(LiteDatabaseServiceOptions options)
            {
                options.Mapper.EmptyStringToNull = false;
            }
        }

        internal class ConfigureLiteDatabaseServiceOptions2 : IConfigureOptions<LiteDatabaseServiceOptions>
        {
            public void Configure(LiteDatabaseServiceOptions options)
            {
                options.Mapper.SerializeNullValues = true;
            }
        }

        private static IConfiguration Configuration
        {
            get
            {
                var configuration = new Dictionary<string, string>
                {
                    {$"ConnectionStrings:{LiteDatabaseServiceExtensions.LiteDatabaseConnectionStringKey}", ConnectionString}
                };
                var builder = new ConfigurationBuilder().AddInMemoryCollection(configuration);
                return builder.Build();
            }
        }

        private static IConfiguration EmptyConfiguration
        {
            get
            {
                var builder = new ConfigurationBuilder().AddInMemoryCollection();
                return builder.Build();
            }
        }
    }
}