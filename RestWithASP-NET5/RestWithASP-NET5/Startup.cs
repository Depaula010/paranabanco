using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestWithASP_NET5.Model.Context;
using RestWithASP_NET5.Business;
using RestWithASP_NET5.Business.Implementations;
using System;
using System.Collections.Generic;
using RestWithASP_NET5.Repository;
using RestWithASP_NET5.Repository.Implementations;
using Serilog;
using RestWithASP_NET5.Repository.Generic;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Rewrite;
using RestWithASP_NET5.Services;
using RestWithASP_NET5.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using RestWithASP_NET5.Services.Implementations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;

namespace RestWithASP_NET5
{
    public class Startup
    {
        public IConfiguration Configuration { get; }    
        public IWebHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //TOKEN
            var tokenConfigurations = new TokenConfiguration();
            new ConfigureFromConfigurationOptions<TokenConfiguration>(
                Configuration.GetSection("TokenConfigurations")).Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);
            //TOKEN

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenConfigurations.Issuer,
                    ValidAudience = tokenConfigurations.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret))
                };
            });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());
            });

            //CORS
            services.AddCors(options => options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));
            //CORS

            services.AddControllers();

            //RECUPERANDO A CONNECTION STRING DO BANCO DE DADOS DA APPSETTING.JSON
            string mySqlConnection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MySQLContext>(options => options.UseMySql(mySqlConnection));

            if (Environment.IsDevelopment())
            {
                MigrateDatabase(mySqlConnection);
            }

            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", MediaTypeHeaderValue.Parse("application/json"));
                options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("application/xml"));
            }).AddXmlSerializerFormatters();

            //var filterOptions = new HyperMediaFilterOptions();
            //filterOptions.ContentResponseEnricherList.Add(new PersonEnricher());

            //services.AddSingleton(filterOptions);

            //VERSIONAMENTO API
            services.AddApiVersioning();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1",
                    new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = "REST API .Net 6.0",
                        Version = "1",
                        Description = "API REST desenvolvida para CRUD de clientes",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact
                        {
                            Name = "Rafael de Paula",
                            Url = new Uri("https://github.com/Depaula010")
                        }
                    });
            });

            //INJEÇÃO DE DEPENDENCIAS 

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IPersonBusiness, PersonBusinessImplementation>(); 
           
            services.AddScoped<ILoginBusiness, LoginBusinessImplementation>();
           

            services.AddTransient<ITokenService, TokenService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPersonRepository, PersonRepositoryImplementation>();

            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //PARA O CORS FUNCIONAR ELE TEM QUE FICAR DEPOIS DE UseHttpsRedirection E UseRouting E ANTES DE UseEndpoints
            app.UseCors();
            //PARA O CORS FUNCIONAR ELE TEM QUE FICAR DEPOIS DE UseHttpsRedirection E UseRouting E ANTES DE UseEndpoints

            //SWAGGER
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "API REST desenvolvida para CRUD de clientes - v1");
            });
            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);
            //SWAGGER

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("DefaultApi", "{controller=values}/{id?}");
            });
        }

        private void MigrateDatabase(string mySqlConnection)
        {
            try
            {
                //EVOLVE É UMA FERRAMENTA QUE PERMITE EXECUTAR MIGRATIONS
                var evolveConnection = new MySql.Data.MySqlClient.MySqlConnection(mySqlConnection);
                var evolve = new Evolve.Evolve(evolveConnection, msg => Log.Information(msg))
                {
                    //CASO APAREÇA ERRO PODE SER ENCODING MUDAR PARA UTF8
                    Locations = new List<string> { "db/migrations", "db/dataset" },
                    IsEraseDisabled = true,
                };
                evolve.Migrate();
            }
            catch (Exception ex)
            {
                //RETORNANDO LOG DE ERRO
                Log.Error("Database migration failed", ex);
                throw;
            }
        }

    }
}
