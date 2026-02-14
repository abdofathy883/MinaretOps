using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Infrastructure.Data;
using Infrastructure.MappingProfiles;
using Infrastructure.Services.Announcements;
using Infrastructure.Services.Attendance;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Branch;
using Infrastructure.Services.Checkpoints;
using Infrastructure.Services.Complaints;
using Infrastructure.Services.ContactForm;
using Infrastructure.Services.Contract;
using Infrastructure.Services.Currency;
using Infrastructure.Services.Discord;
using Infrastructure.Services.Email;
using Infrastructure.Services.InternalTasks;
using Infrastructure.Services.Invitations;
using Infrastructure.Services.JobDescription;
using Infrastructure.Services.KPI;
using Infrastructure.Services.LeaveRequestService;
using Infrastructure.Services.MediaUploads;
using Infrastructure.Services.OutboxProcessor;
using Infrastructure.Services.Payroll;
using Infrastructure.Services.Reporting;
using Infrastructure.Services.Services;
using Infrastructure.Services.Tasks;
using Infrastructure.Services.Vault;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Serilog;
using System.Security.Claims;
using System.Text;
using ClientService = Infrastructure.Services.Clients.ClientService;

namespace ClientAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<MinaretOpsDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MinaretOpsDbContext>()
            .AddDefaultTokenProviders();

            JWTSettings jwtOptions = builder.Configuration.GetSection("JWT").Get<JWTSettings>()
                ?? throw new Exception("Error in JWT Settings");

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSetting"));
            builder.Services.Configure<DiscordSettings>(builder.Configuration.GetSection("Discord"));
            builder.Services.Configure<RecaptchaSeetings>(builder.Configuration.GetSection("Recaptcha"));



            builder.Services.AddSingleton<JWTSettings>(jwtOptions);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier,
                        ValidateLifetime = true,

                    };
                });

            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("contact-limit", config =>
                {
                    config.PermitLimit = 5;
                    config.Window = TimeSpan.FromMinutes(1);
                    config.QueueLimit = 0;
                });
            });

            builder.Services.AddAuthorization();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJWTServices, JWTService>();
            builder.Services.AddScoped<IClientServices, ClientService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<IArchivedTaskService, ArchivedTaskService>();
            builder.Services.AddScoped<TaskHelperService>();
            builder.Services.AddScoped<IInternalTaskService, InternalTaskService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IPayrollService, PayrollService>();
            builder.Services.AddScoped<MediaUploadService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddScoped<IAttendanceDashboard, AttendanceDashboard>();
            builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            builder.Services.AddScoped<IBreakService, BreakService>();
            builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
            builder.Services.AddScoped<IComplaintService, ComplaintService>();
            builder.Services.AddScoped<IKPIService, KPIService>();
            builder.Services.AddScoped<IJobDescribtionService, JobDescriptionService>();
            builder.Services.AddScoped<IOutboxHandler, OutboxHandler>();
            builder.Services.AddScoped<IReportingService, ReportingService>();
            builder.Services.AddScoped<ICheckpointService, CheckpointService>();
            builder.Services.AddScoped<ICurrencyService, CurrencyService>();
            builder.Services.AddScoped<IContractService, ContractService>();
            builder.Services.AddScoped<IBranchService, BranchService>();
            builder.Services.AddScoped<IVaultService, VaultService>();
            builder.Services.AddScoped<ILeadService, Infrastructure.Services.Leads.LeadService>();
            builder.Services.AddScoped<IInvitationService, InvitationService>();
            builder.Services.AddHttpClient<IContactService, ContactService>();
            builder.Services.AddSingleton<DiscordService>();
            builder.Services.AddHostedService<DiscordHostedService>();
            builder.Services.AddHostedService<OutboxProcessor>();
            builder.Services.AddHostedService<OutboxCleaner>();
            builder.Services.AddHttpContextAccessor();
            //builder.Services.AddHttpClient();


            builder.Services.AddQuartz(q =>
            {
                // Register the job
                var jobKey = new JobKey("AttendanceJob");
                q.AddJob<AttendanceJob>(opts => opts.WithIdentity(jobKey));

                //TimeZoneInfo egyptTZ = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                // Hardcode Egypt timezone as UTC+3
                TimeZoneInfo egyptTZ = TimeZoneInfo.CreateCustomTimeZone(
                    "Egypt Time",
                    TimeSpan.FromHours(2),
                    "Egypt Time",
                    "Egypt Time");

                // Run at 23:59 every day
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("AttendanceJob-trigger")
                    //.WithCronSchedule("59 23 * * *") // CRON: sec min hour day month day-of-week
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 5).InTimeZone(egyptTZ))
                );
            });

            // Quartz hosted service
            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            //builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ClientProfile>();
                cfg.AddProfile<ServiceProfile>();
                cfg.AddProfile<ClientServiceProfile>();
                cfg.AddProfile<TaskGroupProfile>();
                cfg.AddProfile<TaskItemProfile>();
                cfg.AddProfile<TaskCommentProfile>();
                cfg.AddProfile<ArchivedTaskProfile>();
                cfg.AddProfile<InternalTaskProfile>();
                cfg.AddProfile<InternalTaskAssignmentProfile>();
                cfg.AddProfile<AttendanceRecordProfile>();
                cfg.AddProfile<LeaveRequestProfile>();
                cfg.AddProfile<AnnouncementProfile>();
                cfg.AddProfile<ComplaintProfile>();
                cfg.AddProfile<KPIIncedintProfile>();
                cfg.AddProfile<TaskHistoryProfile>();
                cfg.AddProfile<TaskResourcesProfile>();
                cfg.AddProfile<JDProfile>();
                cfg.AddProfile<BreakProfile>();
                cfg.AddProfile<CheckpointProfile>();
                cfg.AddProfile<SalaryPaymentProfile>();
                cfg.AddProfile<SalaryPeriodProfile>();
                cfg.AddProfile<CurrencyProfile>();
                cfg.AddProfile<ContractProfile>();
                cfg.AddProfile<BranchProfile>();
                cfg.AddProfile<VaultProfile>();
                cfg.AddProfile<LeadProfile>();
                cfg.AddProfile<EmpInvitationProfile>();
            });

            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

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
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendOnly", policy =>
                {
                    policy.WithOrigins(
                        "https://internal.theminaretagency.com",
                        "https://almnara.sa",
                        "http://localhost:4200"
                    )
                    //policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });


            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MinaretOps API v1"));
            app.UseDeveloperExceptionPage();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.Urls.Add("https://localhost:5001");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor,
                KnownProxies = { System.Net.IPAddress.Parse("127.0.0.1") }
                
            });

            //app.UseHttpsRedirection();
            app.Urls.Add("http://0.0.0.0:8080");

            app.UseRouting();
            app.UseCors("FrontendOnly");
            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<MinaretOpsDbContext>();
                await dbContext.Database.MigrateAsync();
                await DbSeeder.SeedAsync(services);
            }

            app.Run();
        }
    }
}
