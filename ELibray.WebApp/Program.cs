using AutoMapper;
using E_Library.Models;
using E_Library.Repository;
using ELibrary.WebApp.Mapping;
using ELibrary.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.CookiePolicy;
using ELibrary.WebApp.Models;
using ELibrary.WebApp.EmailServices;
using ELibrary.WebApp.Middlewares;
using ELibrary.WebApp.Excel;
using ELibrary.WebApp.Hubs;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.IIS;

namespace ELibrary.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure file upload limits (reduced for stability)
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
                options.ValueLengthLimit = 10 * 1024 * 1024;
                options.ValueCountLimit = 1024;
                options.KeyLengthLimit = 1024;
                options.BufferBody = true;
                options.BufferBodyLengthLimit = 10 * 1024 * 1024;
            });

            // Configure Kestrel server limits
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2);
            });

            // Configure IIS limits
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddDbContext<ElibraryContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
            );
            builder.Services.Configure<EmailSettings>(
              builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddSignalR();

            // Repository 
            builder.Services.AddScoped<IReaderRepository, ReaderRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
            builder.Services.AddScoped<ICheckoutRepository, CheckoutRepository>();
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            builder.Services.AddScoped<IReaderRepository,ReaderRepository>();
            builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();



            // service
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IReaderService, ReaderService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<IPublisherService, PublisherService>();
            builder.Services.AddScoped<ICheckoutService, CheckoutService>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddTransient<INotificationService, NotificationService>();
            builder.Services.AddScoped<IBookExcelService, BookExcelService>();
            builder.Services.AddScoped<IReaderService,ReaderService>();
         //   builder.Services.AddScoped<IStatisticsService, StatisticsService>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<IAuditLogService, AuditLogService>();


            // Distributed cache + Session
            builder.Services.AddDistributedMemoryCache();

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
          

            var app = builder.Build();

            // Error + HSTS
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
              app.MapHub<NotificationHub>("/notificationHub");
          
            app.UseStaticFiles();

            app.UseSession();
            app.UseRouting();

         
            app.UseCookiePolicy();

            
          

           app.UseMiddleware<CheckLoginMiddleware>();
           app.UseMiddleware<AuditLogMiddleware>();

            
            app.UseMiddleware<CheckPermissionMiddleware>();
            
            // app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
