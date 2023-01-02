using IdentityLog.Contexts;
using IdentityLog.CustomLogging;
using IdentityLog.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;

namespace IdentityLog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigurationManager configuration = builder.Configuration;


            // Add services to the container.

            //Sql Column
            SqlColumn sqlColumn = new SqlColumn();
            sqlColumn.ColumnName = "UserName";
            sqlColumn.DataType = System.Data.SqlDbType.NVarChar;
            sqlColumn.PropertyName = "UserName";
            sqlColumn.DataLength = 50;
            sqlColumn.AllowNull = true;


            ColumnOptions columnOpt = new ColumnOptions();
            columnOpt.Store.Remove(StandardColumn.Properties);
            columnOpt.Store.Add(StandardColumn.LogEvent);
            columnOpt.AdditionalColumns = new Collection<SqlColumn> { sqlColumn };



            Logger log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt")
                .WriteTo.MSSqlServer(connectionString : builder.Configuration.GetConnectionString("SqlServer"),
                sinkOptions:new MSSqlServerSinkOptions
                {
                    AutoCreateSqlTable = true,
                    TableName = "logs"
                },
                appConfiguration: null,
                columnOptions: columnOpt
                )
                .WriteTo.Seq(builder.Configuration["Seq:ServerURL"])
                .Enrich.FromLogContext()
                .Enrich.With<CustomUserNameColumn>()
                .MinimumLevel.Information()
                .CreateLogger();

            builder.Host.UseSerilog(log);
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<IdentityLogDbContext>(opts =>
            {
                opts.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });
            builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<IdentityLogDbContext>();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Home/Login");
                options.LogoutPath = new PathString("/Home/LogOut");
                options.AccessDeniedPath = new PathString("/Home/AccessDenied");
                options.Cookie = new CookieBuilder
                {
                    Name = "IdentityLogCookie",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };

                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
            });


            var app = builder.Build();
              
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.Use(async (context, next) =>
            {
                var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
                LogContext.PushProperty("UserName", username);
                await next();
            });


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}