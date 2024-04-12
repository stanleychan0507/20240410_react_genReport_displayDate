using _20240314_1first;
using _20240314_1first.Controllers;
using _20240314_1first.Model;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    o.SerializerSettings.MaxDepth = 1;
    o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
//builder.Services.Configure<JsonOptions>(o =>
//{
//    o.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//    o.SerializerOptions.MaxDepth = 0;
//});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var b = builder.Services.Where(x => x.ServiceType.ToString().Contains("Json"));
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<BikestoresContext>();
builder.Services.AddScoped<Learning>();
builder.Services.AddScoped<GenerateResult>();


//builder.Services.AddControllersWithViews()+
//    .AddNewtonsoftJson(options =>
//    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
//);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
Console.Read();