using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ecommerce_API_Project.Models;
using Ecommerce_API_Project.Services;


var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
// Add services

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // How long session data lives
    options.Cookie.HttpOnly = true; // Prevent JavaScript access
    options.Cookie.IsEssential = true; // Make sure it's stored even without conssent
});


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<CheckoutService>();

//builder.Services.AddSingleton<CartService>();  // Register CartService for DI

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>
   (op => op.UseSqlServer(builder.Configuration.GetConnectionString("Con")
));

//Microsoft NewtonJson Package Install
builder.Services.AddControllers().AddNewtonsoftJson(options =>
      options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


//*ADD CORS*** (PROVIDE ACCESS TO EXTERNAL THIRD PARTIES--->
//WHEN YOU WANT TO PULL DATA FROM EXTERNAL APIS THAT ARE PUBLIC OR AUTHORIZED )

// CORS configuration
builder.Services.AddCors(opt => {
    opt.AddPolicy("AllowOrigin", builder =>
    builder.
     AllowAnyOrigin().
     AllowAnyHeader().
     AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();

app.UseCors("AllowOrigin");

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


