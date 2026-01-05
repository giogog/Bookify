using Application;
using Application.Mapping;
using Application.MediatR;
using Application.Services;
using Contracts;
using Domain.Entities;
using Domain.Models;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace API.Extensions
{
    public static class ServiceConfiguration
    {
        public static void ConfigureSqlServer(this IServiceCollection services, IConfiguration configuration)=>
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), options =>
                {
                    options.MigrationsAssembly("API");
                }));

        public static void ConfigureInMemoryDb(this IServiceCollection services) =>
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseInMemoryDatabase("InMemoryDb"));

        public static void ConfigureDapper(this IServiceCollection services) =>
            services.AddScoped<IDapperDbConnection,DapperDbConnection>();

        public static void ConfigureIdentityService(this IServiceCollection services, IConfiguration config)
        {

            string Issuer = config.GetValue<string>("ApiSettings:JwtOptions:Issuer");
            string Audience = config.GetValue<string>("ApiSettings:JwtOptions:Audience");
            string TokenKey = config.GetValue<string>("ApiSettings:JwtOptions:Secret");

            services.AddIdentity<User, Role>(option =>
            {
                option.Password.RequireNonAlphanumeric = false;
                option.User.RequireUniqueEmail = true;


            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();




            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenKey)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = Issuer,
                        ValidAudience = Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        public static void GeneralConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddTransient<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IEmailSender, EmailSender>();

        }

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection service) =>
            service.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureAutomapper(this IServiceCollection services) => 
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

        public static void ConfigureJwtOptions(this IServiceCollection services, IConfiguration config) =>
            services.Configure<JwtOptions>(config.GetSection("ApiSettings:JwtOptions"));

        public static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {

                options.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string example: `Bearer Generated-JWT-Token`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                options.AddSecurityRequirement(
                new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[] {}
                    }
                });

            });
        }

        public static void ConfigureFluentEmail(this IServiceCollection services, IConfiguration config)
        {
            var emailSettings = config.GetSection("EmailOptions").Get<EmailOptions>();

            services
                .AddFluentEmail(emailSettings.FromEmail, emailSettings.SenderName)
                .AddSmtpSender(emailSettings.MailServer, emailSettings.MailPort, emailSettings.FromEmail, emailSettings.Password);
        }

        public static void ConfigureMediatR(this IServiceCollection services) =>
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatRConfiguration).Assembly));

        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://localhost:5003")
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials());
            });
    }
}
