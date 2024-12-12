
using FriendFunny.Application.Contracts.Interface.Student;
using FriendFunny.Application.Service.Student;
using FriendFunny.Extensions;
using FriendFunny.SqlSugar.Extension;

namespace FriendFunny.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddAgileServices(builder.Configuration);
            builder.Services.AddPrintConsole();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSqlSugar();
            builder.Services.AddTransient<IStudentAppService, StudentAppService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();
            app.UseAgileServices();
            app.MapControllers();

            app.Run();
        }
    }
}
