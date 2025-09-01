using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Infrastructure.Data;
using Infrastructure.MappingProfiles;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Email;
using Infrastructure.Services.InternalTasks;
using Infrastructure.Services.NewFolder;
using Infrastructure.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using ClientService = Infrastructure.Services.Clients.ClientService;
using Infrastructure.Services.ContactForm;
using Infrastructure.Services.MediaUploads;
using Infrastructure.Services.Attendance;
using Infrastructure.Services.Notifications;

namespace Client_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));
            builder.Services.AddDbContext<MinaretOpsDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<MinaretOpsDbContext>()
                .AddDefaultTokenProviders();

            JWTSettings jwtOptions = builder.Configuration.GetSection("JWT").Get<JWTSettings>()
                ?? throw new Exception("Error in JWT Settings");

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSetting"));


            builder.Services.AddSingleton<JWTSettings>(jwtOptions);


            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJWTServices, JWTService>();
            builder.Services.AddScoped<IClientServices, ClientService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<IInternalTaskService, InternalTaskService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IContactFormService, ContactFormService>();
            //builder.Services.AddScoped<IBlogService, BlogService>();
            //builder.Services.AddScoped<IPortfolioService, PortfolioService>();
            builder.Services.AddScoped<MediaUploadService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddScoped<INotificationService, NotificatonService>();

            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ClientProfile>();
                cfg.AddProfile<ServiceProfile>();
                cfg.AddProfile<ClientServiceProfile>();
                cfg.AddProfile<TaskGroupProfile>();
                cfg.AddProfile<TaskItemProfile>();
                cfg.AddProfile<InternalTaskProfile>();
                cfg.AddProfile<InternalTaskAssignmentProfile>();
                cfg.AddProfile<ContactFormProfile>();
                //cfg.AddProfile<ProjectProfile>();
                cfg.AddProfile<PostProfile>();
                cfg.AddProfile<AttendanceRecordProfile>();
                cfg.AddProfile<LeaveRequestProfile>();
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendOnly", policy =>
                {
                    policy.WithOrigins("https://internal.theminaretagency.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,
                KnownProxies = { System.Net.IPAddress.Parse("127.0.0.1") }
                
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("FrontendOnly");


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await DbSeeder.SeedAsync(services);
            }

            app.Run("http://127.0.0.1:8080");
        }
    }
}
