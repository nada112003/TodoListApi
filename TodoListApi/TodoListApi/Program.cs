using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;
using System.Globalization;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace TodoListApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure culture settings
            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            // Add services to the container.
            builder.Services.AddControllers();

            // Configure DbContext with SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Todo List API",
                    Version = "v1",
                    Description = "A simple task management API",
                    Contact = new OpenApiContact
                    {
                        Name = "Todo List API Support",
                        Email = "support@todolist.com"
                    }
                });

                // Set the comments path for the Swagger JSON and UI
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo List API V1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            // Redirect to Swagger UI
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

            app.Run();
        }
    }
}
