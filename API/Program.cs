using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApplicationServices(builder.Configuration);                //using our ApplicationServicesExtension class where we've added all the services

builder.Services.AddIdentityServices(builder.Configuration);                   //Similarly IdentityServiceExtension class's function is used here

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

//sequence matters here! it comes after CORS and before Mapcontrollers   --- authentication 1st, authorization 2nd
app.UseAuthentication();   //asks does the request has a valid token
app.UseAuthorization();    //if yes what are you allowed to do

app.MapControllers();              //Middlewares

app.Run();
