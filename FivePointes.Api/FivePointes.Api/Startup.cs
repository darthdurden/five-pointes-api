using AutoMapper;
using FivePointes.Api.Adapters;
using FivePointes.Api.Configuration;
using FivePointes.Data;
using FivePointes.Api.Swagger;
using FivePointes.Logic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using FivePointes.Api.Dtos;
using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System.Text.Json.Serialization;
using Stripe;
using FivePointes.Logic.Configuration;

namespace FivePointes.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServices().AddAdapters(Environment);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder => {
                    builder.WithOrigins("http://localhost:3000", "https://finances.lookatmycode.com", "https://admin.carolynscottva.com", "https://www.carolynscottphotography.com", "https://carolynscottphotography.com", "https://admin.carolynscottphotography.com");
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds((type) =>
                {
                    return type.Name.EndsWith("Dto") ? type.Name.Substring(0, type.Name.Length - 3) : type.Name;
                });
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
                    BearerFormat = "JWT"
                });
                c.OperationFilter<AuthorizationOperationFilter>();
                c.SchemaFilter<ReadOnlySchemaFilter>();

                c.OperationFilter<FileDownloadOperationFilter>();
                c.MapType<Stream>(() => new OpenApiSchema { Description = "File download", Type = "string", Format = "binary" });
                c.MapType<JsonElement>(() => new OpenApiSchema { Description = "JSON Blob", Type = "object" });
                c.MapType<LocalDate>(() => new OpenApiSchema { Description = "Local Date", Type = "string", Format = "date" });
                c.MapType<LocalDate?>(() => new OpenApiSchema { Description = "Local Date", Type = "string", Format = "date" });
                c.MapType<Instant>(() => new OpenApiSchema { Description = "Instant", Type = "string", Format = "date-time" });
                c.MapType<Instant?>(() => new OpenApiSchema { Description = "Instant", Type = "string", Format = "date-time" });

                var schemasToRemove = new List<string>();
                c.SwaggerDoc("finances", new OpenApiInfo { Title = "Finances API", Version = "v1" });
                c.SwaggerDoc("csva", new OpenApiInfo { Title = "Carlolyn Scott Virtual Assistant API", Version = "v1" });
                c.SwaggerDoc("csp", new OpenApiInfo { Title = "Carlolyn Scott Photography API", Version = "v1" });

                c.UseInlineDefinitionsForEnums();
            });
            services.AddMvcCore(options =>
                {
                    if (Environment.IsDevelopment())
                    {
                        options.Filters.Add(new AllowAnonymousFilter());
                    }
                    else
                    {
                        options.Filters.Add(new AuthorizeFilter());
                    }
                    options.Filters.Add(new ProducesAttribute("application/json"));
                    options.Filters.Add(new ProducesDefaultResponseTypeAttribute(typeof(ErrorDto)));
                })
                .AddApiExplorer();

            services
                .Configure<JwtOptions>(options =>
                {
                    Configuration.Bind("Jwt", options);
                })
                .Configure<ClockifyOptions>(options =>
                {
                    Configuration.Bind("Clockify", options);
                })
                .Configure<StripeOptions>(options =>
                {
                    Configuration.Bind("Stripe", options);
                })
                .Configure<PortfolioOptions>(options =>
                {
                    Configuration.Bind("Portfolio", options);
                })
                .AddAuthorization()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(cfg =>
                {
                    var jwtOptions = new JwtOptions();
                    Configuration.Bind("Jwt", jwtOptions);
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(jwtOptions.Secret)),

                    };
                });

            services.AddAutoMapper(typeof(Startup));

            services
                .Configure<MysqlOptions>(options => {
                    Configuration.Bind("Mysql", options);
                })
                .AddDbContext<FivePointesDbContext>(options => {
                    var mysqlOptions = new MysqlOptions();
                    Configuration.Bind("Mysql", mysqlOptions);
                    options.UseMySQL(mysqlOptions.ConnectionString);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/csva/swagger.json", "Carolyn Scott Virtual Assistant API v1");
                c.SwaggerEndpoint("/swagger/finances/swagger.json", "Joint Finances API v1");
                c.SwaggerEndpoint("/swagger/csp/swagger.json", "Carolyn Scott Photography API v1");
            });

            app.UseAuthentication().UseAuthorization();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
