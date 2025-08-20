using DemoApi;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
var app = builder.Build();

LoadTestEndpoints.Map(app);
app.Run("http://0.0.0.0:5000");
