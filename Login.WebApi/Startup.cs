using Login.Data;
using Login.Data.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MediatR;
using System.Reflection;
using Login.Services.UtilityServices.PasswordService;
using FluentValidation.AspNetCore;
using Login.Integration.Interface.Validator;

namespace Login.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services
                .AddControllers()
                .AddFluentValidation(options =>
                {
                    options.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    options.RegisterValidatorsFromAssemblyContaining<ValidatorBase<IRequest>>();
                });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    name: "v1",
                    info: new OpenApiInfo
                    {
                        Title = $"Login.WebApi - {HostEnvironment.EnvironmentName}",
                        Version = "v1"
                    });
                options.UseAllOfToExtendReferenceSchemas();
            }).AddSwaggerGenNewtonsoftSupport();

            services.AddDbContext<LoginDbContext>(options =>
            {
                //var connectionString = Configuration.GetConnectionString("DefaultConnection");
                //options.UseSqlServer(connectionString, options => options.EnableRetryOnFailure());
                options.UseInMemoryDatabase("Database");
            });

            services.AddMediatR(Assembly.GetAssembly(typeof(Services.Core.RequestHandler<,>)));

            services.AddSingleton<ICryptoService, CryptoService>();

            services.AddScoped<ILoginDbContext, LoginDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Login.WebApi v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
