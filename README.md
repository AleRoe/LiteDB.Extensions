# LiteDB extensions for Microsoft.Extensions.DependencyInjection

Provides extension methods for registering and customizing a LiteDB.LiteDabase singleton instance using Microsoft.Extensions.DependencyInjection.

### Installing via NuGet

```csharp
Install-Package AleRoe.LiteDB.Extensions.DependencyInjection
```

### Basic usage

```csharp
//Register
services.AddLiteDatabase();

//Retrieve
var database = provider.GetRequiredService<LiteDatabase>()
```
This will register a singleton LiteDatabase instance using the connection string provided in your appsettings.json file under the `ConnectionStrings:LiteDatabase` key. 
The default registration requires that this key is present, otherwise a AgrumentNullException will be thrown. 
The LiteDatabase instance is configured using the default `BsonMapper.Global` settings and will use an `ILogger` instance for logging if configured.

### Custom configuration

The `LiteDB.ConnectionString`, `LiteDB.BsonMapper` and the logger to be used can be customized by configuring the `LiteDatabaseServiceOptions` object:

```csharp
services.AddLiteDatabase(configure =>
    {
        configure.ConnectionString.Filename = "MyDatabaseFile.db";
        configure.Mapper.EmptyStringToNull = false;
    });
```
Alternatively, you can use `IConfigureOptions<LiteDatabaseServiceOptions>` to configure your settings:

```csharp
internal class ConfigureLiteDatabaseServiceOptions : IConfigureOptions<LiteDatabaseServiceOptions>
{
    public void Configure(LiteDatabaseServiceOptions options)
    {
        options.Mapper.EmptyStringToNull = false;
    }
}

//...
services.AddLiteDatabase();
services.ConfigureOptions<ConfigureLiteDatabaseServiceOptions>()

// Alternatively in one call
services.AddLiteDatabase<ConfigureLiteDatabaseServiceOptions>();
```

Configuring options can be called multiple times.

#### A note on Logging
LiteDB currently does really not support logging. If you pass an `ILogger` instance in  `LiteDatabaseServiceOptions` or use the `IServiceCollection.AddLogging()` extension, then this will only be used to log an information message with the filename of the database.  
