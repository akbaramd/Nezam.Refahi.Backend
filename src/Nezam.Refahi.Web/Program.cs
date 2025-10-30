using Bonyan.Plugins;
using Nezam.Refahi.WebApi;

var builder = BonyanApplication.CreateModularBuilder<NezamWebModule>("web", c =>
{
  c.PlugInSources.AddFolder("./Plugins");
});

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
var app = await builder.BuildAsync();

await app.RunAsync();
