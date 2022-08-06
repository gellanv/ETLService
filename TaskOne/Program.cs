using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile($"appsettings.json").Build();



