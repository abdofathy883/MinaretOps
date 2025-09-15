using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Infrastructure.Data;
using Infrastructure.MappingProfiles;
using Infrastructure.Services.Announcements;
using Infrastructure.Services.Attendance;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Blog;
using Infrastructure.Services.Complaints;
using Infrastructure.Services.ContactForm;
using Infrastructure.Services.Discord;
using Infrastructure.Services.Email;
using Infrastructure.Services.InternalTasks;
using Infrastructure.Services.KPI;
using Infrastructure.Services.MediaUploads;
using Infrastructure.Services.NewFolder;
using Infrastructure.Services.Notifications;
using Infrastructure.Services.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ClientService = Infrastructure.Services.Clients.ClientService;

namespace Client_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<MinaretOpsDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<MinaretOpsDbContext>()
                .AddDefaultTokenProviders();

            JWTSettings jwtOptions = builder.Configuration.GetSection("JWT").Get<JWTSettings>()
                ?? throw new Exception("Error in JWT Settings");

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSetting"));
            builder.Services.Configure<DiscordSettings>(builder.Configuration.GetSection("Discord"));


            builder.Services.AddSingleton<JWTSettings>(jwtOptions);


            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJWTServices, JWTService>();
            builder.Services.AddScoped<IClientServices, ClientService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<IInternalTaskService, InternalTaskService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IContactFormService, ContactFormService>();
            builder.Services.AddScoped<IBlogService, BlogService>();
            //builder.Services.AddScoped<IPortfolioService, PortfolioService>();
            builder.Services.AddScoped<MediaUploadService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddScoped<INotificationService, NotificatonService>();
            builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
            builder.Services.AddScoped<IComplaintService, ComplaintService>();
            builder.Services.AddScoped<IKPIService, KPIService>();
            builder.Services.AddScoped<DiscordService>();

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
                cfg.AddProfile<AnnouncementProfile>();
                cfg.AddProfile<NotificationProfile>();
                cfg.AddProfile<ComplaintProfile>();
                cfg.AddProfile<KPIIncedintProfile>();
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            //builder.Services.AddSwaggerGen( c =>
            //{
            //    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            //    {
            //        Title = "My API",
            //        Version = "v1"
            //    });
            //});

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MinaretOps API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendOnly", policy =>
                {
                    policy.WithOrigins("https://internal.theminaretagency.com", "http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MinaretOps API v1"));
            app.UseDeveloperExceptionPage();

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
            app.Urls.Add("http://localhost:8080");
            app.Urls.Add("https://localhost:5001");


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("FrontendOnly");


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                //var services = scope.ServiceProvider;
                //await DbSeeder.SeedAsync(services);

                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<MinaretOpsDbContext>();
                await dbContext.Database.MigrateAsync();
                await DbSeeder.SeedAsync(services);
            }

            app.Run();
        }
    }
}
